using backend.Model;

namespace backend.DTOs {
    // Sent when a Freelancer completes/updates their profile
    public class FreelancerCreateDto {
        public string Bio { get; set; }
        public string Link { get; set; }
        public List<FreelancerSkillCreateDto> Skills { get; set; }
    }

    // Sent as part of freelancer profile: which skills + experience level
    public class FreelancerSkillCreateDto {
        public int SkillId { get; set; }
        public int ExperienceLevel { get; set; }
    }

    public class FreelancerProfileDto
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? ImageUrl { get; set; }
        public string Bio { get; set; } = string.Empty;
        public string? Link { get; set; }
        public decimal AvgRate { get; set; }
        public List<SkillResponseDto> Skills { get; set; } = new();
    }

    public class FreelancerSummaryDto
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? ImageUrl { get; set; }
        public decimal AvgRate { get; set; }
    }

    public class UpdateFreelancerProfileDto
    {
        public string Bio { get; set; } = string.Empty;
        public string? Link { get; set; }
        public List<FreelancerSkillCreateDto> Skills { get; set; } = new();
    }
}