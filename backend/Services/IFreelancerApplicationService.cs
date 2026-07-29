using backend.DTOs;

namespace backend.Services
{
    public interface IFreelancerApplicationService
    {
        Task<List<ApplicationResponseDto>> GetMyApplicationsAsync(int freelancerId);
        Task<ApplicationResponseDto> ApplyToJobAsync(int freelancerId, ApplyJobDto dto);
        Task<bool> WithdrawApplicationAsync(int freelancerId, int jobId);
    }
}
