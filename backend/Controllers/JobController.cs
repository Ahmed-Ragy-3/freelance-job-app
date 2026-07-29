using backend.dtos;
using backend.DTOs;
using backend.Services;
using backend.Model;
using backend.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers {
    [ApiController]
    [Route("api/jobs")]
    public class JobController(JobService jobService, JobStatusService jobStatusService, AppDbContext appDbContext) : ControllerBase {
        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<JobSummaryDto>>> GetJobs([FromQuery] JobFilterDto filter) {
            var jobs = await jobService.GetJobsAsync(filter);

            return Ok(jobs);
        }

        [HttpGet("client")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<PaginatedResponse<JobClientSummaryDto>>> GetClientJobs([FromQuery] JobFilterDto filter) {
            var userId = User.GetUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });

            filter.ClientId = userId.Value;

            var jobs = await jobService.GetJobsAsync(filter);

            var result = new PaginatedResponse<JobClientSummaryDto> {
                Items = jobs.Items
                    .Select(JobClientSummaryDto.FromJobSummary)
                    .ToList(),
                TotalCount = jobs.TotalCount,
            };

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<JobSummaryDto>> GetJobById(int id) {
            var job = await jobService.GetJobByIdAsync(id);

            if (job == null)
                return NotFound();

            return Ok(job);
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PostJob([FromBody] JobCreateDto dto) {
            try {
                var userId = User.GetUserId();
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });

                await jobService.CreateJobAsync(dto, userId.Value);
                return Created();

            } catch (ArgumentException ex) {
                return BadRequest(new {
                    message = ex.Message
                });

            } catch (Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError, new {
                    message = "An unexpected error occurred."
                });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Client")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateJob(int id, [FromBody] JobUpdateDto dto) {
            try {
                var userId = User.GetUserId();
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });

                await jobService.UpdateJobAsync(id, dto, userId.Value);
                return NoContent();

            } catch (KeyNotFoundException ex) {
                return NotFound(new { message = ex.Message });
            
            } catch (ArgumentException ex) {
                return BadRequest(new { message = ex.Message });

            } catch (InvalidOperationException ex) {
                return BadRequest(new { message = ex.Message });

            } catch (Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError, new {
                    message = "An unexpected error occurred."
                });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Client")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteJob(int id) {
            try {
                var userId = User.GetUserId();
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });

                await jobService.DeleteJobAsync(id, userId.Value);
                return NoContent();

            } catch (KeyNotFoundException ex) {
                return NotFound(new { message = ex.Message });

            } catch (ArgumentException ex) {
                return BadRequest(new { message = ex.Message });

            } catch (InvalidOperationException ex) {
                return BadRequest(new { message = ex.Message });

            } catch (Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError, new {
                    message = "An unexpected error occurred."
                });
            }
        }

        [HttpGet("{jobId:int}/applications")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<List<JobApplicationClientDto>>> GetJobApplications(int jobId)
        {
            var userId = User.GetUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });

            var job = await appDbContext.Jobs.FirstOrDefaultAsync(j => j.Id == jobId);
            if (job == null)
                return NotFound(new { message = "Job not found." });

            if (job.ClientId != userId.Value)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You are not allowed to view applications for this job." });

            var applications = await appDbContext.Applications
                .Where(a => a.JobId == jobId && a.AppStatus != AppStatus.Draft)
                .Include(a => a.Freelancer)
                    .ThenInclude(f => f.User)
                .Select(a => new JobApplicationClientDto
                {
                    FreelancerId = a.FreelancerId,
                    FreelancerName = a.Freelancer.User.UserName,
                    CoverLetter = a.CoverLetter,
                    Bid = a.Bid,
                    Timeline = a.Timeline,
                    AppStatus = a.AppStatus
                })
                .ToListAsync();

            return Ok(applications);
        }

        [HttpPut("{jobId:int}/applications/{freelancerId:int}/hire")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> HireFreelancer(int jobId, int freelancerId)
        {
            var userId = User.GetUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });

            try
            {
                await jobStatusService.HireFreelancerAsync(jobId, freelancerId, userId.Value);
                return Ok(new { message = "Freelancer hired successfully. Job is now in progress." });
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

        [HttpPut("{jobId:int}/finish")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> FinishJob(int jobId)
        {
            var userId = User.GetUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });

            try
            {
                await jobStatusService.FinishJobAsync(jobId, userId.Value);
                return Ok(new { message = "Job marked as finished." });
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

        [HttpPut("{jobId:int}/delay")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> MarkJobDelayed(int jobId)
        {
            var userId = User.GetUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });

            try
            {
                await jobStatusService.MarkDelayedAsync(jobId, userId.Value);
                return Ok(new { message = "Job marked as delayed." });
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

        [HttpPut("{jobId:int}/pass")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> MarkJobPassed(int jobId)
        {
            var userId = User.GetUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });

            try
            {
                await jobStatusService.MarkPassedAsync(jobId, userId.Value);
                return Ok(new { message = "Job marked as passed (deadline expired)." });
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
