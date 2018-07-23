using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace WebapiToken.Controllers
{
    public class ValuesController : ApiController
    {
        [HttpGet]
        [Route("api/values")]
        public IHttpActionResult Get()
        {
            return Ok("Hello world");
        }
        [Authorize]
        [HttpGet]
        [Route("api/values/autho")]
        public IHttpActionResult GetAutho()
        {
            var identity = (ClaimsIdentity)User.Identity;
            return Ok("Hello world authentication "+ identity.Name);
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("api/values/admin")]
        public IHttpActionResult GetAdmin()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var roles = identity.Claims.Where(a => a.Type == ClaimTypes.Role).Select(c => c.Value);
            return Ok("Hello world authentication " + identity.Name + " Role " + string.Join(",",roles.ToList()));
        }
    }
}
