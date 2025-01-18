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
    public class InsuranceReportsController : ControllerBase
    {
        private readonly InsuranceDbContext _context;

        public InsuranceReportsController(InsuranceDbContext context)
        {
            _context = context;
        }

        // GET: api/InsuranceReports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InsuranceReport>>> GetInsuranceReports()
        {
            return await _context.InsuranceReports.ToListAsync();
        }

        // GET: api/InsuranceReports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InsuranceReport>> GetInsuranceReport(int id)
        {
            var insuranceReport = await _context.InsuranceReports.FindAsync(id);

            if (insuranceReport == null)
            {
                return NotFound();
            }

            return insuranceReport;
        }

        // PUT: api/InsuranceReports/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInsuranceReport(int id, InsuranceReport insuranceReport)
        {
            if (id != insuranceReport.Id)
            {
                return BadRequest();
            }

            _context.Entry(insuranceReport).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InsuranceReportExists(id))
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

        // POST: api/InsuranceReports
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<InsuranceReport>> PostInsuranceReport(InsuranceReport insuranceReport)
        {
            _context.InsuranceReports.Add(insuranceReport);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInsuranceReport", new { id = insuranceReport.Id }, insuranceReport);
        }

        // DELETE: api/InsuranceReports/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInsuranceReport(int id)
        {
            var insuranceReport = await _context.InsuranceReports.FindAsync(id);
            if (insuranceReport == null)
            {
                return NotFound();
            }

            _context.InsuranceReports.Remove(insuranceReport);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InsuranceReportExists(int id)
        {
            return _context.InsuranceReports.Any(e => e.Id == id);
        }
    }
}
