using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebapiToken.Models.SurveyModel
{
    public class Questions
    {
        public int id { get; set; }

        public int? surveys_id { get; set; }

        public string text { get; set; }

        public int priority { get; set; }

        public DateTime? create_at { get; set; }
    }
}