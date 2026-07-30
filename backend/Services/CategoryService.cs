using backend.DTOs;
using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.Services {
    public class CategoryService(AppDbContext appDbContext) {
        public async Task<List<CategorySummaryDto>> GetAllCategoriesAsync() {
            var categories = await appDbContext.Categories
                .Include(c => c.JobCategories)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return categories.Select(CategorySummaryDto.FromCategory).ToList();
        }

        public async Task<List<CategorySummaryDto>> GetTopNCategoriesAsync(int n) {
            var categories = await appDbContext.Categories
                .Include(c => c.JobCategories)
                .OrderByDescending(c => c.JobCategories.Count)
                .Take(n)
                .ToListAsync();

            return categories.Select(CategorySummaryDto.FromCategory).ToList();
        }

        public async Task<List<CategorySummaryDto>> SearchCategoriesAsync(string searchTerm) {
            var categories = await appDbContext.Categories
                .Include(c => c.JobCategories)
                .Where(c => c.Name.Contains(searchTerm))
                .ToListAsync();

            return categories.Select(CategorySummaryDto.FromCategory).ToList();
        }

        public async Task<CategoryResponseDto> CreateAsync(string name) {
            var normalizedName = name.Trim();
            if (string.IsNullOrWhiteSpace(normalizedName))
                throw new ArgumentException("Category name is required.");

            var exists = await appDbContext.Categories.AnyAsync(c => c.Name == normalizedName);
            if (exists)
                throw new InvalidOperationException($"Category '{normalizedName}' already exists.");

            var category = new Category { Name = normalizedName };
            appDbContext.Categories.Add(category);
            await appDbContext.SaveChangesAsync();

            return new CategoryResponseDto { Id = category.Id, Name = category.Name };
        }

        public async Task<CategoryResponseDto> UpdateAsync(int id, string name) {
            var category = await appDbContext.Categories.FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new KeyNotFoundException($"Category with ID {id} was not found.");

            var normalizedName = name.Trim();
            if (string.IsNullOrWhiteSpace(normalizedName))
                throw new ArgumentException("Category name is required.");

            var exists = await appDbContext.Categories.AnyAsync(c => c.Name == normalizedName && c.Id != id);
            if (exists)
                throw new InvalidOperationException($"Category '{normalizedName}' already exists.");

            category.Name = normalizedName;
            await appDbContext.SaveChangesAsync();

            return new CategoryResponseDto { Id = category.Id, Name = category.Name };
        }

        public async Task DeleteAsync(int id) {
            var category = await appDbContext.Categories.FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new KeyNotFoundException($"Category with ID {id} was not found.");

            var links = await appDbContext.JobCategories.Where(jc => jc.CategoryId == id).ToListAsync();
            if (links.Count > 0)
                appDbContext.JobCategories.RemoveRange(links);

            appDbContext.Categories.Remove(category);
            await appDbContext.SaveChangesAsync();
        }
    }
}
