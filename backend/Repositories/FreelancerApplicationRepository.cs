using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class FreelancerApplicationRepository : IFreelancerApplicationRepository
    {
        private readonly AppDbContext _context;

        public FreelancerApplicationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Application>> GetApplicationsByFreelancerIdAsync(int freelancerId)
        {
            return await _context.Applications
                .Include(a => a.Job)
                .Where(a => a.FreelancerId == freelancerId)
                .OrderByDescending(a => a.JobId)
                .ToListAsync();
        }

        public async Task<Application?> GetApplicationAsync(int jobId, int freelancerId)
        {
            return await _context.Applications
                .Include(a => a.Job)
                .FirstOrDefaultAsync(a => a.JobId == jobId && a.FreelancerId == freelancerId);
        }

        public async Task<Job?> GetJobByIdAsync(int jobId)
        {
            return await _context.Jobs
                .FirstOrDefaultAsync(j => j.Id == jobId);
        }

        public async Task<bool> HasAlreadyAppliedAsync(int freelancerId, int jobId)
        {
            // Checks if an active (non-withdrawn) application already exists
            return await _context.Applications
                .AnyAsync(a => a.FreelancerId == freelancerId
                            && a.JobId == jobId
                            && a.AppStatus != AppStatus.Withdrawn);
        }

        public async Task<Application> CreateApplicationAsync(Application application)
        {
            await _context.Applications.AddAsync(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task UpdateApplicationAsync(Application application)
        {
            _context.Applications.Update(application);
            await _context.SaveChangesAsync();
        }
    }
}
