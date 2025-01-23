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
    public class InsuranceTypesController : ControllerBase
    {
        private readonly InsuranceDbContext _context;
        private readonly ILogger<InsuranceTypesController> _logger;

        public InsuranceTypesController(InsuranceDbContext context, ILogger<InsuranceTypesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/InsuranceTypes
        [HttpGet]
        [ActionName("ShowAllTypes")]
        public async Task<ActionResult<ApiResponse<IEnumerable<InsuranceType>>>> GetInsuranceTypes()
        {
            try
            {
                var types = await _context.InsuranceTypes.ToListAsync();

                return Ok(new ApiResponse<IEnumerable<InsuranceType>>
                {
                    Data = types,
                    Message = "Zwrócono typy",
                    Success = true
                });
            }

            catch (Exception ex) {
                _logger.LogError(ex, "Błąd zwracania typów");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message

                });
            }
        }

        // GET: api/InsuranceTypes/5
        [HttpGet("{id}")]
        [ActionName("GetTypeByID")]
        public async Task<ActionResult<ApiResponse<InsuranceType>>> GetInsuranceType(int id)
        {
            try
            {
                var insuranceType = await _context.InsuranceTypes.FindAsync(id);

                if (insuranceType == null)
                {
                    return NotFound(new ApiResponse<InsuranceType>
                    {
                        Success = false,
                        Message = "Nie ma typu ubezpieczenia o takim ID"
                    });
                }

                return Ok(new ApiResponse<InsuranceType>
                {
                    Success = true,
                    Data = insuranceType,
                    Message = $"Zwrócono {insuranceType}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd w zwróceniu typu z ID {id}");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        // PUT: api/InsuranceTypes/5
        [HttpPut("{id}")]
        [ActionName("EditType")]
        public async Task<ActionResult<ApiResponse<InsuranceType>>> PutInsuranceType(int id, InsuranceType insuranceType)
        {
            try
            {
                if (id != insuranceType.Id)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Brak odpowiedniego ID"
                    });
                }

                //Sprawdzanie duplikatu
                if (await _context.InsuranceTypes.AnyAsync(x =>
                x.Id != id &&
                x.Name.ToLower() == insuranceType.Name.ToLower()))
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Typ o nazwie {insuranceType.Name} już istnieje!"
                    });
                }

                _context.Entry(insuranceType).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                return Ok(new ApiResponse<InsuranceType>
                {
                    Data = insuranceType,
                    Success = true,
                    Message = "Zwracam"
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InsuranceTypeExists(id))
                {
                    
                    return NotFound(new ApiResponse<InsuranceType>
                    {
                        Success = false,
                        Message = "Nie odnaleziono"
                    });
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Wystąpił błąd podczas update'u {id}");
                return StatusCode(500, new ApiResponse<InsuranceType>
                {
                    Message = "Błąd serwera",
                    Success = false
                });
            }

        }

        // POST: api/InsuranceTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ActionName("CreateType")]
        public async Task<ActionResult<ApiResponse<InsuranceType>>> PostInsuranceType(InsuranceType insuranceType)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<InsuranceType>
                    {
                        Success = false,
                        Message = "Błędne zapytanie"
                    });
                }

                //Sprawdzanie duplikatu
                if (await _context.InsuranceTypes.AnyAsync(x =>
                x.Name.ToLower() == insuranceType.Name.ToLower()))
                {
                    return Conflict(new ApiResponse<InsuranceType>
                    {
                        Message = $"Typ o nazwie {insuranceType.Name} już istnieje!",
                        Success = false
                    });
                }
                _context.InsuranceTypes.Add(insuranceType);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetInsuranceType), new { id = insuranceType.Id }, new ApiResponse<InsuranceType>
                {
                    Data = insuranceType,
                    Success = true,
                    Message = "Zapisano poprawnie"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas zapisu typu ubezpieczenia");
                return StatusCode(500, new ApiResponse<InsuranceType>
                {
                    Message = ex.Message,
                    Success = false
                });
            }
        }

        // DELETE: api/InsuranceTypes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteInsuranceType(int id)
        {
            try
            {

                var insuranceType = await _context.InsuranceTypes.FindAsync(id);
                if (insuranceType == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Message = "Nie odnaleziono takiego typu ubezpieczenia",
                        Success = false
                    });
                }

                //Constraint na raporcie
                if (await _context.InsuranceReports.AnyAsync(report => report.InsuranceTypeId == id))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Nie można usunąć tego typu, ponieważ co najmniej jeden raport go używa - zamiast tego zdeaktywuj"
                    });
                }


                _context.InsuranceTypes.Remove(insuranceType);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Skasowano pomyślnie"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas kasowania typu {id}");
                return StatusCode(500, new ApiResponse<object>
                {
                    Message = ex.Message,
                    Success = false
                });
            }
        }

        private bool InsuranceTypeExists(int id)
        {
            return _context.InsuranceTypes.Any(e => e.Id == id);
        }
    }
}
