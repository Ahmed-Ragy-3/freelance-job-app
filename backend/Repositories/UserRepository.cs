using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Freelancer)
                .Include(u => u.Client)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> IsUserNameTakenAsync(string username, int excludeUserId)
        {
            return await _context.Users
                .AnyAsync(u => u.UserName.ToLower() == username.ToLower() && u.Id != excludeUserId);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
