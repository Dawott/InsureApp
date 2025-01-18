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
    [Route("api/[controller]")]
    [ApiController]
    public class InsuranceAgentsController : ControllerBase
    {
        private readonly InsuranceDbContext _context;

        public InsuranceAgentsController(InsuranceDbContext context)
        {
            _context = context;
        }

        // GET: api/InsuranceAgents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InsuranceAgent>>> GetInsuranceAgents()
        {
            return await _context.InsuranceAgents.ToListAsync();
        }

        // GET: api/InsuranceAgents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InsuranceAgent>> GetInsuranceAgent(int id)
        {
            var insuranceAgent = await _context.InsuranceAgents.FindAsync(id);

            if (insuranceAgent == null)
            {
                return NotFound();
            }

            return insuranceAgent;
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

            _context.Entry(insuranceAgent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<InsuranceAgent>> PostInsuranceAgent(InsuranceAgent insuranceAgent)
        {
            _context.InsuranceAgents.Add(insuranceAgent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInsuranceAgent", new { id = insuranceAgent.Id }, insuranceAgent);
        }

        // DELETE: api/InsuranceAgents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInsuranceAgent(int id)
        {
            var insuranceAgent = await _context.InsuranceAgents.FindAsync(id);
            if (insuranceAgent == null)
            {
                return NotFound();
            }

            _context.InsuranceAgents.Remove(insuranceAgent);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InsuranceAgentExists(int id)
        {
            return _context.InsuranceAgents.Any(e => e.Id == id);
        }
    }
}
