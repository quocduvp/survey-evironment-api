using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebapiToken.Models.SurveyModel
{
    public class AllSurveys
    {
        public virtual IQueryable<SurveyDetails> lists { get; set; }
        public int? total_page { get; set; }
        public int? total { get; set; }
        public int? page_size { get; set; }
        public int? page { get; set; }
    }
}