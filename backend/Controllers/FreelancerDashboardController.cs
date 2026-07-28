using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            int? userId = GetUserIdFromClaims();
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });
            }

            var dashboardData = await _dashboardService.GetDashboardOverviewAsync(userId.Value);
            return Ok(dashboardData);
        }

        /// Helper method to extract the UserId from JWT claims.
        /// Configured for standard JWT ClaimTypes or custom 'id'/'userId' claims.
        private int? GetUserIdFromClaims()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                     ?? User.FindFirst("id")
                     ?? User.FindFirst("userId");

            if (claim != null && int.TryParse(claim.Value, out int userId))
            {
                return userId;
            }

            return null;
        }
    }
}