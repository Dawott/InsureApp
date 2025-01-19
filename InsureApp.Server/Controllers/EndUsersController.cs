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
       //private readonly ILogger<EndUsersController> _logger;

        public EndUsersController(InsuranceDbContext context)
        {
            _db = context;
            //_logger = logger;
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

        [HttpGet]
        [ActionName("GetUserID")]
        public async Task<ActionResult<EndUser>> GetUserByName([FromQuery] string firstName, [FromQuery] string lastName)
        {
            try
            {
                if(firstName == null || lastName == null)
                {
                    return BadRequest(new { message = "Proszę podać imię i nazwisko" });
                }

                var user = await _db.EndUsers.FirstOrDefaultAsync(u => u.FirstName.ToLower() == firstName.ToLower() && u.LastName.ToLower() == lastName.ToLower());

                if (user == null)
                {
                    return NotFound(new { message = $"Nie ma użytkownika o danych {firstName} {lastName}"});
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd ze strony serwera! Powód:",ex});
            }
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
