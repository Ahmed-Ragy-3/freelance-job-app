using backend.model;
using Microsoft.EntityFrameworkCore;

namespace backend.Services {
    public class CategoryService(AppDbContext appDbContext) {
        public async Task<List<Category>> GetTopNCategoriesAsync(int n) {
            return await appDbContext.categories
                .OrderByDescending(c => c.JobCategories.Count)
                .Take(n)
                .ToListAsync();
        }
    }
}
