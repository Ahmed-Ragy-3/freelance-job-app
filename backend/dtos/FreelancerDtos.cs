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

        public static FreelancerSummaryDto FromFreelancer(Freelancer f) {
            var reviews = f.Applications?
                .Where(a => a.Job?.Reviews != null && a.Job.Reviews.Any())
                .SelectMany(a => a.Job!.Reviews)
                .Where(r => r.RevieweeId == f.UserId)
                .Select(r => r.Rate)
                .ToList() ?? new List<int>();

            return new FreelancerSummaryDto {
                UserId = f.UserId,
                UserName = f.User?.UserName,
                ImageUrl = f.User?.ImageUrl,
                AvgRate = reviews.Count > 0
                    ? Math.Round((decimal)reviews.Average(), 2)
                    : 0
            };
        }
    }

    public class UpdateFreelancerProfileDto
    {
        public string Bio { get; set; } = string.Empty;
        public string? Link { get; set; }
        /// <summary>When null, existing skills are left unchanged. Empty list clears skills.</summary>
        public List<FreelancerSkillCreateDto>? Skills { get; set; }
    }
}