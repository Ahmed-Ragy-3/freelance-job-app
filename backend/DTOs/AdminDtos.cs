using System.ComponentModel.DataAnnotations;
using backend.Model;

namespace backend.DTOs
{
    public class AdminUserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Role Role { get; set; }
        public bool IsSuspended { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SuspendUserDto
    {
        [Required]
        public bool IsSuspended { get; set; }
    }

    public class CreateNamedEntityDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;
    }

    public class PendingApplicationAdminDto
    {
        public int JobId { get; set; }
        public int FreelancerId { get; set; }
        public string FreelancerName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string ClientCompanyName { get; set; } = string.Empty;
        public string CoverLetter { get; set; } = string.Empty;
        public int Bid { get; set; }
        public int Timeline { get; set; }
        public AppStatus AppStatus { get; set; }
    }

    public class AdminSkillDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
