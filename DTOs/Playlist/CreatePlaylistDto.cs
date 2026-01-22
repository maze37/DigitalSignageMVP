using System.ComponentModel.DataAnnotations;

namespace DigitalSignageMVP.DTOs.Playlist;

public class CreatePlaylistDto
{
    [Required]
    [StringLength(128)]
    public string Name { get; set; } = string.Empty;
}