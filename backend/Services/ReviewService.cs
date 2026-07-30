using backend.DTOs;
using backend.Model;
using backend.NotificationBuilders;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;
        private readonly NotificationService _notificationService;

        public ReviewService(AppDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<ReviewResponseDto> AddReviewAsync(int jobId, int reviewerId, CreateReviewDto dto)
        {
            var job = await _context.Jobs
                .Include(j => j.Applications)
                .Include(j => j.Client)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
            {
                throw new KeyNotFoundException($"Job with ID {jobId} was not found.");
            }

            if (job.JobStatus != JobStatus.Finished)
            {
                throw new InvalidOperationException("Reviews can only be submitted after a job has been completed (Finished).");
            }

            var hiredApp = job.Applications
                .FirstOrDefault(a => a.AppStatus == AppStatus.Accepted || a.AppStatus == AppStatus.JobDone);

            if (hiredApp == null)
            {
                throw new InvalidOperationException("No hired freelancer application was found for this job.");
            }

            int revieweeId;
            if (reviewerId == job.ClientId)
            {
                // Client is reviewing the Freelancer
                revieweeId = hiredApp.FreelancerId;
            }
            else if (reviewerId == hiredApp.FreelancerId)
            {
                // Freelancer is reviewing the Client
                revieweeId = job.ClientId;
            }
            else
            {
                throw new UnauthorizedAccessException("Only the client or the hired freelancer for this job can leave a review.");
            }

            var alreadyReviewed = await _context.Reviews
                .AnyAsync(r => r.JobId == jobId && r.ReviewerId == reviewerId);

            if (alreadyReviewed)
            {
                throw new InvalidOperationException("You have already submitted a review for this job.");
            }

            var review = new Review
            {
                JobId = jobId,
                ReviewerId = reviewerId,
                RevieweeId = revieweeId,
                Rate = dto.Rate,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var reviewerUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == reviewerId);
            var revieweeUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == revieweeId);

            var notificationBuilder = new ReviewReceivedNotificationBuilder(revieweeId, reviewerUser?.UserName ?? string.Empty, job.Title);
            await _notificationService.SendNotificationAsync(notificationBuilder);

            return new ReviewResponseDto
            {
                Id = review.Id,
                JobId = job.Id,
                JobTitle = job.Title,
                ReviewerId = reviewerId,
                ReviewerName = reviewerUser?.UserName ?? string.Empty,
                RevieweeId = revieweeId,
                RevieweeName = revieweeUser?.UserName ?? string.Empty,
                Rate = review.Rate,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };
        }

        public async Task<List<ReviewResponseDto>> GetJobReviewsAsync(int jobId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.JobId == jobId)
                .Include(r => r.Job)
                .Include(r => r.Reviewer)
                .Include(r => r.Reviewee)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewResponseDto
                {
                    Id = r.Id,
                    JobId = r.JobId,
                    JobTitle = r.Job.Title,
                    ReviewerId = r.ReviewerId,
                    ReviewerName = r.Reviewer.UserName,
                    RevieweeId = r.RevieweeId,
                    RevieweeName = r.Reviewee.UserName,
                    Rate = r.Rate,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return reviews;
        }

        public async Task<UserReviewSummaryDto> GetUserReviewsAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} was not found.");
            }

            var reviews = await _context.Reviews
                .Where(r => r.RevieweeId == userId)
                .Include(r => r.Job)
                .Include(r => r.Reviewer)
                .Include(r => r.Reviewee)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewResponseDto
                {
                    Id = r.Id,
                    JobId = r.JobId,
                    JobTitle = r.Job.Title,
                    ReviewerId = r.ReviewerId,
                    ReviewerName = r.Reviewer.UserName,
                    RevieweeId = r.RevieweeId,
                    RevieweeName = r.Reviewee.UserName,
                    Rate = r.Rate,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            decimal avgRating = reviews.Any()
                ? Math.Round((decimal)reviews.Average(r => r.Rate), 2)
                : 0;

            return new UserReviewSummaryDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                AverageRating = avgRating,
                TotalReviews = reviews.Count,
                Reviews = reviews
            };
        }
    }
}
