using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DigitalSignageMVP.DTOs.MediaFile;

public class UploadMediaFileDto
{
    [Required]
    public IFormFile? File { get; set; }
}