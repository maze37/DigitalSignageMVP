using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DigitalSignageMVP.Services.FileStorageService;

public interface IFileStorageService
{
    Task<string> UploadAsync(IFormFile file);
    Task DeleteAsync(string fileUrl);
}
