namespace WebapiToken.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class question_text
    {
        public int id { get; set; }

        public int? question_id { get; set; }

        [StringLength(1000)]
        public string text { get; set; }

        public DateTime? create_at { get; set; }

        public virtual question question { get; set; }
    }
}
