using backend.model;
using Microsoft.EntityFrameworkCore;

namespace backend.Services {
    public class ClientService(AppDbContext appDbContext) {

        public async Task<List<Client>> GetTopNClientsAsync(int n) {
            return await appDbContext.Clients
                .OrderByDescending(c => c.Jobs.Count)
                .Take(n)
                .ToListAsync();
        }
    }
}
