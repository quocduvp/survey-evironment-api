namespace WebapiToken.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("faculty")]
    public partial class faculty
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public faculty()
        {
            classrooms = new HashSet<classroom>();
        }

        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string faculty_code { get; set; }

        [Required]
        [StringLength(100)]
        public string faculty_name { get; set; }

        [Required]
        [StringLength(1000)]
        public string faculty_description { get; set; }

        public DateTime? create_at { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<classroom> classrooms { get; set; }
    }
}
