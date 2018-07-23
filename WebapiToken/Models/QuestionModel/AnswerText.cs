using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebapiToken.Models.QuestionModel
{
    public class AnswerText
    {
        public int id { get; set; }

        public int? question_id { get; set; }

        public string text { get; set; }

        public DateTime? create_at { get; set; }
    }
}