using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InsureApp.Server.Model;
using InsureApp.Server.Services;

namespace InsureApp.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InsuranceDocumentsController : ControllerBase
    {
        private readonly InsuranceDbContext _context;
        private readonly IFileService _fileService;
        private readonly string _uploadsFolder;
        private readonly ILogger<InsuranceDocumentsController> _logger;

        public InsuranceDocumentsController(InsuranceDbContext context, IFileService fileService,
        IConfiguration configuration, ILogger<InsuranceDocumentsController> logger)
        {
            _context = context;
            _fileService = fileService;
            _uploadsFolder = configuration.GetValue<string>("FileStorage:UploadPath")
                ?? Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            _logger = logger;
        }

        // GET
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InsuranceDocument>>> GetInsuranceDocuments()
        {
            return await _context.InsuranceDocuments.ToListAsync();
        }

        // GET
        [HttpGet("{id}")]
        public async Task<IActionResult> DownloadDocument(int id)
        {
            var document = await _context.InsuranceDocuments.FindAsync(id);
            if (document == null)
                return NotFound();

            var filePath = Path.Combine(_uploadsFolder, document.FilePath);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(document.FileType), document.FileName);
        }

        // PUT: api/InsuranceDocuments/5
        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInsuranceDocument(int id, InsuranceDocument insuranceDocument)
        {
            if (id != insuranceDocument.Id)
            {
                return BadRequest();
            }

            _context.Entry(insuranceDocument).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InsuranceDocumentExists(id))
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

        // POST: api/InsuranceDocuments
        [HttpPost]
        public async Task<ActionResult<ApiResponse<InsuranceDocument>>> UploadDocument(
    [FromForm] int reportId,
    [FromForm] IFormFile file,
    [FromForm] string? description)
        {
            try
            {
                // Sanity check na istniejący raport
                var report = await _context.InsuranceReports.FindAsync(reportId);
                if (report == null)
                    return NotFound(new ApiResponse<InsuranceDocument>
                    {
                        Success = false,
                        Message = "Nie odnaleziono raportu o tym ID"
                    });

                var (success, fileName) = await _fileService.SaveFileAsync(file, _uploadsFolder, reportId);
                if (!success)
                    return BadRequest(new ApiResponse<InsuranceDocument>
                    {
                        Success = false,
                        Message = "Błędny plik"
                    });

                var document = new InsuranceDocument
                {
                    FileName = file.FileName,
                    FilePath = fileName,
                    FileType = Path.GetExtension(file.FileName),
                    Description = description,
                    UploadDate = DateTime.UtcNow,
                    InsuranceReportId = reportId,
                    InsuranceReport = report
                };

                _context.InsuranceDocuments.Add(document);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<InsuranceDocument>
                {
                    Success = true,
                    Data = document,
                    Message = "Dokument załadowany poprawnie"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd uploadu pliku do raportu {reportId}");
                return StatusCode(500, new ApiResponse<InsuranceDocument>
                {
                    Success = false,
                    Message = "Błąd przy uploadzie" + ex.Message
                });
            }
        }

        // DELETE: api/InsuranceDocuments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInsuranceDocument(int id)
        {
            var insuranceDocument = await _context.InsuranceDocuments.FindAsync(id);
            if (insuranceDocument == null)
            {
                return NotFound();
            }

            _context.InsuranceDocuments.Remove(insuranceDocument);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InsuranceDocumentExists(int id)
        {
            return _context.InsuranceDocuments.Any(e => e.Id == id);
        }

        private string GetContentType(string? fileType)
        {
            return fileType?.ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }
    }
}
