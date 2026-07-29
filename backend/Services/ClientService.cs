using backend.DTOs;
using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.Services {
    public class ClientService(AppDbContext appDbContext) {
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
    }
}
