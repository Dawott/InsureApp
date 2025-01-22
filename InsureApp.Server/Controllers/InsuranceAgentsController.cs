using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InsureApp.Server.Model;

namespace InsureApp.Server.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InsuranceAgentsController : ControllerBase
    {
        private readonly InsuranceDbContext _db;
        private readonly ILogger<InsuranceAgentsController> _logger;

        public InsuranceAgentsController(InsuranceDbContext context, ILogger<InsuranceAgentsController> logger)
        {
            _db = context;
            _logger = logger;
        }

        // GET: api/InsuranceAgents
        [HttpGet]
        [ActionName("GetAllAgents")]
        public async Task<ActionResult<IEnumerable<InsuranceAgent>>> GetInsuranceAgents()
        {
            try
            {
                var users = await _db.InsuranceAgents.ToListAsync();
                return Ok(new ApiResponse<IEnumerable<InsuranceAgent>>
                {
                    Success = true,
                    Data = users,
                    Message = "Zwrócono wszystkich użytkowników"

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd przy zwracaniu użytkowników");
                return StatusCode(500, new ApiResponse<IEnumerable<InsuranceAgent>>
                {
                    Success = false,
                    Message = $"Błąd serwera podczas zwracania użytkowników. Szczegóły: {ex}"
                }
                    );
            }
        }

        // GET: api/InsuranceAgents/5
        [HttpGet("{id}")]
        [ActionName("GetAgentByID")]
        public async Task<ActionResult<InsuranceAgent>> GetInsuranceAgent(int id)
        {
            var endUser = await _db.EndUsers.FindAsync(id);


            if (endUser == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Nie odnalazłem użytkownika o ID {id}"
                });
            }
            else
            {
                return Ok(new ApiResponse<EndUser>
                {
                    Success = true,
                    Message = $"Użytkownik o {id} znaleziony",
                    Data = endUser
                });
            }
        }

        // PUT: api/InsuranceAgents/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInsuranceAgent(int id, InsuranceAgent insuranceAgent)
        {
            if (id != insuranceAgent.Id)
            {
                return BadRequest();
            }

            _db.Entry(insuranceAgent).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InsuranceAgentExists(id))
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

        // POST: api/InsuranceAgents
        [HttpPost]
        public async Task<ActionResult<InsuranceAgent>> PostInsuranceAgent(InsuranceAgent insuranceAgent)
        {
            _db.InsuranceAgents.Add(insuranceAgent);
            await _db.SaveChangesAsync();

            return CreatedAtAction("GetInsuranceAgent", new { id = insuranceAgent.Id }, insuranceAgent);
        }

        // DELETE: api/InsuranceAgents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInsuranceAgent(int id)
        {
            var insuranceAgent = await _db.InsuranceAgents.FindAsync(id);
            if (insuranceAgent == null)
            {
                return NotFound();
            }

            _db.InsuranceAgents.Remove(insuranceAgent);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private bool InsuranceAgentExists(int id)
        {
            return _db.InsuranceAgents.Any(e => e.Id == id);
        }
    }
}
