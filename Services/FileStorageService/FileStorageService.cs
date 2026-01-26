using System;
using System.IO;
using System.Threading.Tasks;
using DigitalSignageMVP.Services.MediaFiles;
using Microsoft.AspNetCore.Http;

namespace DigitalSignageMVP.Services.FileStorageService;

public class FileStorageService : IFileStorageService
{
    /// <summary>
    /// Надо дописать позже
    /// </summary>
    
    public async Task<string> UploadAsync(IFormFile file)
    {
        ArgumentNullException.ThrowIfNull(file);

        var extension = Path.GetExtension(file.FileName).ToLower();
        var fileName = $"{Guid.NewGuid()}{extension}";

        // реальная загрузка в S3 / MinIO
        // await _client.PutObjectAsync(...)
        
        return $"https://storage.example.com/media/{fileName}";
    }

    public async Task DeleteAsync(string fileUrl)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileUrl);

        // извлечь key из URL
        // await _client.RemoveObjectAsync(...)
        await Task.Delay(10);
    }
}