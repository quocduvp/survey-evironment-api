using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebapiToken.Models.AccountModel
{
    public class RegisterForm
    {
        public string card_id { get; set; }

        public string username { get; set; }

        public string password { get; set; }

        public string password2 { get; set; }

        public string section { get; set; }

        public DateTime date_join { get; set; }

        public int? role_id { get; set; }

    }
}