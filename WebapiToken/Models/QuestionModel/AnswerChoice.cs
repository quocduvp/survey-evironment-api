using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebapiToken.Models.QuestionModel
{
    public class AnswerChoice
    {
        public int id { get; set; }

        public int? question_id { get; set; }

        public string description { get; set; }

        public bool _checked { get; set; }

        public DateTime? create_at { get; set; }
    }
}