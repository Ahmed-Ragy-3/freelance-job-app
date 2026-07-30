using backend.DTOs;

namespace backend.Services
{
    public interface IReviewService
    {
        Task<ReviewResponseDto> AddReviewAsync(int jobId, int reviewerId, CreateReviewDto dto);
        Task<List<ReviewResponseDto>> GetJobReviewsAsync(int jobId);
        Task<UserReviewSummaryDto> GetUserReviewsAsync(int userId);
    }
}
