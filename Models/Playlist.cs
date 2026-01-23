using System;
using System.Collections.Generic;

namespace DigitalSignageMVP.Models;

public class Playlist
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<MediaFile> MediaFiles { get; set; } = new();
}

