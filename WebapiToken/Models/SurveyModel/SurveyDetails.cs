using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WebapiToken.Models.QuestionModel;

namespace WebapiToken.Models.SurveyModel
{
    public class SurveyDetails
    {
        public int id { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public string thumb { get; set; }

        public bool publish { get; set; }

        public bool deleted { get; set; }

        [Column(TypeName = "date")]
        public DateTime date_start { get; set; }

        public int? surveys_type_id { get; set; }

        public int total_question { get; set; }

        public DateTime? create_at { get; set; }

        public SurveyType surveys_type { get; set; }

        public List<Questions> questions { get; set; }
    }
}