using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebapiToken.Models;
namespace WebapiToken
{
    static public class UserLogin
    {
        static public bool loginAdmin(string username, string password)
        {
            DBS db = new DBS();
            var hashPass = HashPassword.hashPassword(password);
            var admin = db.admins.Where(a => a.username == username && a.password ==  hashPass && a.role_id == 0)
                       .FirstOrDefault();
            if (admin == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        static public bool loginStudent(string username, string password)
        {
            DBS db = new DBS();
            var hashPass = HashPassword.hashPassword(password);
            var user = db.accounts.Where(a => a.username == username && a.password == hashPass && a.role_id == 2 && a.status == true)
                .FirstOrDefault();
            if(user == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        static public bool loginStaff(string username, string password)
        {
            DBS db = new DBS();
            var hashPass = HashPassword.hashPassword(password);
            var user = db.accounts.Where(a => a.username == username && a.password == hashPass && a.role_id == 1 && a.status == true)
                .FirstOrDefault();
            if (user == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}