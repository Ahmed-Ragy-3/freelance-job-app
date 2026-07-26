using backend.Dtos;
using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.Services {
    public class CategoryService(AppDbContext appDbContext) {
        public async Task<List<CategorySummaryDto>> GetTopNCategoriesAsync(int n) {
            var categories = await appDbContext.Categories
                .OrderByDescending(c => c.JobCategories.Count)
                .Take(n)
                .ToListAsync();

            return categories.Select(CategorySummaryDto.FromCategory).ToList();
        }
    }
}
