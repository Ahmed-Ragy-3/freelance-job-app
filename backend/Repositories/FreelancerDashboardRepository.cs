using backend.DTOs;
using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class FreelancerDashboardRepository : IFreelancerDashboardRepository
    {
        private readonly AppDbContext _context;

        public FreelancerDashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        // Active Applications: Submitted & pending review
        public async Task<int> GetActiveApplicationsCountAsync(int freelancerId)
        {
            return await _context.Applications
                .CountAsync(a => a.FreelancerId == freelancerId && a.AppStatus == AppStatus.In_Progress);
        }

        // Active Jobs: Accepted applications where the job is currently in progress or delayed
        public async Task<int> GetActiveJobsCountAsync(int freelancerId)
        {
            return await _context.Applications
                .CountAsync(a => a.FreelancerId == freelancerId
                              && a.AppStatus == AppStatus.Accepted
                              && (a.Job.JobStatus == JobStatus.In_Progress
                                  || a.Job.JobStatus == JobStatus.Delayed));
        }

        // Completed Jobs: Applications on finished jobs
        public async Task<int> GetCompletedJobsCountAsync(int freelancerId)
        {
            return await _context.Applications
                .CountAsync(a => a.FreelancerId == freelancerId
                              && (a.AppStatus == AppStatus.JobDone || a.AppStatus == AppStatus.Accepted)
                              && a.Job.JobStatus == JobStatus.Finished);
        }

        // Total Earnings: Sum of bids on finished jobs
        public async Task<decimal> GetTotalEarningsAsync(int freelancerId)
        {
            return await _context.Applications
                .Where(a => a.FreelancerId == freelancerId
                         && (a.AppStatus == AppStatus.JobDone || a.AppStatus == AppStatus.Accepted)
                         && a.Job.JobStatus == JobStatus.Finished)
                .SumAsync(a => (decimal)a.Bid);
        }

        public async Task<int> GetTotalBookmarksCountAsync(int freelancerId)
        {
            return await _context.Bookmarks
                .CountAsync(b => b.UserId == freelancerId);
        }

        public async Task<int> GetUnreadNotificationsCountAsync(int userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task<decimal> GetAverageRatingAsync(int freelancerId)
        {
            var ratings = await _context.Applications
                .Where(a => a.FreelancerId == freelancerId
                         && a.AppStatus == AppStatus.JobDone
                         && a.Job.Review != null)
                .Select(a => (decimal?)a.Job.Review!.Rate)
                .ToListAsync();

            if (!ratings.Any() || ratings.All(r => r == null))
            {
                return 0.0m;
            }

            return Math.Round((decimal)ratings.Average()!, 2);
        }

        public async Task<List<RecentApplicationDto>> GetRecentApplicationsAsync(int freelancerId, int count = 5)
        {
            return await _context.Applications
                .Where(a => a.FreelancerId == freelancerId)
                .OrderByDescending(a => a.JobId)
                .Take(count)
                .Select(a => new RecentApplicationDto
                {
                    JobId = a.JobId,
                    JobTitle = a.Job.Title,
                    CompanyName = _context.Clients
                        .Where(c => c.UserId == a.Job.ClientId)
                        .Select(c => c.CompanyName)
                        .FirstOrDefault() ?? string.Empty,
                    CompanyLogo = _context.Clients
                        .Where(c => c.UserId == a.Job.ClientId)
                        .Select(c => c.Logo)
                        .FirstOrDefault(),
                    Bid = a.Bid,
                    Timeline = a.Timeline,
                    AppStatus = a.AppStatus
                })
                .ToListAsync();
        }
    }
}
