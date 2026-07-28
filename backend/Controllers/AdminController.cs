using backend.model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { message = "You are an admin!" });
        }

        [HttpGet("overview/stats")]
        public async Task<IActionResult> GetStats()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalJobs = await _context.Jobs.CountAsync();
            var totalApplications = await _context.Applications.CountAsync();

            // Assumption: revenue = sum of budgets for finished jobs
            var totalRevenue = await _context.Jobs
                .Where(j => j.JobStatus == JobStatus.Finished)
                .SumAsync(j => (int?)j.Budget) ?? 0;

            return Ok(new
            {
                totalUsers,
                totalJobs,
                totalApplications,
                totalRevenue
            });
        }
    }
}