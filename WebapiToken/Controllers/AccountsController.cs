using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebapiToken.Models.AccountModel;
using WebapiToken.Models;
using System.Web.Http.Description;
using WebapiToken.FuncProcess;
using WebapiToken.Models.Panigation;
using WebapiToken.FuncProcess.ResponseMessage;
using System.Security.Claims;
using WebapiToken.FuncProcess.ProcessAccount;

namespace WebapiToken.Controllers
{
    public class AccountsController : ApiController
    {
        private DBS db = new DBS();

        //create account
        [HttpPost]
        [Route("api/v1/register")]
        public async Task<IHttpActionResult> Register([FromBody]RegisterForm form)
        {
            try
            {
                var check = db.accounts.Where(a => a.username == form.username).FirstOrDefault();
                if(check == null)
                {
                    var pa1 = HashPassword.hashPassword(form.password);
                    var pa2 = HashPassword.hashPassword(form.password2);
                    var role = 2;
                    if (form.section.ToLower() == "teacher")
                        role = 1;
                    if (pa1 == pa2)
                    {
                        var user = new account
                        {
                            card_id = form.card_id,
                            username = form.username,
                            password = pa1,
                            password2 = pa2,
                            section = form.section,
                            status = false,
                            role_id = role,
                            date_join = form.date_join,
                            create_at = DateTime.Now
                        };
                        db.accounts.Add(user);
                        int val = await db.SaveChangesAsync();
                        if (val > 0) {
                            var msg = new ResponseMsg
                            {
                                message = "Register success. please wait check from admin."
                            };
                            return Created("ok", msg);
                        }
                    else
                        return BadRequest("Register fails.");
                    }
                    else
                    {
                        return BadRequest("Passwords are not the same.");
                    }
                }
                else
                {
                    return BadRequest("username have changes.");
                }
                
            }
            catch (Exception e)
            {
                string message = e.Message;
                return BadRequest(message);
            }
        }

        //Reset pass user by admin
        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("api/v1/admin/reset_password/{id}")]
        public async Task<IHttpActionResult> ResetPassword(int id, [FromBody]ResetAccount form)
        {
            try
            {
                var account = (from a in db.accounts where a.id == id select a).FirstOrDefault();
                if(account != null)
                {
                    //check same password
                    var pass1 = HashPassword.hashPassword(form.password);
                    var pass2 = HashPassword.hashPassword(form.pre_password);
                    if (pass1 == pass2)
                    {
                        account.password = pass1;
                        account.password2 = pass2;
                        int val = await db.SaveChangesAsync();
                        //search  from list report
                        var findListReport = db.report_account.Where(a => a.account_id == account.id).FirstOrDefault();
                        if(findListReport!= null)
                        {
                            db.Entry(findListReport).State = System.Data.Entity.EntityState.Deleted;
                            await db.SaveChangesAsync();
                        }
                        if (val > 0)
                            return Ok(await FetchDetailsAccount.GetDetailsAccount(id));
                        else
                            return BadRequest("Reset password error.");
                       
                    }
                    else
                    {
                        return BadRequest("Passwords are not the same.");
                    }
                }
                else
                {
                    return BadRequest("Not found accounts.");
                }  
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }

        //reset password admin
        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("api/v1/admin/password_admin")]
        public async Task<IHttpActionResult> ResetPasswordAdmin([FromBody]admin form)
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var username = identity.Claims.Where(a => a.Type == ClaimTypes.Name).Select(c => c.Value).FirstOrDefault();
                var admin = (from a in db.admins where a.username == username select a).FirstOrDefault();
                if (admin != null)
                {
                    admin.password = HashPassword.hashPassword(form.password);
                    int val = await db.SaveChangesAsync();
                    if (val > 0)
                       return Ok(await FetchDetailsAdmin.GetDetailsAccount(admin.id));
                    else
                       return BadRequest("Reset password error.");
                }
                else
                {
                    return BadRequest("Not found accounts.");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        //profile admin
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/v1/admin/profile")]
        public async Task<IHttpActionResult> GetAdminInfo()
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var username = identity.Claims.Where(a => a.Type == ClaimTypes.Name).Select(c => c.Value).FirstOrDefault();
                //find id account vs username
                var find = db.admins.Where(e => e.username == username).Select(e => e).FirstOrDefault();
                if (find != null)
                {
                    return Ok(await FetchDetailsAdmin.GetDetailsAccount(find.id));
                }
                else
                {
                    return BadRequest("Not found account.");
                }
            }
            catch (Exception e)
            {
                return BadRequest("Error fetch my profile.");
            }
        }

        //change status if true allow account login to system
        [Authorize(Roles ="admin")]
        [HttpGet]
        [Route("api/v1/admin/accounts/status/{id}")]
        public async Task<IHttpActionResult> ChangeStatus(int id)
        {
            try
            {
                var account = db.accounts.Where(acc => acc.id == id).FirstOrDefault();
                if (account != null)
                {
                    account.status = !account.status;
                    int val = await db.SaveChangesAsync();
                    if (val > 0)
                        //refesh account
                        return Ok(await FetchDetailsAccount.GetDetailsAccount(id));
                    else
                        return BadRequest("Register fails.");
                }
                else
                {
                    return BadRequest("Not found.");
                }
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //get details
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/v1/admin/accounts/{id}")]
        public async Task<IHttpActionResult> GetDetailsAccount(int id)
        {
            try
            {
                var account = await FetchDetailsAccount.GetDetailsAccount(id);
                if (account != null)
                    return Ok(account);
                else
                    return BadRequest("Dont find account.");
                //refesh account

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }

        //get all account
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/v1/admin/accounts")]
        public async Task<IHttpActionResult> GetAccounts([FromUri]Page form)
        {
            return Ok(await FetchListAccounts.getAllAccount(form._page,form._page_size));
        }

        //get all account
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/v1/admin/accounts_verify")]
        public async Task<IHttpActionResult> GetAccountsDontVerify([FromUri]Page form)
        {
            return Ok(await FetchListAccountNoVerify.getAllAccount(form._page, form._page_size));
        }

        //get  all  account for student
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/v1/admin/student_accounts")]
        public async Task<IHttpActionResult> GetAccountsForStudent(int page, int page_size, [FromUri]Page form)
        {
            return Ok(await FetchListAccountForStudent.getAllAccount(form.page, form.page_size));
        }

        //get  all  account for student
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/v1/admin/staff_accounts")]
        public async Task<IHttpActionResult> GetAccountsForStaff(int page, int page_size, [FromUri]Page form)
        {
            return Ok(await FetchListAccountForStaff.getAllAccount(form.page, form.page_size));
        }

        //delete account
        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("api/v1/admin/accounts/{id}")]
        public async Task<IHttpActionResult> DeteletAccount(int id)
        {
            bool check = await DeleteAccount.FuncDeleteAccount(id);
            ResponseMsg messages = new ResponseMsg();
            if (check)
            {
                messages.message = "Delete account successful.";
                return Ok(messages);
            }
            else
                return BadRequest("Error deleted account.");
        }

        //get list report
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/v1/admin/list_report")]
        public async Task<IHttpActionResult> GetListReport()
        {
            var list = db.report_account.Select(a=> new {
                id = a.id,
                account_id = a.account_id,
                username = a.username,
                create_at = a.create_at
            }).ToList();
            return Ok(list);
        }

        //get list report
        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("api/v1/admin/report_account/{id}")]
        public async Task<IHttpActionResult> DeleteReport(int id)
        {
            var account = db.report_account.Where(a=>a.account_id == id).FirstOrDefault();
            if(account != null)
            {
                db.report_account.Remove(account);
                var check = await db.SaveChangesAsync();
                if(check > 0)
                {
                    var smg = new ResponseMsg();
                    smg.message = "Remove report account success";
                    return Ok(smg);
                }
                else
                {
                    return BadRequest("Dont remove report.");
                }
                
            }
            else
            {
                return BadRequest("Account not found.");
            }
        }

        /// <summary>
        /// Account User api 
        /// </summary>
        /// <returns></returns>
        //get list account  user
        [Authorize(Roles = "student,staff")]
        [HttpGet]
        [Route("api/v1/user/profile")]
        public async Task<IHttpActionResult> GetAccountInfo()
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var username = identity.Claims.Where(a => a.Type == ClaimTypes.Name).Select(c => c.Value).FirstOrDefault();
                //find id account vs username
                var find = db.accounts.Where(e => e.username == username).Select(e => e.id).FirstOrDefault();
                if (find != null)
                {
                    return Ok(await FetchDetailsAccount.GetDetailsAccount(find));
                }
                else
                {
                    return BadRequest("Not found account.");
                }
            }catch(Exception e)
            {
                return BadRequest("Error fetch my profile.");
            }
        }

        [Authorize(Roles = "student,staff")]
        [HttpPut]
        [Route("api/v1/user/profile")]
        public async Task<IHttpActionResult> CreateAccountInfo([FromBody]detail form)
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var username = identity.Claims.Where(a => a.Type == ClaimTypes.Name).Select(c => c.Value).FirstOrDefault();
                var findIDAcc = db.accounts.Where(a => a.username == username).FirstOrDefault();
                if(findIDAcc != null)
                {
                    var details = db.details.Where(a => a.account_id == findIDAcc.id).FirstOrDefault();
                    if(details != null)
                    {
                        details.classroom_id = form.classroom_id;
                        details.birthday = form.birthday;
                        details.first_name = form.first_name;
                        details.last_name = form.last_name;
                        details.phone_number = form.phone_number;
                        details.description = form.description;
                        details.gender = form.gender;
                        details.modify_date = DateTime.Now;
                        int check = await db.SaveChangesAsync();
                        if(check > 0)
                        {
                            return Ok(await FetchDetailsAccount.GetDetailsAccount(findIDAcc.id));
                        }
                        else
                        {
                            return BadRequest("Update profile error.");
                        }
                    }
                    else
                    {
                        return BadRequest("Dont find details.");
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            catch
            {
                return BadRequest("Error request.");
            }
        }
        //user report account
        [HttpPost]
        [Route("api/v1/user/report_account")]
        public async Task<IHttpActionResult> ReportAccount([FromBody]ReportAccount form)
        {
            try
            {
                    //find id account vs username
                    var findAccount = db.accounts.Where(a => a.username == form.username).FirstOrDefault();
                    if (findAccount != null)
                    {
                        if (db.report_account.Where(a => a.account_id == findAccount.id).FirstOrDefault() == null)
                        {
                            var report = new report_account
                            {
                                account_id = findAccount.id,
                                username = findAccount.username,
                                create_at = DateTime.Now
                            };
                            db.report_account.Add(report);
                            int check = await db.SaveChangesAsync();
                            if (check > 0)
                            {
                                var smg = new ResponseMsg();
                                smg.message = "Report successful.";
                                return Ok(smg);
                            }
                            else
                            {
                                return BadRequest("Report account fails.");
                            }
                        }
                        else
                        {
                            return BadRequest("Account have been reported.");
                        }
                    }
                    else
                    {
                        return BadRequest("Dont find account");
                    }
                
            }
            catch
            {
                return BadRequest("Error.");
            }
        }

        [Authorize(Roles = "student,staff")]
        [HttpPost]
        [Route("api/v1/user/reset_account")]
        public async Task<IHttpActionResult> ResetPasswordUser([FromBody]account form)
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var username = identity.Claims.Where(a => a.Type == ClaimTypes.Name).Select(c => c.Value).FirstOrDefault();
                var account = (from a in db.accounts where a.username == username  select a).FirstOrDefault();
                if (account != null)
                {
                    //check same password
                    var pass1 = HashPassword.hashPassword(form.password);
                    var pass2 = HashPassword.hashPassword(form.password2);
                    if (pass1 == pass2)
                    {
                        account.password = pass1;
                        account.password2 = pass2;
                        int val = await db.SaveChangesAsync();
                        if (val > 0)
                            return Ok(await FetchDetailsAccount.GetDetailsAccount(account.id));
                        else
                            return BadRequest("Reset password error.");

                    }
                    else
                    {
                        return BadRequest("Passwords are not the same.");
                    }
                }
                else
                {
                    return BadRequest("Not found accounts.");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

    }
}
