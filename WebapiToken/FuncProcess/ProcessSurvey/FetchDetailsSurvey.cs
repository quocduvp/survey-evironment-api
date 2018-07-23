using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebapiToken.Models;
using WebapiToken.Models.QuestionModel;
using WebapiToken.Models.SurveyModel;

namespace WebapiToken.FuncProcess.ProcessSurvey
{
    public static class FetchDetailsSurvey
    {
        private static DBS db = new DBS();

        public static async Task<object> GetDetailsSurvey(int id)
        {
            var checkType = db.surveys.Where(a => a.id == id).Select(a => a.surveys_type_id).FirstOrDefault();
            if (checkType == 0)
                return await DetailsSurveysText(id);
            else if (checkType == 1)
                return await DetailsSurveysChoices(id);
            else
                return "Not found";
        }

        //
        static async Task<object> DetailsSurveysText(int id)
        {
            var survey = (from a in db.surveys
                          where a.id == id
                          select new
                          {
                              id = a.id,
                              title = a.title,
                              description = a.description,
                              surveys_type_id = a.surveys_type_id,
                              publish = a.publish,
                              deleted = a.deleted,
                              create_at = a.create_at,
                              thumb = a.thumb,
                              date_start = a.date_start,
                              total_questions = db.questions.Where(b=> b.surveys_id == a.id).Count(),
                              surveys_type = db.surveys_type.Where(b => b.id == a.surveys_type_id)
                                                    .Select(b => new {
                                                        id = b.id,
                                                        name = b.name,
                                                        create_at = b.create_at
                                                    }).FirstOrDefault(),
                              questions = db.questions.Where(b => b.surveys_id == a.id)
                                                    .Select(b => new {
                                                        id = b.id,
                                                        surveys_id = b.surveys_id,
                                                        priority = b.priority,
                                                        text = b.text,
                                                        create_at = b.create_at,
                                                        asks = db.question_text.Where(c => c.question_id == b.id)
                                                                .Select(c => new {
                                                                    id = c.id,
                                                                    question_id = c.question_id,
                                                                    text = c.text,
                                                                    create_at = c.create_at
                                                                }).FirstOrDefault()
                                                    }).ToList()
                          }).FirstOrDefault();
            return survey;
        }
        //
        static async Task<object> DetailsSurveysChoices(int id)
        {
            var survey = (from a in db.surveys
                          where a.id == id
                          select new
                          {
                              id = a.id,
                              title = a.title,
                              description = a.description,
                              surveys_type_id = a.surveys_type_id,
                              publish = a.publish,
                              deleted = a.deleted,
                              create_at = a.create_at,
                              thumb = a.thumb,
                              date_start = a.date_start,
                              total_questions = db.questions.Where(b => b.surveys_id == a.id).Count(),
                              surveys_type = db.surveys_type.Where(b => b.id == a.surveys_type_id)
                                                    .Select(b => new {
                                                        id = b.id,
                                                        name = b.name,
                                                        create_at = b.create_at
                                                    }).FirstOrDefault(),
                              questions = db.questions.Where(b => b.surveys_id == a.id)
                                                    .Select(b => new {
                                                        id = b.id,
                                                        surveys_id = b.surveys_id,
                                                        priority = b.priority,
                                                        text = b.text,
                                                        create_at = b.create_at,
                                                        asks = db.question_choice.Where(c => c.question_id == b.id)
                                                                .Select(c => new {
                                                                    id = c.id,
                                                                    question_id = c.question_id,
                                                                    description = c.description,
                                                                    _checked = c._checked,
                                                                    create_at = c.create_at
                                                                }).ToList()
                                                    }).ToList()
                          }).FirstOrDefault();
            return survey;
        }

        ///details survey user message all 
        public static async Task<object> DetailsSurveysResponseText(int id)
        {
            using (var db = new DBS())
            {
                var message = (from a in db.surveys
                               where a.id == id
                               select new
                               {
                                   id = a.id,
                                   surveys_id = a.surveys_type_id,
                                   title = a.title,
                                   description = a.description,
                                   thumb = a.thumb,
                                   date_start = a.date_start,
                                   joiner = db.surveys_response.Where(b => b.surveys_id == a.id).Select(b => new
                                   {
                                       id = b.id,
                                       surveys_id = b.surveys_id,
                                       accounts_id = b.accounts_id,
                                       message_questions = db.question_text_response
                                       .Where(c => c.accounts_id == b.accounts_id && c.surveys_response_id == b.id)
                                       .Select(c=> new {
                                           id = c.id,
                                           question_id = c.question_id,
                                           question_text_id = c.question_id,
                                           account_id = c.accounts_id,
                                           surveys_response_id = c.surveys_response_id,
                                           text = c.text,
                                           create_at = c.create_at
                                       }).FirstOrDefault(),
                                       userinfo = db.details.Where(c => c.account_id == b.accounts_id).Select(c => new
                                       {
                                           id = c.id,
                                           accounts_id = c.account_id,
                                           fullname = c.first_name + " " + c.last_name,
                                           avatar = c.avatar,
                                       }).FirstOrDefault(),
                                       username = b.username,
                                       create_at = b.create_at
                                   }).ToList(),
                                   create_at = a.create_at
                               }).FirstOrDefault();
                return message;
            }
        }
        ///choice
        public static async Task<object> DetailsSurveysResponseChoice(int id)
        {
            using (var db = new DBS())
            {
                var message = (from a in db.surveys
                               where a.id == id
                               select new
                               {
                                   id = a.id,
                                   surveys_id = a.surveys_type_id,
                                   title = a.title,
                                   description = a.description,
                                   thumb = a.thumb,
                                   date_start = a.date_start,
                                   joiner = db.surveys_response.Where(b => b.surveys_id == a.id).Select(b => new
                                   {
                                       id = b.id,
                                       surveys_id = b.surveys_id,
                                       accounts_id = b.accounts_id,
                                       message_questions = db.question_choice_response
                                       .Where(c => c.accounts_id == b.accounts_id && c.surveys_response_id == b.id)
                                       .Select(c => new {
                                           id = c.id,
                                           question_id = c.question_id,
                                           question_choice_id = c.question_choice_id,
                                           text = db.question_choice.Where(d => d.question_id == c.question_id && d.id == c.question_choice_id).Select(d => d.description)
                                           .FirstOrDefault(),
                                           account_id = c.accounts_id,
                                           surveys_response_id = c.surveys_response_id,
                                           create_at = c.create_at
                                       }).ToList(),
                                       userinfo = db.details.Where(c => c.account_id == b.accounts_id).Select(c => new
                                       {
                                           id = c.id,
                                           accounts_id = c.account_id,
                                           fullname = c.first_name + " " + c.last_name,
                                           avatar = c.avatar,
                                       }).FirstOrDefault(),
                                       username = b.username,
                                       create_at = b.create_at
                                   }).ToList(),
                                   create_at = a.create_at
                               }).FirstOrDefault();
                return message;
            }
        }

        //details survey response for user
        public static async Task<object> DetailsSurveyUserText(int id, string username)
        {
            using(var db = new DBS())
            {
                var surveyRes = (from a in db.surveys from b in db.surveys_response
                                 where a.id == b.surveys_id && b.id == id && b.username == username
                                 select new {
                                     id = a.id,
                                     surveys_id = a.surveys_type_id,
                                     title = a.title,
                                     description = a.description,
                                     thumb = a.thumb,
                                     date_start = a.date_start,
                                     username = b.username,
                                     questions = db.question_text_response
                                        .Where(c=>c.surveys_response_id == b.id && c.accounts_id == b.accounts_id).Select(c=>new {
                                            id = c.id,
                                            question_text_ida = c.question_text_id,
                                            question_id = c.question_id,
                                            accounts_id = c.accounts_id,
                                            text = c.text,
                                            create_at = b.create_at
                                        }).FirstOrDefault(),
                                     userinfo = db.details.Where(c => c.account_id == b.accounts_id).Select(c => new
                                     {
                                         id = c.id,
                                         accounts_id = c.account_id,
                                         fullname = c.first_name + " " + c.last_name,
                                         avatar = c.avatar,
                                     }).FirstOrDefault(),
                                     create_at = a.create_at
                                 }).FirstOrDefault();
                return surveyRes;
            }
        }
        public static async Task<object> DetailsSurveyUserChoice(int id, string username)
        {
            using (var db = new DBS())
            {
                var surveyRes = (from a in db.surveys
                                 from b in db.surveys_response
                                 where a.id == b.surveys_id && b.id == id && b.username == username
                                 select new
                                 {
                                     id = a.id,
                                     surveys_id = a.surveys_type_id,
                                     title = a.title,
                                     description = a.description,
                                     thumb = a.thumb,
                                     date_start = a.date_start,
                                     username = b.username,
                                     questions = db.question_choice_response
                                            .Where(c => c.surveys_response_id == b.id && c.accounts_id == b.accounts_id).Select(c => new {
                                               id = b.id,
                                               question_text_id = c.question_choice_id,
                                               question_id = c.question_id,
                                               accounts_id = c.accounts_id,
                                               text = db.question_choice.Where(d => d.id == c.question_choice_id).Select(d => d.description).FirstOrDefault(),
                                               create_at = c.create_at
                                            }).ToList()
                                            ,
                                     userinfo = db.details.Where(c => c.account_id == b.accounts_id).Select(c => new
                                     {
                                         id = c.id,
                                         accounts_id = c.account_id,
                                         fullname = c.first_name + " " + c.last_name,
                                         avatar = c.avatar,
                                     }).FirstOrDefault(),
                                     create_at = a.create_at
                                 }).FirstOrDefault();
                return surveyRes;
            }
        }


    }
}