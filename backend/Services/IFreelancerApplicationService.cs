using backend.DTOs;

namespace backend.Services
{
    public interface IFreelancerApplicationService
    {
        Task<List<ApplicationResponseDto>> GetMyApplicationsAsync(int freelancerId);
        Task<ApplicationResponseDto> ApplyToJobAsync(int freelancerId, ApplyJobDto dto);
        Task<ApplicationResponseDto> SaveApplicationDraftAsync(int freelancerId, SaveApplicationDraftDto dto);
        Task<ApplicationResponseDto> SubmitApplicationAsync(int freelancerId, int jobId, ApplyJobDto dto);
        Task<bool> WithdrawApplicationAsync(int freelancerId, int jobId);
        Task<ApplicationResponseDto> SubmitJobAsync(int freelancerId, int jobId);
    }
}
