using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InsureApp.Server.Model;
using System.Net;

namespace InsureApp.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EndUsersController : ControllerBase
    {
        private readonly InsuranceDbContext _db;

        public EndUsersController(InsuranceDbContext context)
        {
            _db = context;
        }

       
        [HttpGet]
        [ActionName("GetAllCustomers")]
        public async Task<ActionResult<IEnumerable<EndUser>>> GetEndUsers()
        {
            try
            {
                return Ok(await _db.EndUsers.ToListAsync());
            }
            catch (Exception ex)
            {
                var Exception = ex;
                while (Exception.InnerException != null)
                {
                    Exception = Exception.InnerException;
                }
                return new ObjectResult("Nie można połączyć się z bazą") { StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        }


        // GET: api/EndUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EndUser>> GetEndUser(int id)
        {
            var endUser = await _db.EndUsers.FindAsync(id);

            if (endUser == null)
            {
                return NotFound();
            }

            return endUser;
        }

        // PUT: api/EndUsers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEndUser(int id, EndUser endUser)
        {
            if (id != endUser.Id)
            {
                return BadRequest();
            }

            _db.Entry(endUser).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EndUserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/EndUsers
        [HttpPost]
        public async Task<ActionResult<EndUser>> PostEndUser(EndUser endUser)
        {
            _db.EndUsers.Add(endUser);
            await _db.SaveChangesAsync();

            return CreatedAtAction("GetEndUser", new { id = endUser.Id }, endUser);
        }

        // DELETE: api/EndUsers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEndUser(int id)
        {
            var endUser = await _db.EndUsers.FindAsync(id);
            if (endUser == null)
            {
                return NotFound();
            }

            _db.EndUsers.Remove(endUser);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private bool EndUserExists(int id)
        {
            return _db.EndUsers.Any(e => e.Id == id);
        }
    }
}
