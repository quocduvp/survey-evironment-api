namespace WebapiToken.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class faq
    {
        public int id { get; set; }

        [Required]
        [StringLength(200)]
        public string title { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string body { get; set; }

        public DateTime? create_at { get; set; }
    }
}
