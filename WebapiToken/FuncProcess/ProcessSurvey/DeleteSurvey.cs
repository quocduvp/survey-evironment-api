using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebapiToken.Models;

namespace WebapiToken.FuncProcess.ProcessSurvey
{
    public static class DeleteSurvey
    {
        private static DBS db = new DBS();
        public static async Task<bool> Deleted(int id)
        {
            try
            {
                var findSurvey = db.surveys.Where(a => a.id == id && a.deleted == false).FirstOrDefault();
                if(findSurvey != null)
                {
                    findSurvey.deleted = true;
                    int check = await db.SaveChangesAsync();
                    if(check > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}