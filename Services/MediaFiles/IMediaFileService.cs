using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalSignageMVP.DTOs.MediaFile;

namespace DigitalSignageMVP.Services.MediaFiles;

public interface IMediaFileService
{
    Task<IEnumerable<MediaFileResponseDto>> GetAsync(string? name, int? playlistId);
    Task<MediaFileResponseDto?> GetByIdAsync(int id);
    Task<MediaFileResponseDto> UploadAsync(UploadMediaFileDto request);
    Task<bool> DeleteAsync(int id);
}