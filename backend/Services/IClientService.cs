using backend.DTOs;

namespace backend.Services
{
    public interface IClientService
    {
        Task<ClientProfileDto?> GetProfileByUserIdAsync(int userId);
        Task<ClientProfileDto?> UpdateProfileAsync(int userId, UpdateClientProfileDto dto);
        Task<List<ClientSummaryDto>> GetTopNClientsAsync(int n);
        Task<List<ClientSummaryDto>> SearchClientsAsync(string searchTerm);
    }
}
