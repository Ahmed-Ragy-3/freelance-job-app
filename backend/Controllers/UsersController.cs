using backend.Auth;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFreelancerService _freelancerService;
        private readonly IClientService _clientService;

        public UsersController(IUserService userService, IFreelancerService freelancerService, IClientService clientService)
        {
            _userService = userService;
            _freelancerService = freelancerService;
            _clientService = clientService;
        }

        /// Retrieves the basic user profile for the currently logged-in user.
        [HttpGet("me")]
        public async Task<ActionResult<UserProfileDto>> GetMyProfile()
        {
            int? userId = User.GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });
            }

            var profile = await _userService.GetUserProfileAsync(userId.Value);
            if (profile == null)
            {
                return NotFound(new { message = "User record was not found." });
            }

            return Ok(profile);
        }

        /// Updates username and profile image URL for the currently logged-in user.
        [HttpPut("me")]
        public async Task<ActionResult<UserProfileDto>> UpdateMyProfile([FromBody] UpdateUserProfileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int? userId = User.GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });
            }

            try
            {
                var updatedProfile = await _userService.UpdateUserProfileAsync(userId.Value, dto);
                if (updatedProfile == null)
                {
                    return NotFound(new { message = "User record was not found." });
                }

                return Ok(updatedProfile);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// Retrieves a freelancer profile by User ID.
        [HttpGet("freelancer/{userId:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<FreelancerProfileDto>> GetFreelancerProfile(int userId)
        {
            var profile = await _freelancerService.GetProfileByUserIdAsync(userId);
            if (profile == null)
            {
                return NotFound(new { message = $"Freelancer profile for user ID {userId} was not found." });
            }

            return Ok(profile);
        }

        /// Updates a freelancer profile by User ID.
        [HttpPut("freelancer/{userId:int}")]
        [Authorize(Roles = "Freelancer")]
        public async Task<ActionResult<FreelancerProfileDto>> UpdateFreelancerProfile(int userId, [FromBody] UpdateFreelancerProfileDto dto)
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

        /// Retrieves a client profile by User ID.
        [HttpGet("client/{userId:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ClientProfileDto>> GetClientProfile(int userId)
        {
            var profile = await _clientService.GetProfileByUserIdAsync(userId);
            if (profile == null)
            {
                return NotFound(new { message = $"Client profile for user ID {userId} was not found." });
            }

            return Ok(profile);
        }

        /// Updates a client profile by User ID.
        [HttpPut("client/{userId:int}")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<ClientProfileDto>> UpdateClientProfile(int userId, [FromBody] UpdateClientProfileDto dto)
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
                var updatedProfile = await _clientService.UpdateProfileAsync(userId, dto);
                if (updatedProfile == null)
                {
                    return NotFound(new { message = $"Client profile for user ID {userId} was not found." });
                }

                return Ok(updatedProfile);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// Changes the password for the currently logged-in user.
        [HttpPut("me/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            // TODO: Implement password verification and hashing logic in dedicated auth/user service
            return StatusCode(StatusCodes.Status501NotImplemented, new { message = "Password change functionality will be implemented in a future update." });
        }
    }
}