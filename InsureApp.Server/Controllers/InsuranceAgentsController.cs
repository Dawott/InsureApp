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
        public async Task<ActionResult<ApiResponse<IEnumerable<InsuranceAgent>>>> GetInsuranceAgents()
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
        public async Task<ActionResult<ApiResponse<InsuranceAgent>>> GetInsuranceAgent(int id)
        {
            var insuranceAgent = await _db.InsuranceAgents.FindAsync(id);


            if (insuranceAgent == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Nie odnalazłem użytkownika o ID {id}"
                });
            }
            else
            {
                return Ok(new ApiResponse<InsuranceAgent>
                {
                    Success = true,
                    Message = $"Użytkownik o {id} znaleziony",
                    Data = insuranceAgent
                });
            }
        }

        //Znajdź po mailu
        [HttpGet]
        [ActionName("GetAgentIDByMail")]
        public async Task<ActionResult<ApiResponse<InsuranceAgent>>> GetAgentByMail([FromQuery] string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest(new ApiResponse<InsuranceAgent>
                    {
                        Success = false,
                        Message = "Proszę podać poprawny email"
                    });
                }

                var user = await _db.InsuranceAgents.FirstOrDefaultAsync(u =>
                u.Email.ToLower() == email.ToLower());

                if (user == null)
                {
                    return NotFound(new ApiResponse<InsuranceAgent>
                    {
                        Success = false,
                        Message = $"Nie ma użytkownika o mailu {email}"
                    });
                }
                return Ok(new ApiResponse<InsuranceAgent>
                {
                    Success = true,
                    Message = "Użytkownik znaleziony",
                    Data = user
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas wyszukiwania użytkownika");
                return StatusCode(500, new ApiResponse<InsuranceAgent>
                {
                    Success = false,
                    Message = "Błąd ze strony serwera! Powód: " + ex
                });
            }
        }

        //Zrzutka powiązanych raportów
        [HttpGet("{id}/reports")]
        [ActionName("GetReportsForAgent")]
        public async Task<ActionResult<ApiResponse<IEnumerable<InsuranceReport>>>> GetAgentReports(int id)
        {
            try
            {
                // Sprawdzenie czy istnieje
                if (!await _db.InsuranceAgents.AnyAsync(a => a.Id == id))
                {
                    return NotFound(new ApiResponse<IEnumerable<InsuranceReport>>
                    {
                        Success = false,
                        Message = $"Agent z ID {id} nie istnieje",
                        Data = null
                    });
                }

                // Get reports with related data
                var reports = await _db.InsuranceReports
                    .Include(r => r.InsuranceType)
                    .Include(r => r.EndUser)
                    .Where(r => r.InsuranceAgentId == id)
                    .ToListAsync();

                return Ok(new ApiResponse<IEnumerable<InsuranceReport>>
                {
                    Success = true,
                    Message = $"Załadowano raporty w ilości {reports.Count} dla użytkownika {id}",
                    Data = reports
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<InsuranceReport>>
                {
                    Success = false,
                    Message = $"Pojawił się błąd podczas ładowania raportów: {ex.Message}",
                    Data = null
                });
            }
        }

        // PUT: api/InsuranceAgents/5
        [HttpPut("{id}")]
        [ActionName("EditAgent")]
        public async Task<ActionResult<ApiResponse<InsuranceAgent>>> PutInsuranceAgent(int id, InsuranceAgent insuranceAgent)
        {
            if (id != insuranceAgent.Id)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"ID {id} nie istnieje"
                });
            }

            try
            {
                //Na wszelki wypadek - lock na nazwie użytkownika
                if (await _db.InsuranceAgents.AnyAsync(u =>
                u.Id != id &&
                u.Username.ToLower() == insuranceAgent.Username.ToLower()))
                {
                    return Conflict(new ApiResponse<InsuranceAgent>
                    {
                        Success = false,
                        Message = $"Nazwa '{insuranceAgent.Username}' jest już zajęta"
                    }
                       );
                }

                //Na wszelki wypadek - lock na Emailu
                if (await _db.InsuranceAgents.AnyAsync(u =>
                u.Id != id &&
                u.Email.ToLower() == insuranceAgent.Email.ToLower()))
                {
                    return Conflict(new ApiResponse<InsuranceAgent>
                    {
                        Success = false,
                        Message = $"Email '{insuranceAgent.Email}' jest już w bazie"
                    });
                }

                _db.Entry(insuranceAgent).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return Ok(new ApiResponse<InsuranceAgent>
                {
                    Success = true,
                    Data = insuranceAgent,
                    Message = "Update zakończony sukcesem"
                });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Błąd concurrency podczas update'u usera {id}");
                if (!InsuranceAgentExists(id))
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Nie odnaleziono użytkownika"
                    });
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Błąd update'u agenta ID {id}");

                if (ex.InnerException?.Message.Contains("unique constraint", StringComparison.OrdinalIgnoreCase) ?? false)
                {
                    return Conflict(new ApiResponse<InsuranceAgent>
                    {
                        Success = false,
                        Message = "Duplikat nazwy użytkownika lub maila"
                    });
                }
                return StatusCode(500, new ApiResponse<InsuranceAgent>
                {
                    Success = false,
                    Message = "Pojawił się błąd przy dodawaniu użytkownika" + ex.Message
                });
            }
        }
        

        // POST: api/InsuranceAgents
        [HttpPost]
        [ActionName("AddAgentManually")]
        public async Task<ActionResult<ApiResponse<InsuranceAgent>>> PostInsuranceAgent(InsuranceAgent insuranceAgent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<InsuranceAgent>
                {
                    Success = false,
                    Message = "Błędne dane!"
                });
            }

            //Jeśli username istnieje
            if (await _db.InsuranceAgents.AnyAsync(u => u.Username.ToLower() == insuranceAgent.Username.ToLower()))
            {
                return Conflict(new ApiResponse<InsuranceAgent>
                {
                    Success = false,
                    Message = $"Nazwa Użytkownika jest już zajęta"
                }
                    );
            }

            //Jeśli mail już jest w bazie
            if (await _db.InsuranceAgents.AnyAsync(u => u.Email.ToLower() == insuranceAgent.Email.ToLower()))
            {
                return Conflict(new ApiResponse<InsuranceAgent>
                {
                    Success = false,
                    Message = $"Taki adres już jest w bazie. Zresetuj hasło, jeśli go nie pamiętasz albo skonsultuj się z agentem"
                }
                    );
            }

            try
            {
                _db.InsuranceAgents.Add(insuranceAgent);
                await _db.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetInsuranceAgent),
                    new { id = insuranceAgent.Id },
                    new ApiResponse<InsuranceAgent>
                    {
                        Success = true,
                        Data = insuranceAgent,
                        Message = "Utworzono użytkownika"
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w utworzeniu użytkownika");
                if (ex.InnerException?.Message.Contains("unique constraint", StringComparison.OrdinalIgnoreCase) ?? false)
                {
                    return Conflict(new ApiResponse<InsuranceAgent>
                    {
                        Success = false,
                        Message = "Duplikat nazwy użytkownika lub maila"
                    });
                }
                return StatusCode(500, new ApiResponse<InsuranceAgent>
                {
                    Success = false,
                    Message = "Pojawił się błąd przy dodawaniu użytkownika" + ex.Message
                });
            }
        }

        // DELETE: api/InsuranceAgents/5
        [HttpDelete("{id}")]
        [ActionName("DeleteAgent")]
        public async Task<ActionResult<ApiResponse<InsuranceAgent>>> DeleteInsuranceAgent(int id)
        {
            var insuranceAgent = await _db.InsuranceAgents.FindAsync(id);
            if (insuranceAgent == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Nie odnalazłem użytkownika o ID {id}"
                });
            }
            else
            {
                _db.InsuranceAgents.Remove(insuranceAgent);
                await _db.SaveChangesAsync();
                return Ok(new ApiResponse<InsuranceAgent>
                {
                    Success = true,
                    Message = $"Użytkownik o {id} skasowany"
                });
            }
        }

        private bool InsuranceAgentExists(int id)
        {
            return _db.InsuranceAgents.Any(e => e.Id == id);
        }
    }
}
