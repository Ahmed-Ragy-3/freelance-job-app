using backend.Model;

namespace backend.Repositories
{
    public interface IFreelancerApplicationRepository
    {
        Task<List<Application>> GetApplicationsByFreelancerIdAsync(int freelancerId);
        Task<Application?> GetApplicationAsync(int jobId, int freelancerId);
        Task<Job?> GetJobByIdAsync(int jobId);
        Task<bool> HasAlreadyAppliedAsync(int freelancerId, int jobId);
        Task<Application> CreateApplicationAsync(Application application);
        Task UpdateApplicationAsync(Application application);
    }
}
