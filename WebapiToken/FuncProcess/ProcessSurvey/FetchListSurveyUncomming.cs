using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebapiToken.Models;

namespace WebapiToken.FuncProcess.ProcessSurvey
{
    public static class FetchListSurveyUncomming
    {
        private static DBS db = new DBS();
        public static async Task<object> ListSurveyUnComming()
        {
            using(var db = new DBS())
            {
                var list = db.surveys.Where(a => a.date_start > DateTime.Now && a.deleted == false && a.publish == true)
                .OrderByDescending(a => a.date_start)
                    .Select(a => new
                {
                    id = a.id,
                    title = a.title,
                    description = a.description,
                    thumb = a.thumb,
                    publish = a.publish,
                    date_start = a.date_start,
                    deleted = a.deleted,
                    surveys_type_id = a.surveys_type_id,
                    total_question = db.questions.Where(b => b.surveys_id == a.id).Count(),
                    create_at = a.create_at
                }).ToList();
                return list;
            }
        }
        public static async Task<object> ListSurveyInComming()
        {
            using (var db = new DBS())
            {
                var list = db.surveys.Where(a => a.date_start <= DateTime.Now && a.deleted == false && a.publish == true)
                    .OrderByDescending(a=>a.date_start)
                .Select(a => new
                {
                    id = a.id,
                    title = a.title,
                    description = a.description,
                    thumb = a.thumb,
                    publish = a.publish,
                    date_start = a.date_start,
                    deleted = a.deleted,
                    surveys_type_id = a.surveys_type_id,
                    total_question = db.questions.Where(b => b.surveys_id == a.id).Count(),
                    create_at = a.create_at
                }).ToList();
                return list;
            }
        }
        //public static async Task<object> DetailsSurvey(int id)
        //{

        //}
    }
}