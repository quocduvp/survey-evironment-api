using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebapiToken.Models;
using WebapiToken.Models.SurveyModel;

namespace WebapiToken.FuncProcess.ProcessSurvey
{
    public static class FetchListSurveys
    {
        private static DBS db = new DBS();
        public static async Task<int> getTotalRecord()
        {
            var total = (from a in db.surveys where a.deleted == false && a.publish == true select a).Count();
            return total;
        }

        public static async Task<int> getTotalPage(int page_size)
        {
            int totalPage = (int)(Math.Ceiling((double)await getTotalRecord() / page_size));
            return totalPage;
        }

        public static async Task<IQueryable<SurveyDetails>> getListSurveys(int skip_row, int page_size)
        {
            var list = (from a in db.surveys
                       from b in db.surveys_type
                       where a.surveys_type_id == b.id && a.deleted == false && a.publish == true
                        orderby a.create_at descending
                       select new SurveyDetails
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
                           surveys_type = new SurveyType
                           {
                               id = b.id,
                               name = b.name,
                               create_at = b.create_at
                           },
                           create_at = a.create_at
                       }).Skip(skip_row).Take(page_size);
            return list;
        }

        public static async Task<AllSurveys> GetAllSurveys(int page, int page_size)
        {
            //record của từng trang
            var skip_row = (page - 1) * page_size;
            var all = new AllSurveys
            {
                lists = await getListSurveys(skip_row,page_size),
                total = await getTotalRecord(),
                total_page = await getTotalPage(page_size),
                page_size = page_size,
                page = page
            };
            return all;
        }
    }
}