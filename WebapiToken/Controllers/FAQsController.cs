using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebapiToken.Models;

namespace WebapiToken.Controllers
{
    public class FAQsController : ApiController
    {
        private DBS db = new DBS();

        [Authorize(Roles = "admin,student,staff")]
        [HttpGet]
        [Route("api/v1/all/faqs")]
        public IQueryable<faq> Getfaqs()
        {
            return db.faqs.OrderByDescending(a=>a.create_at);
        }

        // GET: api/FAQs/5
        [Authorize(Roles = "admin,student,staff")]
        [HttpGet]
        [Route("api/v1/all/faqs/{id}")]
        public async Task<IHttpActionResult> Getfaq(int id)
        {
            faq faq = await db.faqs.Where(a=>a.id == id).FirstOrDefaultAsync();
            if (faq != null)
            {
                return Ok(faq);
            }
            else
            {
                return BadRequest("FAQs not found.");
            }   
        }

        //update
        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("api/v1/admin/faqs/{id}")]
        public async Task<IHttpActionResult> Putfaq(int id, faq faq)
        {
            var findFAQs = await db.faqs.Where(a => a.id == id).FirstOrDefaultAsync();
            if(findFAQs != null)
            {
                findFAQs.title = faq.title;
                findFAQs.body = faq.body;
                db.Entry(findFAQs).State = EntityState.Modified;
                int check = await db.SaveChangesAsync();
                if (check > 0)
                    return Ok(await db.faqs.ToListAsync());
                else
                    return BadRequest("Update fails.");
            }
            else
            {
                return BadRequest("FAQs not found.");
            }
        }

        // POST: api/FAQs
        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("api/v1/admin/faqs")]
        public async Task<IHttpActionResult> Postfaq(faq faq)
        {

            faq.create_at = DateTime.Now;
            db.faqs.Add(faq);
            int check = await db.SaveChangesAsync();
            if (check > 0)
                return Ok(await db.faqs.ToListAsync());
            else
                return BadRequest("Create fails");
        }

        // DELETE: api/FAQs/5
        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("api/v1/admin/faqs/{id}")]
        public async Task<IHttpActionResult> Deletefaq(int id)
        {
            var findFAQs = await db.faqs.Where(a => a.id == id).FirstOrDefaultAsync();
            if (findFAQs != null)
            {
                db.Entry(findFAQs).State = EntityState.Deleted;
                int check = await db.SaveChangesAsync();
                if (check > 0)
                    return Ok(await db.faqs.ToListAsync());
                else
                    return BadRequest("Update fails.");
            }
            else
            {
                return BadRequest("FAQs not found.");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool faqExists(int id)
        {
            return db.faqs.Count(e => e.id == id) > 0;
        }
    }
}