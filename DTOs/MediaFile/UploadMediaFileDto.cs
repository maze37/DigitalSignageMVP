using System.ComponentModel.DataAnnotations;

namespace DigitalSignageMVP.DTOs.MediaFile;

public class UploadMediaFileDto
{
    [Required]
    public IFormFile? File { get; set; }
}