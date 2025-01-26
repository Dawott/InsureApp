namespace InsureApp.Server.Services
{
    public interface IFileService
    {
        Task<(bool Success, string FileName)> SaveFileAsync(IFormFile file, string uploadsFolder, int reportId);
        Task<bool> DeleteFileAsync(string baseUploadsFolder, string relativePath);
    }

    public class FileService : IFileService
    {
        private readonly string[] allowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };
        private const int maxFileSizeMB = 10;

        public async Task<(bool Success, string FileName)> SaveFileAsync(IFormFile file, string baseUploadsFolder, int reportId)
        {
            try
            {
                if (file.Length == 0)
                    return (false, string.Empty);

                if (file == null)
                    return (false, string.Empty);

                if (file.Length > maxFileSizeMB * 1024 * 1024)
                    return (false, string.Empty);

                //Sprawdzenie rozszerzenia
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                    return (false, string.Empty);

                var reportFolder = Path.Combine(baseUploadsFolder, reportId.ToString());

                //Tworzenie unikatowej ścieżki
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(reportFolder, uniqueFileName);

                Directory.CreateDirectory(reportFolder);

                // Zapis
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Zwrot ścieżki
                return (true, Path.Combine(reportId.ToString(), uniqueFileName));
            }
            catch
            {
                return (false, string.Empty);
            }
        }

        public async Task<bool> DeleteFileAsync(string baseUploadsFolder, string relativePath)
        {
            try
            {
                var fullPath = Path.Combine(baseUploadsFolder, relativePath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);

                    var directory = Path.GetDirectoryName(fullPath);
                    if (directory != null && Directory.Exists(directory)
                        && !Directory.EnumerateFileSystemEntries(directory).Any())
                    {
                        Directory.Delete(directory);
                    }

                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

    }
}
   
