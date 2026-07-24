using backend.Dtos;
using backend.model;
using Microsoft.EntityFrameworkCore;

namespace backend.Services {
    public class CategoryService(AppDbContext appDbContext) {
        public async Task<List<CategorySummaryDto>> GetTopNCategoriesAsync(int n) {
            var categories = await appDbContext.categories
                .OrderByDescending(c => c.JobCategories.Count)
                .Take(n)
                .ToListAsync();

            return categories.Select(CategorySummaryDto.FromCategory).ToList();
        }
    }
}
