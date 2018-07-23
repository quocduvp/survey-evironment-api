using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebapiToken.Models.AccountModel
{
    public class AccountProfile
    {
        public int id { get; set; }

        public int? account_id { get; set; }

        public int? classroom_id { get; set; }

        public object classroom { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public string avatar { get; set; }

        public bool? gender { get; set; }

        public DateTime? birthday { get; set; }

        public string phone_number { get; set; }

        public string description { get; set; }

        public DateTime? modify_date { get; set; }
    }
}