using backend.DTOs;
using backend.Model;
using backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Services {
    public class ClientService(AppDbContext appDbContext, IClientRepository clientRepository) : IClientService {
        public async Task<List<ClientSummaryDto>> GetTopNClientsAsync(int n) {
            var clients = await appDbContext.Clients
                .OrderByDescending(c => c.Jobs.Count)
                .Take(n)
                .ToListAsync();

            return clients.Select(ClientSummaryDto.FromClient).ToList();
        }

        public async Task<List<ClientSummaryDto>> SearchClientsAsync(string searchTerm) {
            var clients = await appDbContext.Clients
                .Where(c => c.CompanyName.Contains(searchTerm))
                .ToListAsync();

            return clients.Select(ClientSummaryDto.FromClient).ToList();
        }

        public async Task<ClientProfileDto?> GetProfileByUserIdAsync(int userId)
        {
            var client = await clientRepository.GetByUserIdAsync(userId);
            if (client == null) return null;

            return new ClientProfileDto
            {
                UserId = client.UserId,
                UserName = client.User?.UserName,
                Email = client.User?.Email,
                ImageUrl = client.User?.ImageUrl,
                CompanyName = client.CompanyName,
                CompanyDetails = client.CompanyDetails,
                Logo = client.Logo
            };
        }

        public async Task<ClientProfileDto?> UpdateProfileAsync(int userId, UpdateClientProfileDto dto)
        {
            var client = await clientRepository.GetByUserIdAsync(userId);
            if (client == null) return null;

            client.CompanyName = dto.CompanyName ?? client.CompanyName;
            client.CompanyDetails = dto.CompanyDetails ?? client.CompanyDetails;
            client.Logo = dto.Logo ?? client.Logo;

            await clientRepository.UpdateProfileAsync(client);

            return new ClientProfileDto
            {
                UserId = client.UserId,
                UserName = client.User?.UserName,
                Email = client.User?.Email,
                ImageUrl = client.User?.ImageUrl,
                CompanyName = client.CompanyName,
                CompanyDetails = client.CompanyDetails,
                Logo = client.Logo
            };
        }
    }
}
