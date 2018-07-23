using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using WebapiToken.FuncProcess.ResponseMessage;
using WebapiToken.Models;

namespace WebapiToken.Controllers
{
    public class FeedbacksController : ApiController
    {
        private DBS db = new DBS();

        //create feedback
        [Authorize(Roles = "student,staff")]
        [HttpPost]
        [Route("api/v1/user/feedback")]
        public async Task<IHttpActionResult> SubmitFeedBack(feedback form)
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var username = identity.Claims.Where(a => a.Type == ClaimTypes.Name).Select(c => c.Value).FirstOrDefault();
                var findUser = db.accounts.Where(a => a.username == username).FirstOrDefault();
                if(findUser != null)
                {
                    form.username = username;
                    form._checked = false;
                    form.create_at = DateTime.Now;
                    db.feedbacks.Add(form);
                    int check = await db.SaveChangesAsync();
                    if(check > 0)
                    {
                        var msg = new ResponseMsg();
                        msg.message = "Your request has been sent";
                        return Ok(msg);
                    }
                    else
                    {
                        return BadRequest("Feedback fails.");
                    }
                }
                else
                {
                    return BadRequest("Account not found.");
                }
                
            }
            catch(Exception e)
            {
                return BadRequest("Error.");
            }
            
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/v1/admin/feedback_checked")]
        public async Task<IHttpActionResult> GetListFeedbackChecked()
        {
            return Ok(await GetFeedbackAsync());
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/v1/admin/feedback_unchecked")]
        public async Task<IHttpActionResult> GetListFeedbackUnChecked()
        {
            return Ok(await GetFeedbackUnAsync());
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("api/v1/admin/feedback/{id}")]
        public async Task<IHttpActionResult> ChangeCheck(int id)
        {
            var finFeed = (from a in db.feedbacks where a.id == id select a).FirstOrDefault();
            if(finFeed != null)
            {
                if (finFeed._checked == true)
                    return Ok(await GetFeedbackAsync());
                else if(finFeed._checked == false) {
                    finFeed._checked = true;
                    db.Entry(finFeed).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Ok(await GetFeedbackAsync());
                }else
                    return Ok(await GetFeedbackAsync());
            }
            else
            {
                return BadRequest("Not found.");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("api/v1/admin/feedback/{id}")]
        public async Task<IHttpActionResult> ChangeDelete(int id)
        {
            var finFeed = (from a in db.feedbacks where a.id == id select a).FirstOrDefault();
            if (finFeed != null)
            {
                db.Entry(finFeed).State = System.Data.Entity.EntityState.Deleted;
                int check = await db.SaveChangesAsync();
                if (check > 0)
                    return Ok(await GetFeedbackAsync());
                else
                    return BadRequest("Delete fails.");

            }
            else
            {
                return BadRequest("Not found.");
            }
        }

        //
        private async Task<object> GetFeedbackAsync()
        {
            var list = db.feedbacks.Where(a=>a._checked == true).OrderByDescending(a=>a.create_at).Select(a => new {
                id = a.id,
                userinfo = db.accounts.Where(b => b.username == a.username).Select(b => new
                {
                    username = a.username,
                    user_id = b.id,
                    section = b.section,
                    avatar = db.details.Where(c=>c.account_id == b.id).Select(c=>c.avatar).FirstOrDefault(),
                    fullname = db.details.Where(c => c.account_id == b.id).Select(c => c.first_name + " " + c.last_name).FirstOrDefault(),
                }).FirstOrDefault(),
                title = a.title,
                description = a.description,
                check = a._checked,
                create_at = a.create_at
            }).ToList();
            return list;
        }
        private async Task<object> GetFeedbackUnAsync()
        {
            var list = db.feedbacks.Where(a => a._checked == false).OrderByDescending(a => a.create_at).Select(a => new {
                id = a.id,
                userinfo = db.accounts.Where(b => b.username == a.username).Select(b => new
                {
                    username = a.username,
                    user_id = b.id,
                    section = b.section,
                    avatar = db.details.Where(c => c.account_id == b.id).Select(c => c.avatar).FirstOrDefault(),
                    fullname = db.details.Where(c => c.account_id == b.id).Select(c => c.first_name + " " + c.last_name).FirstOrDefault(),
                }).FirstOrDefault(),
                title = a.title,
                description = a.description,
                check = a._checked,
                create_at = a.create_at
            }).ToList();
            return list;
        }
    }
}
