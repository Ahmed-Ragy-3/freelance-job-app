using backend.DTOs;
using backend.Auth;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// Submits a review for a completed job (Client reviewing Freelancer OR Freelancer reviewing Client).
        /// </summary>
        [HttpPost("/api/jobs/{jobId:int}/reviews")]
        [Authorize]
        public async Task<ActionResult<ReviewResponseDto>> AddReview(int jobId, [FromBody] CreateReviewDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });
            }

            try
            {
                var result = await _reviewService.AddReviewAsync(jobId, userId.Value, dto);
                return CreatedAtAction(nameof(GetJobReviews), new { jobId = jobId }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all reviews left for a specific finished job.
        /// </summary>
        [HttpGet("/api/jobs/{jobId:int}/reviews")]
        public async Task<ActionResult<List<ReviewResponseDto>>> GetJobReviews(int jobId)
        {
            var reviews = await _reviewService.GetJobReviewsAsync(jobId);
            return Ok(reviews);
        }

        /// <summary>
        /// Retrieves all reviews received by a specific user (Client or Freelancer) along with their average rating.
        /// </summary>
        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<UserReviewSummaryDto>> GetUserReviews(int userId)
        {
            try
            {
                var summary = await _reviewService.GetUserReviewsAsync(userId);
                return Ok(summary);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
