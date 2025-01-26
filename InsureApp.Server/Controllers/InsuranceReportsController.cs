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
    public class InsuranceReportsController : ControllerBase
    {
        private readonly InsuranceDbContext _context;
        private readonly ILogger<InsuranceReportsController> _logger;

        public InsuranceReportsController(InsuranceDbContext context, ILogger<InsuranceReportsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/InsuranceReports
        [HttpGet]
        [ActionName("GetAllReports")]
        public async Task<ActionResult<ApiResponse<IEnumerable<InsuranceReport>>>> GetInsuranceReports()
        {
            try
            {
                var reports = await _context.InsuranceReports
                    .Include(r => r.InsuranceType)
                    .Include(r => r.EndUser)
                    .Include(r => r.InsuranceAgent)
                    .ToListAsync();

                return Ok(new ApiResponse<IEnumerable<InsuranceReport>>
                {
                    Success = true,
                    Data = reports,
                    Message = "Zwrócono raporty"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd GET");
                return StatusCode(500, new ApiResponse<IEnumerable<InsuranceReport>>
                {
                    Success = false,
                    Message = "Błąd odczytu raportów"
                });
            }
        }

        [HttpGet("{id}")]
        [ActionName("GetReportByID")]
        public async Task<ActionResult<ApiResponse<InsuranceReport>>> GetInsuranceReport(int id)
        {
            try
            {
                var report = await _context.InsuranceReports
                    .Include(r => r.InsuranceType)
                    .Include(r => r.EndUser)
                    .Include(r => r.InsuranceAgent)
                    .Include(r => r.Documents)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (report == null)
                {
                    return NotFound(new ApiResponse<InsuranceReport>
                    {
                        Success = false,
                        Message = $"Raport o ID {id} nie został znaleziony"
                    });
                }

                return Ok(new ApiResponse<InsuranceReport>
                {
                    Success = true,
                    Data = report,
                    Message = "Raport zwrócono poprawnie"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas zwracania raportu o ID {id}");
                return StatusCode(500, new ApiResponse<InsuranceReport>
                {
                    Success = false,
                    Message = "Błąd podczas zwracania pojedynczego raportu"
                });
            }
        }

        [HttpPost]
        [ActionName("CreateReport")]
        public async Task<ActionResult<ApiResponse<InsuranceReport>>> PostInsuranceReport([FromBody] InsuranceReport report)
        {
            try
            {
                var endUser = await _context.EndUsers.FindAsync(report.EndUserId);
                if (endUser == null)
                {
                    return BadRequest(new ApiResponse<InsuranceReport>
                    {
                        Success = false,
                        Message = "Błędny EndUserID"
                    });
                }
                /*
                if (!await _context.EndUsers.AnyAsync(u => u.Id == report.EndUserId))
                {
                    return BadRequest(new ApiResponse<InsuranceReport>
                    {
                        Success = false,
                        Message = "Błędne ID klienta"
                    });
                }*/

                var insuranceType = await _context.InsuranceTypes.FindAsync(report.InsuranceTypeId);
                if (insuranceType == null)
                {
                    return BadRequest(new ApiResponse<InsuranceReport>
                    {
                        Success = false,
                        Message = "Błędny Typ ID"
                    });
                }

                /*if (!await _context.InsuranceTypes.AnyAsync(t => t.Id == report.InsuranceTypeId))
                {
                    return BadRequest(new ApiResponse<InsuranceReport>
                    {
                        Success = false,
                        Message = "Błędny typ ubezpieczenia"
                    });
                }*/
                report.EndUser = endUser;
                report.InsuranceType = insuranceType;
                report.SubmissionDate = DateTime.UtcNow;
                report.Status = "Nowy";

                _context.InsuranceReports.Add(report);
                await _context.SaveChangesAsync();

                return CreatedAtAction("CreateReport",
                    new { id = report.Id },
                    new ApiResponse<InsuranceReport>
                    {
                        Success = true,
                        Data = report,
                        Message = "Utworzono raport!"
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd przy tworzeniu raport");
                return StatusCode(500, new ApiResponse<InsuranceReport>
                {
                    Success = false,
                    Message = "Błąd przy tworzeniu raportu"
                });
            }
        }

        [HttpPut("{id}")]
        [ActionName("EditReport")]
        public async Task<ActionResult<ApiResponse<InsuranceReport>>> PutInsuranceReport(int id, InsuranceReport report)
        {
            if (id != report.Id)
                return BadRequest(new ApiResponse<InsuranceReport>
                {
                    Success = false,
                    Message = "Błędne ID"
                });

            try
            {
                var existingReport = await _context.InsuranceReports.FindAsync(id);
                if (existingReport == null)
                    return NotFound(new ApiResponse<InsuranceReport>
                    {
                        Success = false,
                        Message = "Nie znaleziono raportu"
                    });

                // Preserve original submission date and end user
                report.SubmissionDate = existingReport.SubmissionDate;
                report.EndUserId = existingReport.EndUserId;

                _context.Entry(existingReport).CurrentValues.SetValues(report);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<InsuranceReport>
                {
                    Success = true,
                    Data = report,
                    Message = "Edycja raportu zakończona sukcesem"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd przy edycji raportu {id}");
                return StatusCode(500, new ApiResponse<InsuranceReport>
                {
                    Success = false,
                    Message = "Błąd przy edycji raportu"
                });
            }
        }

        [HttpDelete("{id}")]
        [ActionName("DeleteReport")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteInsuranceReport(int id)
        {
            try
            {
                var report = await _context.InsuranceReports
                    .Include(r => r.Documents)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (report == null)
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Raport nieodnaleziony"
                    });

                _context.InsuranceReports.Remove(report);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Raport skasowano"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd przy usunięciu raportu {id}");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Błąd przy usuwaniu raportu"
                });
            }
        }
    }
}
