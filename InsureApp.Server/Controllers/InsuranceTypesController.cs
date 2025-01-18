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
    public class InsuranceTypesController : ControllerBase
    {
        private readonly InsuranceDbContext _context;

        public InsuranceTypesController(InsuranceDbContext context)
        {
            _context = context;
        }

        // GET: api/InsuranceTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InsuranceType>>> GetInsuranceTypes()
        {
            return await _context.InsuranceTypes.ToListAsync();
        }

        // GET: api/InsuranceTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InsuranceType>> GetInsuranceType(int id)
        {
            var insuranceType = await _context.InsuranceTypes.FindAsync(id);

            if (insuranceType == null)
            {
                return NotFound();
            }

            return insuranceType;
        }

        // PUT: api/InsuranceTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInsuranceType(int id, InsuranceType insuranceType)
        {
            if (id != insuranceType.Id)
            {
                return BadRequest();
            }

            _context.Entry(insuranceType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InsuranceTypeExists(id))
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

        // POST: api/InsuranceTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<InsuranceType>> PostInsuranceType(InsuranceType insuranceType)
        {
            _context.InsuranceTypes.Add(insuranceType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInsuranceType", new { id = insuranceType.Id }, insuranceType);
        }

        // DELETE: api/InsuranceTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInsuranceType(int id)
        {
            var insuranceType = await _context.InsuranceTypes.FindAsync(id);
            if (insuranceType == null)
            {
                return NotFound();
            }

            _context.InsuranceTypes.Remove(insuranceType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InsuranceTypeExists(int id)
        {
            return _context.InsuranceTypes.Any(e => e.Id == id);
        }
    }
}
