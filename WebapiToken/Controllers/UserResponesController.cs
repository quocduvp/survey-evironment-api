using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using WebapiToken.FuncProcess.ProcessSurvey;
using WebapiToken.Models;
using WebapiToken.Models.QuestionModel;

namespace WebapiToken.Controllers
{
    public class UserResponesController : ApiController
    {
        private static DBS db = new DBS();

        //overview of the survey
        [Authorize(Roles = "admin,student,staff")]
        [HttpGet]
        [Route("api/v1/survey/response/{idSurvey}")]
        public async Task<IHttpActionResult> DetailsSurveysResponse(int idSurvey)
        {
            var findSurvey = (from a in db.surveys
                             where a.id == idSurvey && a.deleted == false && a.publish == true
                              select a).FirstOrDefault();
            if (findSurvey != null)
            {
                if (findSurvey.surveys_type_id == 0)
                {
                    return Ok(await FetchDetailsSurvey.DetailsSurveysResponseText(findSurvey.id));
                }
                else if (findSurvey.surveys_type_id == 1)
                {
                    return Ok(await FetchDetailsSurvey.DetailsSurveysResponseChoice(findSurvey.id));
                }
                else
                {
                    return BadRequest("Surveys not found.");
                }
            }
            else
            {
                return BadRequest("Surveys not found.");
            }
        }

        //student and staff submit joining surveys
        [Authorize(Roles = "student,staff")]
        [HttpPost]
        [Route("api/v1/survey/join/{idSurvey}")]
        public async Task<IHttpActionResult> CreateSurveysResponse(int idSurvey)
        {
            //find and check username
            var identity = (ClaimsIdentity)User.Identity;
            var username = identity.Claims.Where(a => a.Type == ClaimTypes.Name).Select(c => c.Value).FirstOrDefault();
            var findUser = db.accounts.Where(a => a.username == username).FirstOrDefault();
            if(findUser != null)
            {
                //find survey with date start less than current date
                var findSurvey = db.surveys.Where(a => a.id == idSurvey && 
                a.date_start <= DateTime.Now &&
                a.deleted == false && a.publish == true).FirstOrDefault();
                if (findSurvey != null)
                {
                    //find this user joined if not join create else update
                    var findMessage = db.surveys_response.Where(a => a.surveys_id == idSurvey && a.accounts_id == findUser.id).FirstOrDefault();
                    if (findMessage == null) {
                        surveys_response form = new surveys_response();
                        form.surveys_id = findSurvey.id;
                        form.accounts_id = findUser.id;
                        form.create_at = DateTime.Now;
                        form.username = username;
                        db.surveys_response.Add(form);
                        int check = await db.SaveChangesAsync();
                        if (check > 0)
                           return Ok(await FetchDetailsSurvey.GetDetailsSurvey(findSurvey.id));
                        else
                            return BadRequest("Create fails.");

                    }
                    else
                    {
                        findMessage.create_at = DateTime.Now;
                        db.Entry(findMessage).State = System.Data.Entity.EntityState.Modified;
                        int check = await db.SaveChangesAsync();
                        if (check > 0)
                            return Ok(await FetchDetailsSurvey.GetDetailsSurvey(findSurvey.id));
                        else
                            return BadRequest("update fails.");
                    }
                }
                else
                {
                    return BadRequest("Survey not found.");
                }
            }
            else
            {
                return BadRequest("Account not found.");
            }
            
        }

        //submit survey
        [Authorize(Roles = "student,staff")]
        [HttpPost]
        [Route("api/v1/survey/join/{idSurvey}/question/{idQuestion}")]
        public async Task<IHttpActionResult> SubmitQuestionResponse(int idSurvey, int idQuestion,[FromBody]ResponseQuestion form)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var username = identity.Claims.Where(a => a.Type == ClaimTypes.Name).Select(c => c.Value).FirstOrDefault();
            var findUser = db.accounts.Where(a => a.username == username).FirstOrDefault();
            if (findUser != null)
            {
                var findSurvey = db.surveys.Where(a => a.id == idSurvey &&
                a.date_start <= DateTime.Now
                && a.deleted == false && a.publish == true).FirstOrDefault();
                if(findSurvey.surveys_type_id == 0)
                {
                    //Check question null or not null
                    var question = (from a in db.questions
                                    from b in db.question_text
                                    where a.id == idQuestion && b.question_id == a.id
                                    select b).FirstOrDefault();
                    if (question != null)
                    {
                        //See if the user has joined
                        var surveyRes = db.surveys_response
                            .Where(a => a.surveys_id == findSurvey.id && a.username == username).FirstOrDefault();
                        if (surveyRes != null)
                        {
                            //tim xem da co cau tra loi chua
                            var findQuestionRes = db.question_text_response
                                .Where(a => a.accounts_id == findUser.id && a.question_id == question.question_id).FirstOrDefault();
                            if (findQuestionRes == null)
                            {
                                question_text_response text = new question_text_response();
                                text.question_id = question.question_id;
                                text.surveys_response_id = surveyRes.id;
                                text.question_text_id = question.id;
                                text.accounts_id = findUser.id;
                                text.create_at = DateTime.Now;
                                text.text = form.text;
                                db.question_text_response.Add(text);
                                int check = await db.SaveChangesAsync();
                                if (check > 0)
                                {
                                    return Ok("Ok"); //tra ve thong tin
                                }
                                else
                                {
                                    return BadRequest("Submit fails.");
                                }
                            }
                            else
                            {
                                findQuestionRes.create_at = DateTime.Now;
                                findQuestionRes.text = form.text;
                                db.Entry(findQuestionRes).State = System.Data.Entity.EntityState.Modified;
                                if(await db.SaveChangesAsync() > 0)
                                {
                                    return Ok("Update submit success.");
                                }
                                else
                                {
                                    return BadRequest("Submit fails.");
                                }
                            }
                        }
                        else
                        {
                            return BadRequest("You dont join surveys.");
                        }
                    }
                    else
                    {
                        return BadRequest("Question not found.");
                    }
                }
                else if(findSurvey.surveys_type_id == 1)
                {
                    //find question for surveys
                    var question = (from a in db.questions
                                    from b in db.question_choice
                                    where a.id == idQuestion && b.question_id == a.id
                                    select b).FirstOrDefault();
                    if (question != null)
                    {
                        //find survey for username
                        var surveyRes = db.surveys_response
                            .Where(a => a.surveys_id == findSurvey.id && a.username == username).FirstOrDefault();
                        if (surveyRes != null)
                        {
                            //See if the user has answered the question
                            var findQuestionRes = db.question_choice_response
                                .Where(a => a.accounts_id == findUser.id && a.question_id == question.question_id).FirstOrDefault();
                            if (findQuestionRes == null)
                            {
                                //create new answer from user
                                question_choice_response choice = new question_choice_response();
                                choice.question_id = question.question_id;
                                choice.surveys_response_id = surveyRes.id;
                                choice.accounts_id = findUser.id;
                                choice.create_at = DateTime.Now;
                                choice.question_choice_id = form.question_choice_id;
                                db.question_choice_response.Add(choice);
                                int check = await db.SaveChangesAsync();
                                if (check > 0)
                                {
                                    return Ok("Success");
                                }
                                else
                                {
                                    return BadRequest("Submit fails.");
                                }
                            }
                            else
                            {
                                //Update new aswer from user
                                findQuestionRes.question_choice_id = form.question_choice_id;
                                findQuestionRes.create_at = DateTime.Now;
                                db.Entry(findQuestionRes).State = System.Data.Entity.EntityState.Modified;
                                if (await db.SaveChangesAsync() > 0)
                                {
                                    return Ok("Update submit success.");
                                }
                                else
                                {
                                    return BadRequest("Submit fails.");
                                }
                            }
                        }
                        else
                        {
                            return BadRequest("You dont join surveys.");
                        }
                    }
                    else
                    {
                        return BadRequest("Question not found.");
                    }
                }
                else
                {
                    return BadRequest("Survey not found.");
                }
            }
            else
            {
                return BadRequest("Account not found");
            }
        }
       
        //list surveys of the user submitted
        [Authorize(Roles = "student,staff")]
        [HttpGet]
        [Route("api/v1/survey_response")]
        public async Task<IHttpActionResult> ListSurveyResponse()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var username = identity.Claims.Where(a => a.Type == ClaimTypes.Name).Select(c => c.Value).FirstOrDefault();
            var findUser = db.accounts.Where(a => a.username == username).FirstOrDefault();
            if(findUser != null)
            {
                var messageSurvey = db.surveys_response.Where(a => a.username == username)
                    .Select(a => new
                    {
                        id = a.id,
                        username = a.username,
                        survey = db.surveys.Where(b=>b.id == a.surveys_id).Select(b=> new{
                            id = b.id,
                            title = b.title,
                            description = b.description,
                            thumb = b.thumb,
                            date_start = b.date_start,
                            create_at = b.create_at,
                        }).FirstOrDefault(),
                        userinfo = db.details.Where(b => b.account_id == findUser.id).Select(c => new
                        {
                            id = c.id,
                            accounts_id = c.account_id,
                            fullname = c.first_name + " " + c.last_name,
                            avatar = c.avatar,
                        }).FirstOrDefault(),
                        create_at = a.create_at
                    }).ToList();
                return Ok(messageSurvey);
            }
            else
            {
                return BadRequest("Account not found.");
            }
        }
        //Details survey response
        [Authorize(Roles = "student,staff")]

        [HttpGet]
        [Route("api/v1/survey_response/{idSurveyResponse}")]
        public async Task<IHttpActionResult> DetailsSurveyResponse(int idSurveyResponse)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var username = identity.Claims.Where(a => a.Type == ClaimTypes.Name).Select(c => c.Value).FirstOrDefault();
            var findUser = db.accounts.Where(a => a.username == username).FirstOrDefault();
            if (findUser != null)
            {
                var findSurvey = (from a in db.surveys
                                  from b in db.surveys_response
                                  where a.id == b.surveys_id && b.id == idSurveyResponse && b.username == username
                                  select a).FirstOrDefault();
                if (findSurvey != null)
                {
                    if (findSurvey.surveys_type_id == 0)
                        return Ok(await FetchDetailsSurvey.DetailsSurveyUserText(idSurveyResponse, username));
                    else if (findSurvey.surveys_type_id == 1)
                        return Ok(await FetchDetailsSurvey.DetailsSurveyUserChoice(idSurveyResponse, username));
                    else
                        return BadRequest("Survey not found");
                }
                else
                {
                    return BadRequest("Not found.");
                }
            }
            else
            {
                return BadRequest("Account not found.");
            }
        }
    }
}
