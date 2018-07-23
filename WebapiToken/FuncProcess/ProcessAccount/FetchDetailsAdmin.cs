using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebapiToken.Models;
using WebapiToken.Models.AccountModel;

namespace WebapiToken.FuncProcess.ProcessAccount
{
    public static class FetchDetailsAdmin
    {
        private static DBS db = new DBS();
        //get details for user
        public static async Task<AccountDetails> GetDetailsAccount(int id)
        {
            try
            {
                var find = (from a in db.admins
                            from b in db.details_admin
                            where a.id == b.id && a.id == id
                            select a).FirstOrDefault();
                if (find != null)
                    return await WithProfile(id);
                else
                {
                    var details = new details_admin
                    {
                        admin_id = id,
                        first_name = "",
                        last_name = "",
                        gender = null,
                        birthday = null,
                        description = "",
                        avatar = "",
                        phone_number = "",
                        modify_date = DateTime.Now
                    };
                    db.details_admin.Add(details);
                    int check = await db.SaveChangesAsync();
                    if (check > 0)
                        return await WithProfile(id);
                    else
                        return await WithOutProfile(id);
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        // if profile != null
        private static async Task<AccountDetails> WithProfile(int id)
        {
            var details = (from a in db.admins
                           from b in db.roles
                           from c in db.details_admin
                           where a.role_id == b.id && a.id == id && a.id == c.admin_id
                           select new AccountDetails
                           {
                               id = a.id,
                               role_id = a.role_id,
                               username = a.username,
                               create_at = a.create_at,
                               userinfo = new AccountProfile
                               {
                                   id = c.id,
                                   account_id = c.admin_id,
                                   first_name = c.first_name,
                                   last_name = c.last_name,
                                   phone_number = c.phone_number,
                                   gender = c.gender,
                                   avatar = c.avatar,
                                   birthday = c.birthday,
                                   description = c.description,
                                   modify_date = c.modify_date
                               },
                               role = new RoleAccount
                               {
                                   id = b.id,
                                   role_name = b.role_name,
                                   create_at = b.create_at
                               }
                           }).FirstOrDefault();
            return details;
        }

        //if profile == null
        private static async Task<AccountDetails> WithOutProfile(int id)
        {
            var details = (from a in db.admins
                           from b in db.roles
                           where a.role_id == b.id && a.id == id
                           select new AccountDetails
                           {
                               id = a.id,
                               role_id = a.role_id,
                               username = a.username,
                               create_at = a.create_at,
                               role = new RoleAccount
                               {
                                   id = b.id,
                                   role_name = b.role_name,
                                   create_at = b.create_at
                               }
                           }).FirstOrDefault();
            return details;
        }
    }
}