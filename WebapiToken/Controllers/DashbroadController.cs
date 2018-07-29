using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebapiToken.Models;

namespace WebapiToken.Controllers
{
    public class DashbroadController : ApiController
    {
        private DBS db = new DBS();
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/v1/admin/dashboard")]
        public async Task<IHttpActionResult> DashBoard()
        {
            return Ok(await allBoard());
        }

        [Authorize(Roles = "admin,staff,user")]
        [HttpGet]
        [Route("api/v1/admin/dashboard/surveys/{idSurveys}")]
        public async Task<IHttpActionResult> DashBoard(int idSurveys)
        {
            var findSurveys = db.surveys.Where(a => a.id == idSurveys).FirstOrDefault();
            if(findSurveys!= null)
            {
                if(findSurveys.surveys_type_id == 0)
                {
                    if (await SurveysText(findSurveys.id) != null)
                        return Ok(await SurveysText(findSurveys.id));
                    else
                        return BadRequest("Fetch fails");
                }
                else
                {
                    if (await SurveysChoice(findSurveys.id) != null)
                        return Ok(await SurveysChoice(findSurveys.id));
                    else
                        return BadRequest("Fetch fails");
                }

            }
            else
            {
                return BadRequest("surveys not found.");
            }
        }

        //details surveys submit
        private async Task<object> SurveysText(int idSurveys)
        {
            //get details total submit from user
            var survey = db.surveys.Where(a => a.id == idSurveys).Select(a => new
            {
                id = a.id,
                publish = a.publish,
                deleted = a.deleted,
                surveys_type_id = a.surveys_type_id,
                title = a.title,
                thumb = a.thumb,
                date_start = a.date_start,
                description = a.description,
                questions = db.questions.Where(b => b.surveys_id == a.id).Select(b => new
                {
                    id = b.id,
                    priority = b.priority,
                    text = b.text,
                    answers = (from c in db.question_text from d in db.question_text_response
                               where c.question_id == b.id && c.id == d.question_text_id
                               select new
                               {
                                   id = c.id,
                                   question_text_id = d.question_text_id,
                                   accounts_id = d.accounts_id,
                                   avatar = db.details.Where(e=>e.account_id == d.accounts_id).Select(e=>e.avatar).FirstOrDefault(),
                                   username = db.accounts.Where(e=> e.id == d.accounts_id).Select(e=>e.username).FirstOrDefault(),
                                   text = d.text,
                                   create_at = d.create_at
                               }).ToList(),
                    create_at = b.create_at
                }).FirstOrDefault(),
                create_at = a.create_at
            }).FirstOrDefault();
            return survey;
        }
        private async Task<object> SurveysChoice(int idSurveys)
        {
            //get details total submit from user
            var survey = db.surveys.Where(a => a.id == idSurveys).Select(a => new
            {
                id = a.id,
                publish = a.publish,
                deleted = a.deleted,
                surveys_type_id  = a.surveys_type_id,
                title  = a.title,
                thumb = a.thumb,
                date_start = a.date_start,
                description = a.description,
                questions = db.questions.Where(b=>b.surveys_id == a.id).Select(b => new
                {
                    id = b.id,
                    priority = b.priority,
                    text = b.text,
                    answers = (from c in db.question_choice
                              where c.question_id == b.id select new
                              {
                                  id = c.id,
                                  text = c.description,
                                  total_submit = db.question_choice_response.Where(d=>d.question_choice_id == c.id).Count(),
                                  create_at = c.create_at
                              }).ToList(),
                    create_at = b.create_at
                }).ToList(),
                create_at = a.create_at
            }).FirstOrDefault();
            return survey;
        }

        //Dashbroard
        private async Task<object> allBoard()
        {
            return new
            {
                total_account = await Total_Accounts(),
                total_surveys = await Total_Surveys(),
                total_submitted = await Total_UserSubmitSurveys(),
                total_classrooms = await Total_Classrooms(),
                total_faculty = await Total_Faculty(),
                total_feedbacks = await Total_Feedback(),
                total_faqs = await Total_FAQs(),
            };
        }
        private async Task<object> Total_Accounts()
        {
            var accounts = db.accounts.Count();
            return accounts;
        }
        private async Task<object> Total_Surveys()
        {
            var surveys = db.surveys.Count();
            return surveys;
        }
        private async Task<object> Total_UserSubmitSurveys()
        {
            var surveys_submit = db.surveys_response.Count();
            return surveys_submit;
        }
        private async Task<object> Total_Classrooms()
        {
            var classrooms = db.classrooms.Count();
            return classrooms;
        }
        private async Task<object> Total_Faculty()
        {
            var faculties = db.faculties.Count();
            return faculties;
        }
        private async Task<object> Total_Feedback()
        {
            var feedbacks = db.feedbacks.Count();
            return feedbacks;
        }
        private async Task<object> Total_FAQs()
        {
            var faqs = db.faqs.Count();
            return faqs;
        }
    }
}
