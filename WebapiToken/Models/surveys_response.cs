namespace WebapiToken.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class surveys_response
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public surveys_response()
        {
            question_choice_response = new HashSet<question_choice_response>();
            question_text_response = new HashSet<question_text_response>();
        }

        public int id { get; set; }

        public int? surveys_id { get; set; }

        public int? accounts_id { get; set; }

        [StringLength(50)]
        public string username { get; set; }

        public DateTime? create_at { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<question_choice_response> question_choice_response { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<question_text_response> question_text_response { get; set; }

        public virtual survey survey { get; set; }
    }
}
