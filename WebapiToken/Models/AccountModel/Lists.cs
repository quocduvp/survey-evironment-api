using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebapiToken.Models.AccountModel
{
    public class Lists
    {
        public int id { get; set; }

        public string card_id { get; set; }

        public string username { get; set; }

        public string section { get; set; }

        public bool status { get; set; }

        public DateTime date_join { get; set; }

        public int? role_id { get; set; }

        public DateTime? create_at { get; set; }

        public virtual RoleAccount role { get; set; }
    }
}