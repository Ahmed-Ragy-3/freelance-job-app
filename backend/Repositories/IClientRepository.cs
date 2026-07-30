using backend.Model;

namespace backend.Repositories
{
    public interface IClientRepository
    {
        Task<Client?> GetByUserIdAsync(int userId);
        Task UpdateProfileAsync(Client client);
    }
}
