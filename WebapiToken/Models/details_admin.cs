namespace WebapiToken.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class details_admin
    {
        public int id { get; set; }

        public int? admin_id { get; set; }

        [StringLength(50)]
        public string first_name { get; set; }

        [StringLength(50)]
        public string last_name { get; set; }

        [StringLength(1000)]
        public string avatar { get; set; }

        public bool? gender { get; set; }

        [Column(TypeName = "date")]
        public DateTime? birthday { get; set; }

        [StringLength(15)]
        public string phone_number { get; set; }

        [StringLength(1000)]
        public string description { get; set; }

        public DateTime? modify_date { get; set; }

        public virtual admin admin { get; set; }
    }
}
