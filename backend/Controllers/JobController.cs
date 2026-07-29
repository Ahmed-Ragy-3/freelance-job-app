using backend.dtos;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Auth;

namespace backend.Controllers {
    [ApiController]
    [Route("api/jobs")]
    public class JobController(JobService jobService) : ControllerBase {

        private int GetUserId() {
            var userId = User.GetUserId();
            if (!userId.HasValue)
                throw new UnauthorizedAccessException("Invalid or missing user identity in JWT token.");
            return userId.Value;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<JobSummaryDto>>> GetJobs([FromQuery] JobFilterDto filter) {
            var jobs = await jobService.GetJobsAsync(filter);
            return Ok(jobs);
        }

        [HttpGet("client")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<PaginatedResponse<JobClientSummaryDto>>> GetClientJobs([FromQuery] JobFilterDto filter) {
            filter.ClientId = GetUserId();

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
                await jobService.CreateJobAsync(dto, GetUserId());
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
                await jobService.UpdateJobAsync(id, dto, GetUserId());
                return NoContent();

            } catch (KeyNotFoundException ex) {
                return NotFound(new { message = ex.Message });

            } catch (ArgumentException ex) {
                return BadRequest(new { message = ex.Message });

            } catch (Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError, new {
                    message = "An unexpected error occurred."
                });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Client, Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteJob(int id) {
            try {
                await jobService.DeleteJobAsync(id, GetUserId());
                return NoContent();

            } catch (KeyNotFoundException ex) {
                return NotFound(new { message = ex.Message });

            } catch (ArgumentException ex) {
                return BadRequest(new { message = ex.Message });

            } catch (Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError, new {
                    message = "An unexpected error occurred."
                });
            }
        }
    }
}