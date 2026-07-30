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
        /// Submits a new job application with optional portfolio/proposal attachments (PDFs or Images).
        /// </summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ApplicationResponseDto>> ApplyToJob([FromForm] ApplyJobDto dto)
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
                var result = await _applicationService.ApplyToJobAsync(userId.Value, dto);
                return CreatedAtAction(nameof(GetMyApplications), new { id = result.ApplicationId }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
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

        /// <summary>
        /// Saves or updates a draft application without submitting it to the client.
        /// </summary>
        [HttpPost("draft")]
        public async Task<ActionResult<ApplicationResponseDto>> SaveApplicationDraft([FromBody] SaveApplicationDraftDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int? userId = User.GetUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });

            try
            {
                var result = await _applicationService.SaveApplicationDraftAsync(userId.Value, dto);
                return Ok(result);
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
        /// Submits a saved draft application to the client for review.
        /// </summary>
        [HttpPut("job/{jobId:int}/submit-application")]
        public async Task<ActionResult<ApplicationResponseDto>> SubmitApplication(int jobId, [FromBody] ApplyJobDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int? userId = User.GetUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });

            try
            {
                var result = await _applicationService.SubmitApplicationAsync(userId.Value, jobId, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Freelancer marks their work as complete and ready for client review.
        /// </summary>
        [HttpPut("job/{jobId:int}/submit")]
        public async Task<ActionResult<ApplicationResponseDto>> SubmitJob(int jobId)
        {
            int? userId = User.GetUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });

            try
            {
                var result = await _applicationService.SubmitJobAsync(userId.Value, jobId);
                return Ok(result);
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

    }
}
