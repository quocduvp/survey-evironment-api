using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebapiToken.Models.SurveyModel
{
    public class SurveyType
    {
        public int id { get; set; }

        public string name { get; set; }

        public DateTime? create_at { get; set; }
    }
}