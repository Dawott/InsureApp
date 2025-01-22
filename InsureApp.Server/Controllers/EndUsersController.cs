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
        private readonly ILogger<EndUsersController> _logger;

        public EndUsersController(InsuranceDbContext context, ILogger<EndUsersController> logger)
        {
            _db = context;
            _logger = logger;
        }

       
        [HttpGet]
        [ActionName("GetAllCustomers")]
        public async Task<ActionResult<IEnumerable<EndUser>>> GetEndUsers()
        {
            try
            {
                var users = await _db.EndUsers.ToListAsync();
                return Ok(new ApiResponse<IEnumerable<EndUser>>
                {
                    Success = true,
                    Data = users,
                    Message = "Zwrócono wszystkich użytkowników"

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd przy zwracaniu użytkowników");
                return StatusCode(500, new ApiResponse<IEnumerable<EndUser>>
                {
                    Success = false,
                    Message = $"Błąd serwera podczas zwracania użytkowników. Szczegóły: {ex}"
                }
                    );
            }
        }


        // GET: api/EndUsers/5
        [HttpGet("{id}")]
        [ActionName("GetUserByID")]
        public async Task<ActionResult<EndUser>> GetEndUser(int id)
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

            //return endUser;
        }

        //Znajdź po mailu
        [HttpGet]
        [ActionName("GetUserIDByMail")]
        public async Task<ActionResult<EndUser>> GetUserByMail([FromQuery] string email)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest(new ApiResponse<EndUser>
                    {   Success = false,
                        Message = "Proszę podać poprawny email"               
                    });
                }

                var user = await _db.EndUsers.FirstOrDefaultAsync(u => 
                u.Email.ToLower() == email.ToLower());

                if (user == null)
                {
                    return NotFound(new ApiResponse<EndUser>
                    {   Success = false,
                        Message = $"Nie ma użytkownika o mailu {email}" }) ;
                }
                return Ok(new ApiResponse<EndUser>
                {
                    Success= true,
                    Message = "Użytkownik znaleziony",
                    Data = user
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas wyszukiwania użytkownika");
                return StatusCode(500, new ApiResponse<EndUser> 
                { 
                    Success= false,
                    Message = "Błąd ze strony serwera! Powód: "+ex
                });
            }
        }

        // PUT
        [HttpPut("{id}")]
        [ActionName("EditUser")]
        public async Task<IActionResult> PutEndUser(int id, EndUser endUser)
        {
            if (id != endUser.Id)
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
                if (await _db.EndUsers.AnyAsync(u =>
                u.Id != id &&
                u.Username.ToLower() == endUser.Username.ToLower()))
                {
                    return Conflict(new ApiResponse<EndUser>
                    {
                        Success = false,
                        Message = $"Nazwa '{endUser.Username}' jest już zajęta"
                    }
                       );
                }

                //Na wszelki wypadek - lock na Emailu
                if (await _db.EndUsers.AnyAsync(u =>
                u.Id != id &&
                u.Email.ToLower() == endUser.Email.ToLower()))
                {
                    return Conflict(new ApiResponse<EndUser>
                    {
                        Success = false,
                        Message = $"Email '{endUser.Email}' jest już w bazie"
                    });
                }

                _db.Entry(endUser).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return Ok(new ApiResponse<EndUser>
                {
                    Success = true,
                    Data = endUser,
                    Message = "Update zakończony sukcesem"
                });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Błąd concurrency podczas update'u usera {id}");
                if (!EndUserExists(id))
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
                _logger.LogError(ex, $"Błąd update'u użytkownika ID {id}");

                if(ex.InnerException?.Message.Contains("unique constraint", StringComparison.OrdinalIgnoreCase) ?? false)
                {
                    return Conflict(new ApiResponse<EndUser>
                    {
                        Success = false,
                        Message = "Duplikat nazwy użytkownika lub maila"
                    });
                }
                return StatusCode(500, new ApiResponse<EndUser>
                {
                    Success = false,
                    Message = "Pojawił się błąd przy dodawaniu użytkownika" + ex.Message
                });
            }
        }

        // POST: api/EndUsers
        [HttpPost]
        [ActionName("AddUserManually")]
        public async Task<ActionResult<EndUser>> PostEndUser(EndUser endUser)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<EndUser>
                {
                    Success = false,
                    Message = "Błędne dane!"
                });
            }

            //Jeśli username istnieje
            if (await _db.EndUsers.AnyAsync(u => u.Username.ToLower() == endUser.Username.ToLower()))
            {
                return Conflict(new ApiResponse<EndUser>
                {
                    Success = false,
                    Message = $"Nazwa Użytkownika jest już zajęta"
                }
                    );
            }

            //Jeśli mail już jest w bazie
            if (await _db.EndUsers.AnyAsync(u => u.Email.ToLower() == endUser.Email.ToLower()))
            {
                return Conflict(new ApiResponse<EndUser>
                {
                    Success = false,
                    Message = $"Taki adres już jest w bazie. Zresetuj hasło, jeśli go nie pamiętasz albo skonsultuj się z agentem"
                }
                    );
            }

            try
            {
                _db.EndUsers.Add(endUser);
                await _db.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetEndUser),
                    new { id = endUser.Id },
                    new ApiResponse<EndUser>
                    {
                        Success = true,
                        Data = endUser,
                        Message = "Utworzono użytkownika"
                    });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Błąd w utworzeniu użytkownika");
                if(ex.InnerException?.Message.Contains("unique constraint", StringComparison.OrdinalIgnoreCase) ?? false) 
                {
                    return Conflict(new ApiResponse<EndUser>
                    {
                        Success = false,
                        Message = "Duplikat nazwy użytkownika lub maila"
                    });
                }
                return StatusCode(500, new ApiResponse<EndUser>
                {
                    Success = false,
                    Message = "Pojawił się błąd przy dodawaniu użytkownika" + ex.Message
                });
            } 
        }

        // DELETE: api/EndUsers/5
        [HttpDelete("{id}")]
        [ActionName("DeleteUser")]
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
