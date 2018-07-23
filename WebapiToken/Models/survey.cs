namespace WebapiToken.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class survey
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public survey()
        {
            questions = new HashSet<question>();
            surveys_response = new HashSet<surveys_response>();
        }

        public int id { get; set; }

        [Required]
        [StringLength(500)]
        public string title { get; set; }

        [Required]
        [StringLength(1000)]
        public string description { get; set; }

        public bool publish { get; set; }

        public int? surveys_type_id { get; set; }

        [StringLength(1000)]
        public string thumb { get; set; }

        [Column(TypeName = "date")]
        public DateTime date_start { get; set; }

        public bool deleted { get; set; }

        public DateTime? create_at { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<question> questions { get; set; }

        public virtual surveys_type surveys_type { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<surveys_response> surveys_response { get; set; }
    }
}
