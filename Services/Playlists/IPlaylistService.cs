using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalSignageMVP.DTOs.Playlist;

namespace DigitalSignageMVP.Services.Playlists;

public interface IPlaylistService
{
    Task<IEnumerable<PlaylistResponseDto>> GetAllAsync();
    Task<PlaylistDetailResponseDto?> GetByIdAsync(int id);
    Task<PlaylistResponseDto> CreateAsync(CreatePlaylistDto dto);
    Task<PlaylistResponseDto?> UpdateAsync(int id, UpdatePlaylistDto dto);
    Task<bool> DeleteAsync(int id);

    Task<bool> AddMediaAsync(int playlistId, int mediaFileId);
    Task<bool> RemoveMediaAsync(int playlistId, int mediaFileId);
}
