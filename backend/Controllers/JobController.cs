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

        //[HttpPost]
        //[ProducesResponseType(typeof(JobSummaryDto), StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<JobSummaryDto>> CreateJob([FromBody] CreateJobDto dto) {
        //    var job = await jobService.CreateJobAsync(dto);

        //    return CreatedAtAction(
        //        nameof(GetJobById),
        //        new { id = job.Id },
        //        job);
        //}

        [HttpGet("{id:int}")]
        public async Task<ActionResult<JobSummaryDto>> GetJobById(int id) {
            var job = await jobService.GetJobByIdAsync(id);

            if (job == null)
                return NotFound();

            return Ok(job);
        }
    }
}
