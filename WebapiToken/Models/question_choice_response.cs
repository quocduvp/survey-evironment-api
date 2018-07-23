namespace WebapiToken.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class question_choice_response
    {
        public int id { get; set; }

        public int? question_id { get; set; }

        public int? question_choice_id { get; set; }

        public DateTime? create_at { get; set; }

        public int? accounts_id { get; set; }

        public int? surveys_response_id { get; set; }

        public virtual surveys_response surveys_response { get; set; }
    }
}
