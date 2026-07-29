using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Auth;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FreelancersController : ControllerBase
    {
        private readonly IFreelancerService _freelancerService;

        public FreelancersController(IFreelancerService freelancerService)
        {
            _freelancerService = freelancerService;
        }

        /// <summary>
        /// Retrieves a freelancer profile by User ID.
        /// </summary>
        [HttpGet("{userId:int}")]
        public async Task<ActionResult<FreelancerProfileDto>> GetProfile(int userId)
        {
            var profile = await _freelancerService.GetProfileByUserIdAsync(userId);
            if (profile == null)
            {
                return NotFound(new { message = $"Freelancer profile for user ID {userId} was not found." });
            }

            return Ok(profile);
        }

        /// <summary>
        /// Updates a freelancer profile by User ID.
        /// </summary>
        [HttpPut("{userId:int}")]
        [Authorize(Roles = "Freelancer")]
        public async Task<ActionResult<FreelancerProfileDto>> UpdateProfile(int userId, [FromBody] UpdateFreelancerProfileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tokenUserId = User.GetUserId();
            if (!tokenUserId.HasValue)
                return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });

            if (tokenUserId.Value != userId)
                return Forbid();

            try
            {
                var updatedProfile = await _freelancerService.UpdateProfileAsync(userId, dto);
                if (updatedProfile == null)
                {
                    return NotFound(new { message = $"Freelancer profile for user ID {userId} was not found." });
                }

                return Ok(updatedProfile);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}