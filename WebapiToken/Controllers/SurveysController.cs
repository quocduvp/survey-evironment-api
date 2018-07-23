using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebapiToken.FuncProcess.ProcessSurvey;
using WebapiToken.FuncProcess.ResponseMessage;
using WebapiToken.Models;
using WebapiToken.Models.Panigation;

namespace WebapiToken.Controllers
{
    public class SurveysController : ApiController
    {

        private DBS db = new DBS();

        //get list survey type
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/v1/admin/survey_type")]
        public async Task<IHttpActionResult> GetSurveyType()
        {
            return Ok(db.surveys_type.Select(a => new {
                id = a.id,
                name = a.name,
                create_at = a.create_at
            }).ToList());
        }

        //get list survey
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/v1/admin/surveys")]
        public async Task<IHttpActionResult> GetListSurvey([FromUri]Page form)
        {
            return Ok(await FetchListSurveys.GetAllSurveys(form._page,form._page_size));
        }

        //get list surveys_unpublish
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/v1/admin/surveys_unpublish")]
        public async Task<IHttpActionResult> GetListSurveyUnpublish([FromUri]Page form)
        {
            return Ok(await FetchListSurveyUnpublish.GetAllSurveys(form._page, form._page_size));
        }

        //get list surveys_deleted
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/v1/admin/surveys_deleted")]
        public async Task<IHttpActionResult> GetListSurveyDeleted([FromUri]Page form)
        {
            return Ok(await FetchListSurveysDeleted.GetAllSurveys(form._page, form._page_size));
        }

        //get list surveys_text
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/v1/admin/surveys_text")]
        public async Task<IHttpActionResult> GetListSurveyForTypeText([FromUri]Page form)
        {
            return Ok(await FetchListSurveysTypeText.GetAllSurveys(form._page, form._page_size));
        }

        //get list survey
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/v1/admin/surveys_choice")]
        public async Task<IHttpActionResult> GetListSurveyForTypeChoice([FromUri]Page form)
        {
            return Ok(await FetchListSurveysTypeChoice.GetAllSurveys(form._page, form._page_size));
        }

        // get details
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/v1/admin/surveys/{id}")]
        public async Task<IHttpActionResult> GetSurveyDetails(int id)
        {
            return Ok(await FetchDetailsSurvey.GetDetailsSurvey(id));
        }

        // get details
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/v1/admin/surveys_restore/{id}")]
        public async Task<IHttpActionResult> RestoreSurvey(int id)
        {
            try
            {
                var find = db.surveys.Where(a => a.id == id).FirstOrDefault();
                if (find != null)
                {
                    find.deleted = false;
                    int check = await db.SaveChangesAsync();
                    if (check > 0)
                        return Ok(await FetchListSurveysDeleted.GetAllSurveys(1,10));
                    else
                        return BadRequest("Restore this survey fails.");
                }
                else
                {
                    return BadRequest("Survey not found.");
                }
            }
            catch
            {
                return BadRequest("Error.");
            }
        }

        //add new survey
        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("api/v1/admin/surveys")]
        public async Task<IHttpActionResult> CreateNewSurvey([FromBody]survey form)
        {
            try
            {
                var checkTitle = db.surveys.Where(a => a.title == form.title).FirstOrDefault();
                if(checkTitle == null)
                {
                    form.create_at = DateTime.Now;
                    db.surveys.Add(form);
                    int check = await db.SaveChangesAsync();
                    if(check > 0)
                    {
                        var msg = new ResponseMsg
                        {
                            message = "Create successful."
                        };
                        return Ok(msg);
                    }
                    else
                    {
                        return BadRequest("Create survey fails.");
                    }
                }
                else
                {
                    return BadRequest("Title survey have been same.");
                }
            }
            catch
            {
                return BadRequest("Error event.");
            }
        }

        // update details
        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("api/v1/admin/surveys/{id}")]
        public async Task<IHttpActionResult> UpdateSurvey([FromBody]survey form, int id)
        {
            try
            {
                var findSurvey = db.surveys.Where(a => a.id == id).FirstOrDefault();
                if(findSurvey != null)
                {
                    findSurvey.title = form.title;
                    findSurvey.description = form.description;
                    findSurvey.date_start = form.date_start;
                    int check = await db.SaveChangesAsync();
                    if(check > 0)
                    {
                        return Ok(await FetchDetailsSurvey.GetDetailsSurvey(id));
                    }
                    else
                    {
                        return BadRequest("Edit survey fails.");
                    }
                }
                else
                {
                    return BadRequest("Survey not found.");
                }
            }
            catch
            {
                return BadRequest("Error request.");
            }
        }

        // delete survey
        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("api/v1/admin/surveys/{id}")]
        public async Task<IHttpActionResult> DeleteSurveys(int id)
        {
            try
            {
                var findSurvey = db.surveys.Where(a => a.id == id && a.deleted == false).FirstOrDefault();
                if (findSurvey != null)
                {
                    findSurvey.deleted = true;
                    db.Entry(findSurvey).State = System.Data.Entity.EntityState.Modified;
                    int check = await db.SaveChangesAsync();
                    if (check > 0)
                    {
                        return Ok("Deleted successful.");
                    }
                    else
                    {
                        return BadRequest("Deleted fails.");
                    }
                }
                else
                {
                    return BadRequest("Surveys not found.");
                }
            }
            catch
            {
                return BadRequest("Error.");
            }
            
        }

        //publish/or unpublish surveys
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/v1/admin/surveys_publish/{id}")]
        public async Task<IHttpActionResult> PublishSurveys(int id)
        {
            try
            {
                survey findSurvey = (from a in db.surveys where a.id == id select a).FirstOrDefault();
                if (findSurvey != null)
                 {
                    findSurvey.publish = !findSurvey.publish;
                    var check = await db.SaveChangesAsync();
                    if (check > 0)
                        return Ok(await FetchDetailsSurvey.GetDetailsSurvey(id));
                    else
                        return BadRequest("Publish/unpublish fails.");
                }
                else
                {
                    return BadRequest("Surveys not found.");
                }
 
            }
            catch
            {
                return BadRequest("Error.");
            }
        }

        ///User
        ///
        [Authorize(Roles = "student,staff")]
        [HttpGet]
        [Route("api/v1/user/surveys_uncomming")]
        public async Task<IHttpActionResult> ListSurveyUncomming()
        {
            try
            {
                return Ok(await FetchListSurveyUncomming.ListSurveyUnComming());
            }
            catch
            {
                return BadRequest("Error");
            }
        }
        [Authorize(Roles = "student,staff")]
        [HttpGet]
        [Route("api/v1/user/surveys_incomming")]
        public async Task<IHttpActionResult> ListSurveyincomming()
        {
            try
            {
                return Ok(await FetchListSurveyUncomming.ListSurveyInComming());
            }
            catch
            {
                return BadRequest("Error");
            }
        }

        [Authorize(Roles = "student,staff")]
        [HttpGet]
        [Route("api/v1/user/surveys/{id}")]
        public async Task<IHttpActionResult> GetSurveyDetailsForUser(int id)
        {
            var surveys = db.surveys.Where(a => a.date_start <= DateTime.Now && a.publish == true
                            && a.deleted == false && a.id == id).FirstOrDefault();
            if (surveys != null)
                return Ok(await FetchDetailsSurvey.GetDetailsSurvey(surveys.id));
            else
                return BadRequest("Surveys not found.");
        }
    }
}
