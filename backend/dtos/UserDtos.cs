using System.ComponentModel.DataAnnotations;
using backend.Model;
namespace backend.DTOs
{
    // Sent by frontend when someone signs up
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "Freelancer" or "Client"

        // Freelancer-only field (ignored if Role == "Client")
        public string? Bio { get; set; }

        // Client-only field (ignored if Role == "Freelancer")
        public string? CompanyName { get; set; }
    }

    // Sent by frontend when someone logs in
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // Returned after successful login/register
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    // Returned when viewing a user's basic profile info (never include password)
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserProfileDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; }

        /// Indicates whether the role-specific profile (Freelancer or Client) has been set up.
        public bool IsProfileComplete { get; set; }
    }

    public class UpdateUserProfileDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string UserName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Image URL is too long.")]
        public string? ImageUrl { get; set; }
    }

    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}