using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebapiToken.FuncProcess.UploadToAzure;
using WebapiToken.Models;
using WebapiToken.FuncProcess;
using WebapiToken.FuncProcess.ProcessSurvey;
using System.Security.Claims;

namespace WebapiToken.Controllers
{
    public class UploadMediaController : ApiController
    {
        private static DBS db = new DBS();

        [Authorize(Roles = "student,staff")]
        [HttpPost, Route("api/v1/upload")]
        public async Task<IHttpActionResult> Post()
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var username = identity.Claims.Where(a => a.Type == ClaimTypes.Name).Select(c => c.Value).FirstOrDefault();
                var findUser = db.accounts.Where(a => a.username == username).FirstOrDefault();
                if (findUser != null)
                {
                    var httpRequest = HttpContext.Current.Request;
                    if (httpRequest.Files.Count > 0)
                    {
                        var docfiles = "";
                        foreach (string file in httpRequest.Files)
                        {
                            var postedFile = httpRequest.Files[file];
                            //check image file
                            if (postedFile.ContentType == "image/jpeg" || postedFile.ContentType == "image/jpg" ||
                                postedFile.ContentType == "image/png" || postedFile.ContentType == "image/gif")
                            {
                                // post to cloud service return path 
                                bool check = await UpdateAvatar(postedFile, findUser.id);
                                if (check)
                                    return Ok(await FetchDetailsAccount.GetDetailsAccount(findUser.id));
                                else
                                    return BadRequest("Update avatar fails.");
                            }
                            else
                            {
                                return BadRequest("file is not image type.");
                            }
                        }
                        return Created("Success", docfiles);
                    }
                    else
                    {
                        return BadRequest("Upload file error.");
                    }
                }
                else
                {
                    return BadRequest("Account not found.");
                }
            }
            catch
            {
                return BadRequest("Error code.");
            }
        }

        
        public static async Task<bool> UpdateAvatar(HttpPostedFile postedFile, int id)
        {
            try
            {
                var details = (from a in db.accounts
                               from b in db.details
                               where a.id == b.account_id && a.id == id select b).FirstOrDefault();
                if (details == null)
                {
                    return false;
                }
                else
                {
                    string fullPath = await ImageServices.UploadImageAsync(postedFile); //return url image
                    details.avatar = fullPath;
                    int check = await db.SaveChangesAsync();
                    if (check > 0)
                        return true;
                    else
                        return false;
                }
            }catch(Exception e)
            {
                return false;
            }
        }

        //upload thumb survey
        [Authorize(Roles = "admin")]
        [HttpPost, Route("api/v1/upload_thumb/{id}")]
        public async Task<IHttpActionResult> PostThumb(int id)
        {
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var docfiles = "";
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    //check image file
                    if (postedFile.ContentType == "image/jpeg" || postedFile.ContentType == "image/jpg" ||
                        postedFile.ContentType == "image/png" || postedFile.ContentType == "image/gif")
                    {
                        // post to cloud service return path 
                        bool check = await UpdateThumb(postedFile, id);
                        if (check)
                            return Ok(await FetchDetailsSurvey.GetDetailsSurvey(id));
                        else
                            return BadRequest("Update avatar fails.");
                    }
                    else
                    {
                        return BadRequest("file is not image type.");
                    }
                }
                return Created("Success", docfiles);
            }
            else
            {
                return BadRequest("Upload file error.");
            }
        }

        public static async Task<bool> UpdateThumb(HttpPostedFile postedFile, int id)
        {
            try
            {
                var details = (from a in db.surveys
                               from b in db.surveys_type
                               where a.surveys_type_id == b.id && a.id == id
                               select a).FirstOrDefault();
                if (details == null)
                {
                    return false;
                }
                else
                {
                    string fullPath = await ImageServices.UploadThumbAsync(postedFile); //return url image
                    details.thumb = fullPath;
                    int check = await db.SaveChangesAsync();
                    if (check > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
