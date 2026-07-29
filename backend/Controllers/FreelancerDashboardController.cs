using backend.DTOs;
using backend.Auth;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/freelancer/dashboard")]
    [Authorize(Roles = "Freelancer")]
    public class FreelancerDashboardController : ControllerBase
    {
        private readonly IFreelancerDashboardService _dashboardService;

        public FreelancerDashboardController(IFreelancerDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// Retrieves overview metrics and recent applications for the logged-in freelancer.
        [HttpGet]
        public async Task<ActionResult<FreelancerDashboardDto>> GetDashboardOverview()
        {
            int? userId = User.GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });
            }

            var dashboardData = await _dashboardService.GetDashboardOverviewAsync(userId.Value);
            return Ok(dashboardData);
        }

    }
}