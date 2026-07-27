namespace backend.Dtos
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
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // Returned after successful login/register
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }

    // Returned when viewing a user's basic profile info (never include password)
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}