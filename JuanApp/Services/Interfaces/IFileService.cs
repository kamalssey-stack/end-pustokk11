using Microsoft.AspNetCore.Http;

namespace JuanApp.Services.Interfaces;

public interface IFileService
{
    Task<string> UploadAsync(IFormFile file, string folder);
    bool Delete(string folder, string fileName);
}
