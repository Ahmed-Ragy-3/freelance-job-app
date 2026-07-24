using backend.Dtos;
using backend.model;
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
    }
}
