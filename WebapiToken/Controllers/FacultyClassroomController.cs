using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebapiToken.Models;

namespace WebapiToken.Controllers
{
    public class FacultyClassroomController : ApiController
    {
        private DBS db = new DBS();
        //create new faculty
        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("api/v1/admin/faculty")]
        public async Task<IHttpActionResult> CreateFaculty([FromBody]faculty form)
        {
            try
            {

                    form.create_at = DateTime.Now;
                    db.Entry(form).State = System.Data.Entity.EntityState.Added;
                    int check = await db.SaveChangesAsync();
                    if (check > 0)
                    return Ok(await ListFaculty());
                else
                        return BadRequest("Create faculty error");
            }
            catch
            {
                return BadRequest("Error");
            }
        }

        //Update faculty
        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("api/v1/admin/faculty/{id}")]
        public async Task<IHttpActionResult> UpdateFaculty(int id, [FromBody]faculty form)
        {
            try
            {
                var findFac = db.faculties.Where(a=>a.id == id).FirstOrDefault();
                if (findFac != null)
                {
                    findFac.faculty_name = form.faculty_name;
                    findFac.faculty_code = form.faculty_code;
                    findFac.faculty_description = form.faculty_description;
                    db.Entry(findFac).State = System.Data.Entity.EntityState.Modified;
                    int check = await db.SaveChangesAsync();
                    if (check > 0)
                        return Ok(await ListFaculty());
                    else
                        return BadRequest("Update faculty error");
                }
                else
                {
                    return BadRequest("Faculty not found.");
                }
            }
            catch
            {
                return BadRequest("Error");
            }
        }

        //Delete faculty
        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("api/v1/admin/faculty/{id}")]
        public async Task<IHttpActionResult> DeleteFaculty(int id)
        {
            try
            {
                    var findClassroom = db.classrooms.Where(a => a.faculty_id == id).ToList();
                    if (findClassroom.Count() > 0)
                    {
                        findClassroom.ForEach(a => a.faculty_id = null);
                        db.Entry(findClassroom).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                    var findFaculty = db.faculties.Where(a => a.id == id).FirstOrDefault();
                if (findFaculty != null)
                {
                    db.Entry(findFaculty).State = System.Data.Entity.EntityState.Deleted;
                    int check = await db.SaveChangesAsync();
                    if (check > 0)
                        return Ok(await ListFaculty());
                    else
                        return BadRequest("Delete fails.");
                }
                else
                {
                    return BadRequest("Faculty not found.");
                }
            }
            catch
            {
                return BadRequest("Error");
            }
        }

        //Get all faculty
        [Authorize(Roles = "admin,student,staff")]
        [HttpGet]
        [Route("api/v1/all/faculty")]
        public async Task<IHttpActionResult> GetAllFaculty()
        {
            try
            {
                return Ok(await ListFaculty());
            }
            catch
            {
                return BadRequest("Error.");
            }
        }

        /// <summary>
        /// Class room
        /// </summary>
        /// <returns></returns>
        /// Create new class
        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("api/v1/admin/classroom")]
        public async Task<IHttpActionResult> CreateClass([FromBody]classroom form)
        {
            try
            {
                form.create_at = DateTime.Now;
                db.Entry(form).State = System.Data.Entity.EntityState.Added;
                int check = await db.SaveChangesAsync();
                if (check > 0)
                    return Ok(await ListClassroom());
                else
                    return BadRequest("Create classroom fails.");
            }
            catch
            {
                return BadRequest("Error.");
            }
        }

        //update class
        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("api/v1/admin/classroom/{id}")]
        public async Task<IHttpActionResult> UpdateClass(int id,[FromBody]classroom form)
        {
            try
            {
                var findClass = db.classrooms.Where(a => a.id == id).FirstOrDefault();
                if (findClass != null)
                {
                    findClass.class_code = form.class_code;
                    findClass.faculty_id = form.faculty_id;
                    db.Entry(findClass).State = System.Data.Entity.EntityState.Modified;
                    int check = await db.SaveChangesAsync();
                    if (check > 0)
                        return Ok(await ListClassroom());
                    else
                        return BadRequest("Create classroom fails.");
                }
                else
                {
                    return BadRequest("Classroom not found.");
                }
            }
            catch
            {
                return BadRequest("Error.");
            }
        }

        //delete class
        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("api/v1/admin/classroom/{id}")]
        public async Task<IHttpActionResult> DeleteClass(int id, [FromBody]classroom form)
        {
            try
            {
                var findDetail = db.details.Where(a => a.classroom_id == id).ToList();
                if (findDetail.Count() > 0)
                {
                    findDetail.ForEach(a => a.classroom_id = null);
                    db.Entry(findDetail).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                var findClass = db.classrooms.Where(a => a.id == id).FirstOrDefault();
                if (findClass != null)
                {
                    db.Entry(findClass).State = System.Data.Entity.EntityState.Deleted;
                    int check = await db.SaveChangesAsync();
                    if (check > 0)
                        return Ok(await ListClassroom());
                    else
                        return BadRequest("Create classroom fails.");
                }
                else
                {
                    return BadRequest("Classroom not found.");
                }
            }
            catch
            {
                return BadRequest("Error.");
            }
        }

        //Get all class
        [Authorize(Roles = "admin,student,staff")]
        [HttpGet]
        [Route("api/v1/all/classroom")]
        public async Task<IHttpActionResult> GetAllClass([FromBody]classroom form)
        {
            try
            {
                return Ok(await ListClassroom());
            }
            catch
            {
                return BadRequest("Error.");
            }
        }
        //list faculty
        public async Task<object> ListFaculty()
        {
            var fac = db.faculties.Select(a => new
            {
                id = a.id,
                faculty_code = a.faculty_code,
                faculty_name = a.faculty_name,
                faculty_description = a.faculty_description,
                create_at = a.create_at
            }).ToList();
            return fac;
        }
        //list class 
        public async Task<object> ListClassroom()
        {
            var list = db.classrooms.Select(a=> new {
                id = a.id,
                class_code = a.class_code,
                faculty_id = a.faculty_id,
                faculty = db.faculties.Where(b=>b.id == a.faculty_id)
                    .Select(b =>new{
                        id = b.id,
                        faculty_code = b.faculty_code,
                        faculty_name = b.faculty_name,
                        faculty_description = b.faculty_description,
                        create_at = b.create_at
                    }).FirstOrDefault(),
                create_at = a.create_at
            }).ToList();
            return list;
        }
    }
}
