namespace HotelDBMiddle.Interfaces_And_Service
{
    public interface IFileUploadService
    {
         bool DeleteSingleFile(string filePath);
         Task<string> UploadSingleFiles(string [] destination, IFormFile files);
         Task<List<string>> UploadMultipleFiles(string [] destination, List<IFormFile> files);
    }
}