using backend.DTOs;
using backend.Auth;
using backend.Model;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/freelancer/applications")]
    [Authorize(Roles = "Freelancer")]
    public class FreelancerApplicationsController : ControllerBase
    {
        private readonly IFreelancerApplicationService _applicationService;

        public FreelancerApplicationsController(IFreelancerApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        /// <summary>
        /// Retrieves all applications submitted by the logged-in freelancer sorted by most recent.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ApplicationResponseDto>>> GetMyApplications()
        {
            int? userId = User.GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });
            }

            var applications = await _applicationService.GetMyApplicationsAsync(userId.Value);
            return Ok(applications);
        }

        /// <summary>
        /// Submits a new job application.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApplicationResponseDto>> ApplyToJob([FromBody] ApplyJobDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int? userId = User.GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });
            }

            try
            {
                var createdApp = await _applicationService.ApplyToJobAsync(userId.Value, dto);

                return CreatedAtAction(
                    nameof(GetMyApplications),
                    new { id = createdApp.JobId },
                    createdApp
                );
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Withdraws an active application.
        /// </summary>
        [HttpPut("job/{jobId:int}/withdraw")]
        public async Task<IActionResult> WithdrawApplication(int jobId)
        {
            int? userId = User.GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });
            }

            try
            {
                await _applicationService.WithdrawApplicationAsync(userId.Value, jobId);
                return Ok(new { message = "Application withdrawn successfully." });
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

    }
}
