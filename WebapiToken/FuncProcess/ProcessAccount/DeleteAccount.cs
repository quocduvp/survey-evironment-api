using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebapiToken.Models;

namespace WebapiToken.FuncProcess
{
    public static class DeleteAccount
    {
        private static DBS db = new DBS();
        public static async Task<bool> FuncDeleteAccount(int id)
        {
            try
            {
                var account = db.accounts.Where(a => a.id == id).FirstOrDefault();
                if (account != null)
                {
                    //check details id == account id
                    var details = db.details.Where(a => a.account_id == account.id).FirstOrDefault();
                    if(details != null)
                    {
                        db.details.Remove(details);
                        await db.SaveChangesAsync();
                    }
                    db.accounts.Remove(account);
                    int check = await db.SaveChangesAsync();
                    if (check > 0)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }catch
            {
                return false;
            }
        }
    }
}