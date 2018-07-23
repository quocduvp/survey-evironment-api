using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebapiToken.Models.QuestionModel
{
    public class ResponseQuestion
    {
        //type text
        public string text { get; set; }
        //type choice
        public int question_choice_id { get; set; }
    }
}