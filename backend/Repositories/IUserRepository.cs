using backend.Model;

namespace backend.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int userId);
        Task<bool> IsUserNameTakenAsync(string username, int excludeUserId);
        Task UpdateUserAsync(User user);
    }
}
