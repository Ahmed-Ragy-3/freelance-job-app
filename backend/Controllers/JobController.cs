using backend.dtos;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers {
    [ApiController]
    [Route("/jobs")]
    public class JobController(JobService jobService) : ControllerBase {
        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<JobSummaryDto>>> GetJobs([FromQuery] JobFilterDto filter) {
            var jobs = await jobService.GetJobsAsync(filter);

            return Ok(jobs);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<JobSummaryDto>> GetJobById(int id) {
            var job = await jobService.GetJobByIdAsync(id);

            if (job == null)
                return NotFound();

            return Ok(job);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PostJob([FromBody] JobCreateDto dto) {
            try {
                // TODO: Get clientId from the authenticated user
                await jobService.CreateJobAsync(dto, 2);
                return Created();

            } catch (ArgumentException ex) {
                return BadRequest(new {
                    message = ex.Message
                });

            } catch (Exception) {
                // TODO: Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, new {
                    message = "An unexpected error occurred."
                });
            }
        }
    }
}
