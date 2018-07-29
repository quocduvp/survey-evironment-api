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
    public class AsksController : ApiController
    {
        //model
        private DBS db = new DBS();
        //api created answer choice and text
        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("api/v1/admin/question/{id}/choice")]
        public async Task<IHttpActionResult> CreateAskChoice(int id,[FromBody]question_choice form)
        {
            try
            {
                //chỉ kiểm tra thg question có tồn tại hay không thoi va check question dang choice
                var findQuestion = (from a in db.questions from b in db.surveys
                                    where a.surveys_id == b.id && a.id == id && b.surveys_type_id == 1 select a).FirstOrDefault();
                //nếu tồn tại tiến hành thêm câu hỏi của  question đó vào csdl
                if (findQuestion != null)
                {
                    //them vao csdl description
                    form.question_id = findQuestion.id;
                    form._checked = false;
                    form.create_at = DateTime.Now;
                    db.question_choice.Add(form);
                    int check = await db.SaveChangesAsync(); //save lai
                    if (check > 0)
                        return Ok(await FetchDetailsSurvey.GetDetailsSurvey(Convert.ToInt32(findQuestion.surveys_id)));
                    else
                        return BadRequest("Create asks fails");
                    //xong chuc nang them khoan cho xiu
                }
                //k tim thấy
                else
                {
                    return BadRequest("Question not found.");
                }
            }
            catch
            {
                //thong bao loi
                return BadRequest("Error");
            }
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("api/v1/admin/question/{id}/text")]
        public async Task<IHttpActionResult> CreateAskText(int id)
        {
            try
            {
                //check question found and survey type is text
                var findQuestion = (from a in db.questions
                                    from b in db.surveys
                                    where a.surveys_id == b.id && a.id == id && b.surveys_type_id == 0
                                    select a).FirstOrDefault();
                //if != null insert to db
                if (findQuestion != null)
                {
                    question_text form = new question_text();
                    form.text = "Let me know your opinion!";
                    form.question_id = findQuestion.id;
                    form.create_at = DateTime.Now;
                    db.question_text.Add(form);
                    int check = await db.SaveChangesAsync(); //save
                    if (check > 0)
                        return Ok(await FetchDetailsSurvey.GetDetailsSurvey(Convert.ToInt32(findQuestion.surveys_id)));
                    else
                        return BadRequest("Create asks fails");
                    //xong chuc nang them khoan cho xiu
                }
                //k tim thấy
                else
                {
                    return BadRequest("Question not found.");
                }
            }
            catch
            {
                //thong bao loi
                return BadRequest("Error");
            }
        }
        //api edit cau tra loi kieu choice
        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("api/v1/admin/question/{idQuestion}/choice/{idAsk}")]
        public async Task<IHttpActionResult> UpdateAskChoice(int idQuestion,int idAsk, [FromBody]question_choice form)
        {
            var findQuestion = (from a in db.questions
                                where a.id == idQuestion
                                select a).FirstOrDefault();
            if(findQuestion != null)
            {
                var ask = db.question_choice.Where(a => a.id == idAsk && a.question_id == findQuestion.id).FirstOrDefault();
                if (ask != null)
                {
                    ask.description = form.description;
                    db.Entry(ask).State = System.Data.Entity.EntityState.Modified;
                    int check = await db.SaveChangesAsync();
                    if (check > 0)
                        return Ok(await FetchDetailsSurvey.GetDetailsSurvey(Convert.ToInt32(findQuestion.surveys_id)));
                    else
                        return BadRequest("Update ask error.");
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
        //api delete cau tra loi kieu choice
        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("api/v1/admin/question/{idQuestion}/choice/{idAsk}")]
        public async Task<IHttpActionResult> DeleteAskChoice(int idQuestion, int idAsk)
        {
            var findQuestion = (from a in db.questions
                                where a.id == idQuestion
                                select a).FirstOrDefault();
            if (findQuestion != null)
            {
                var ask = db.question_choice.Where(a => a.id == idAsk && a.question_id == findQuestion.id).FirstOrDefault();
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