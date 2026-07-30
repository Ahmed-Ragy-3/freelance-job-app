using backend.DTOs;
using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.Services {
    public class BookmarkService(AppDbContext appDbContext) {
        public async Task Save(int jobId, int userId) {
            // Verify the job exists
            var jobExists = await appDbContext.Jobs
                .AnyAsync(j => j.Id == jobId);

            if (!jobExists)
                throw new ArgumentException("Job not found.");

            // Prevent duplicate bookmarks
            var alreadyBookmarked = await appDbContext.Bookmarks
                .AnyAsync(b => b.UserId == userId && b.JobId == jobId);

            if (alreadyBookmarked)
                throw new ArgumentException("Job is already bookmarked.");

            var bookmark = new Bookmark {
                UserId = userId,
                JobId = jobId
            };

            appDbContext.Bookmarks.Add(bookmark);

            await appDbContext.SaveChangesAsync();
        }

        public async Task<PaginatedResponse<JobSummaryDto>> GetBookmarksAsync(BookmarkFetchDto dto) {
            var query = appDbContext.Jobs.AsNoTracking()
                        .Where(j => j.Bookmarks.Any(b => b.UserId == dto.UserId));

            var totalCount = await query.CountAsync();

            var jobs = await query
                    .Include(j => j.Categories)
                        .ThenInclude(jc => jc.Category)
                    .Include(j => j.Tags)
                        .ThenInclude(jt => jt.Tag)
                    .Include(j => j.Applications)
                    .OrderByDescending(j => j.PostedAt)
                    .Skip((dto.Page - 1) * dto.PageSize)
                    .Take(dto.PageSize)
                    .ToListAsync();

            return new PaginatedResponse<JobSummaryDto> {
                Items = jobs.Select(JobSummaryDto.FromJob).ToList(),
                TotalCount = totalCount,
            };
        }

        public async Task Remove(int jobId, int userId) {
            var bookmark = await appDbContext.Bookmarks
                .FirstOrDefaultAsync(b => b.UserId == userId && b.JobId == jobId);

            if (bookmark == null)
                throw new ArgumentException("Bookmark not found.");

            appDbContext.Bookmarks.Remove(bookmark);

            await appDbContext.SaveChangesAsync();
        }
    }
}
