namespace WebapiToken.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class question_choice
    {
        public int id { get; set; }

        public int? question_id { get; set; }

        [Required]
        [StringLength(500)]
        public string description { get; set; }

        [Column("checked")]
        public bool _checked { get; set; }

        public DateTime? create_at { get; set; }

        public virtual question question { get; set; }
    }
}
