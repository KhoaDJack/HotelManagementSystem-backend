
namespace HotelDBMiddle.Interfaces_And_Service
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;

        public FileUploadService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }


        public bool DeleteSingleFile(string filePath)
        {
            var fullPath = Path.Combine(_environment.WebRootPath, filePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            return false;
        }

        public async Task<List<string>> UploadMultipleFiles(string[] destination, List<IFormFile> files)
        {
            var filePaths = new List<string>();
            foreach (var file in files)
            {
                var filePath = await UploadSingleFiles(destination, file);
                if (filePath != null)
                {
                    filePaths.Add(filePath);
                }
            }
            return filePaths;
        }

        public async Task<string> UploadSingleFiles(string[] destination, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var uploadPath = Path.Combine(_environment.WebRootPath, Path.Combine(destination));
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);
            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var relativePath = Path.Combine(destination).Replace("\\", "/");
            return $"{relativePath}/{fileName}";
        }

    }
}