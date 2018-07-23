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
    public class QuestionsController : ApiController
    {
        private DBS db = new DBS();
        //create question
        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("api/v1/admin/questions")]
        public async Task<IHttpActionResult> CreateQuestion([FromBody]question form)
        {
            try
            {
                var findSurvey = db.surveys.Where(a => a.id == form.surveys_id).FirstOrDefault();
                if (findSurvey != null)
                {
                    form.create_at = DateTime.Now;
                    db.questions.Add(form);
                    int check = await db.SaveChangesAsync();
                    if (check > 0)
                    {
                        return Ok(await FetchDetailsSurvey.GetDetailsSurvey(findSurvey.id));
                    }
                    else
                    {
                        return BadRequest("Create question error.");
                    }
                }
                else
                    return BadRequest("Survey not found.");
            }
            catch
            {
                return BadRequest("Error question.");
            }
        }
        //thag khoa lam toi qua
        //update question
        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("api/v1/admin/questions/{id}")]
        public async Task<IHttpActionResult> UpdateQuestion(int id, [FromBody]question form)
        {
            var todo = db.questions.Where(a => a.id == id).FirstOrDefault();
            if (todo != null)
            {
                todo.priority = form.priority;
                todo.text = form.text;
                todo.create_at = DateTime.Now;

                int check = await db.SaveChangesAsync();
                if (check > 0)
                {
                    return Ok(await FetchDetailsSurvey.GetDetailsSurvey(Convert.ToInt32(todo.surveys_id)));
                }
                else
                {
                    return BadRequest("Update fails.");
                }
            }

            else
            {
                return BadRequest("Question not found!");
            }

        }
        //Remove question
        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("api/v1/admin/questions/{id}")]
        public async Task<IHttpActionResult> Deletequestion(int id)
        {
            // get survey
            var findSurvey = (from a in db.questions from b in db.surveys
                        where a.surveys_id == b.id && a.id == id select b).FirstOrDefault();
            if (findSurvey != null)
            {
                if (findSurvey.surveys_type_id == 0)
                {
                    //find question
                    var question = db.questions.Where(a => a.id == id).FirstOrDefault();
                    //tim cau hoi lien quan
                    var ask = db.question_text.Where(a => a.question_id == question.id).FirstOrDefault();
                    var findResponse = (from a in db.question_text_response
                                        where a.question_id == question.id
                                        select a).ToList();
                    if (findResponse.Count() > 0)
                    {
                        db.question_text_response.RemoveRange(findResponse);
                        await db.SaveChangesAsync();
                    }
                    if (ask != null)
                    {
                        db.question_text.Remove(ask);
                        int check = await db.SaveChangesAsync();
                        if (check > 0)
                        {
                            db.questions.Remove(question);
                            int check2 = await db.SaveChangesAsync();
                            if (check2 > 0)
                                return Ok(await FetchDetailsSurvey.GetDetailsSurvey(Convert.ToInt32(findSurvey.id)));
                            else
                                return BadRequest("Remove fails.");
                        }
                        else
                        {
                            return BadRequest("Remove fails.");
                        }
                    }
                    else if (question != null)
                    {
                        db.questions.Remove(question);
                        int check2 = await db.SaveChangesAsync();
                        if (check2 > 0)
                            return Ok(await FetchDetailsSurvey.GetDetailsSurvey(Convert.ToInt32(findSurvey.id)));
                        else
                            return BadRequest("Remove fails.");
                    }
                    else
                    {
                        return BadRequest("Question not found.");
                    }
                }
                else if (findSurvey.surveys_type_id == 1)
                {
                    //find question
                    var question = db.questions.Where(a =>a.surveys_id == findSurvey.id && a.id == id).FirstOrDefault();
                    //tim cau hoi lien quan
                    var ask = db.question_choice.Where(a => a.question_id == question.id).ToList();
                    var findResponse = (from a in db.question_choice_response
                                        where a.question_id == question.id
                                        select a).ToList();
                    if (findResponse.Count() > 0)
                    {
                        db.question_choice_response.RemoveRange(findResponse);
                        await db.SaveChangesAsync();
                    }
                    if (ask.Count() > 0)
                    {
                        db.question_choice.RemoveRange(ask);
                        int check = await db.SaveChangesAsync();
                        if (check > 0)
                        {
                            db.questions.Remove(question);
                            int check2 = await db.SaveChangesAsync();
                            if (check2 > 0)
                                return Ok(await FetchDetailsSurvey.GetDetailsSurvey(Convert.ToInt32(findSurvey.id)));
                            else
                                return BadRequest("Remove failszxczxc.");
                        }
                        else
                        {
                            return BadRequest("Remove failsss.");
                        }
                    }
                    else if (question != null && ask.Count() == 0)
                    {
                        db.questions.Remove(question);
                        int check2 = await db.SaveChangesAsync();
                        if (check2 > 0)
                            return Ok(await FetchDetailsSurvey.GetDetailsSurvey(Convert.ToInt32(findSurvey.id)));
                        else
                            return BadRequest("Remove fails.");
                    }
                    else
                    {
                        return BadRequest("Question not found.");
                    }
                }
                else
                {
                    return BadRequest("Surveys not found");
                }
            }
            else
            {
                return BadRequest("Survey not found");
            }
        }
        //Detail question
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/v1/admin/questions_text/{id}")]
        public async Task<IHttpActionResult> Detailquestion(int id)
        {

            var todo = (from a in db.questions
                        where a.id == id
                        select new
                        {
                            id = a.id,
                            surveys_id = a.surveys_id,
                            text = a.text,
                            priority = a.priority,
                            create_at = a.create_at,
                            asks = db.question_text.Where(b => b.question_id == a.id).Select(b => new {
                                id = b.id,
                                question_id = b.question_id,
                                text = b.text,
                                create_at = b.create_at
                            }).FirstOrDefault()
                        }).FirstOrDefault();
            if (todo != null)
            {
                return Ok(todo);
            }
            else
            {
                return BadRequest("Question not found!");
            }
        }
        //Get detail question choice
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/v1/admin/questions_choice/{id}")]
        public async Task<IHttpActionResult> Detailquestion_choice(int id)
        {

            var todo = (from a in db.questions
                        where a.id == id
                        select new
                        {
                            id = a.id,
                            surveys_id = a.surveys_id,
                            text = a.text,
                            priority = a.priority,
                            create_at = a.create_at,
                            asks = db.question_choice.Where(b => b.question_id == a.id).Select(b => new {
                                id = b.id,
                                question_id = b.question_id,
                                description = b.description,
                                _checked = b._checked,
                                create_at = b.create_at
                            }).ToList()
                        }).FirstOrDefault();
            if (todo != null)
            {
                return Ok(todo);
            }
            else
            {
                return BadRequest("Question not found!");
            }
        }

    }
}
