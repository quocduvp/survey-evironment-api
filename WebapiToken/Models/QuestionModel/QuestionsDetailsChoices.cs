﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebapiToken.Models.QuestionModel
{
    public class QuestionsDetailsChoices
    {
        public int id { get; set; }

        public int? surveys_id { get; set; }

        public string text { get; set; }

        public List<AnswerChoice> answers { get; set; }

        public int priority { get; set; }

        public DateTime? create_at { get; set; }
    }
}