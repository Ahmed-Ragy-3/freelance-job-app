using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<FreelancerProfileDto>> UpdateProfile(int userId, [FromBody] UpdateFreelancerProfileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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