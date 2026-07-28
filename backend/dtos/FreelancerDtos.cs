namespace backend.Dtos
{
    public class SkillResponseDto
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public int ExperienceLevel { get; set; }
    }

    public class FreelancerSkillCreateDto
    {
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