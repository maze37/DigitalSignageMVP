using System.ComponentModel.DataAnnotations;

namespace DigitalSignageMVP.DTOs.Playlist;

public class UpdatePlaylistDto
{
    [StringLength(128)]
    public string? Name { get; set; }
}