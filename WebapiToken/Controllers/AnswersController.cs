using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebapiToken.FuncProcess.ProcessSurvey;
using WebapiToken.Models;

namespace WebapiToken.Controllers
{
    public class AnswersController : ApiController
    {
        //model
        private DBS db = new DBS();
         
        //api created answer type choice choice
        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("api/v1/admin/question/{idQuestion}/choice")]
        public async Task<IHttpActionResult> CreateAskChoice(int idQuestion, [FromBody]question_choice form)
        {
            try
            {
                //find question null or not null
                var findQuestion = (from a in db.questions from b in db.surveys
                                    where a.surveys_id == b.id && a.id == idQuestion && b.surveys_type_id == 1 select a).FirstOrDefault();
                if (findQuestion != null)
                {
                    //find check answer duplicate
                    var answer_title = db.question_choice
                        .Where(a => a.description.ToString().ToLower() == form.description.ToString().ToLower()).FirstOrDefault();
                    if (answer_title == null)
                    {
                        form.question_id = findQuestion.id;
                        form._checked = false;
                        form.create_at = DateTime.Now;
                        db.question_choice.Add(form);
                        int check = await db.SaveChangesAsync(); //save
                        if (check > 0)
                            return Ok(await FetchDetailsSurvey.GetDetailsSurvey(Convert.ToInt32(findQuestion.surveys_id)));
                        else
                            return BadRequest("Create Answer fails");
                    }
                    else
                    {
                        return BadRequest("Answer duplicate.");
                    }
                }
                else
                {
                    return BadRequest("Question not found.");
                }
            }
            catch
            {
                return BadRequest("Error");
            }
        }

        //create answer type text value default
        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("api/v1/admin/question/{idQuestion}/text")]
        public async Task<IHttpActionResult> CreateAskText(int idQuestion)
        {
            try
            {
                //check question found and survey type is text
                var findQuestion = (from a in db.questions
                                    from b in db.surveys
                                    where a.surveys_id == b.id && a.id == idQuestion && b.surveys_type_id == 0
                                    select a).FirstOrDefault();
                //if != null insert to db
                if (findQuestion != null)
                {
                    //this is placeholder in frontend. value default
                    question_text form = new question_text();
                    form.text = "Let me know your opinion!";
                    form.question_id = findQuestion.id;
                    form.create_at = DateTime.Now;
                    db.question_text.Add(form);
                    int check = await db.SaveChangesAsync(); //save
                    if (check > 0)
                        return Ok(await FetchDetailsSurvey.GetDetailsSurvey(Convert.ToInt32(findQuestion.surveys_id)));
                    else
                        return BadRequest("Create answer fails");
                }
                else
                {
                    return BadRequest("Question not found.");
                }
            }
            catch
            {
                return BadRequest("Error");
            }
        }

        //update answer type choice
        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("api/v1/admin/question/{idQuestion}/choice/{idAnswer}")]
        public async Task<IHttpActionResult> UpdateAskChoice(int idQuestion,int idAnswer, [FromBody]question_choice form)
        {
            var findQuestion = (from a in db.questions
                                where a.id == idQuestion
                                select a).FirstOrDefault();
            if(findQuestion != null)
            {
                var answers = db.question_choice.Where(a => a.id == idAnswer && a.question_id == findQuestion.id).FirstOrDefault();
                if (answers != null)
                {
                    var answer_title = db.question_choice
                       .Where(a => a.description.ToString().ToLower() == form.description.ToString().ToLower() && a.id != idAnswer).FirstOrDefault();
                    if (answer_title == null)
                    {
                        answers.description = form.description;
                        db.Entry(answers).State = System.Data.Entity.EntityState.Modified;
                        int check = await db.SaveChangesAsync();
                        if (check > 0)
                            return Ok(await FetchDetailsSurvey.GetDetailsSurvey(Convert.ToInt32(findQuestion.surveys_id)));
                        else
                            return BadRequest("Update ask error.");
                    }
                    else
                    {
                        return BadRequest("Answer duplicate");
                    }
                }
                else
                {
                    return BadRequest("Asks for question not found.");
                }
            }
            else
            {
                return BadRequest("Question not found.");
            }
        }

        //api delete answer type choice
        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("api/v1/admin/question/{idQuestion}/choice/{idAnswer}")]
        public async Task<IHttpActionResult> DeleteAskChoice(int idQuestion, int idAnswer)
        {
            var findQuestion = (from a in db.questions
                                where a.id == idQuestion
                                select a).FirstOrDefault();
            if (findQuestion != null)
            {
                var ask = db.question_choice.Where(a => a.id == idAnswer && a.question_id == findQuestion.id).FirstOrDefault();
                if (ask != null)
                {
                    db.Entry(ask).State = System.Data.Entity.EntityState.Deleted;
                    int check = await db.SaveChangesAsync();
                    if (check > 0)
                        return Ok(await FetchDetailsSurvey.GetDetailsSurvey(Convert.ToInt32(findQuestion.surveys_id)));
                    else
                        return BadRequest("Delete ask error.");
                }
                else
                {
                    return BadRequest("Asks for question not found.");
                }
            }
            else
            {
                return BadRequest("Question not found.");
            }
        }
    }
}