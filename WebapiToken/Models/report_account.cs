namespace WebapiToken.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class report_account
    {
        public int id { get; set; }

        public int? account_id { get; set; }

        [Required]
        [StringLength(50)]
        public string username { get; set; }

        public DateTime? create_at { get; set; }

        public virtual account account { get; set; }
    }
}
