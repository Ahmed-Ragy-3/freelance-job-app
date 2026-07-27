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
    }
}
