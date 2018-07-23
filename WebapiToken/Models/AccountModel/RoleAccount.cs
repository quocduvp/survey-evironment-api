using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebapiToken.Models.AccountModel
{
    public class RoleAccount
    {
        public int id { get; set; }

        public string role_name { get; set; }

        public DateTime? create_at { get; set; }
    }
}