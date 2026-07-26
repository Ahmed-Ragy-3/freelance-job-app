using backend.Model;

namespace backend.Dtos {
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

    // Returned when viewing a freelancer's public profile
    public class FreelancerResponseDto {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Bio { get; set; }
        public string Link { get; set; }
        public double AvgRate { get; set; } // computed field
        public List<SkillResponseDto> Skills { get; set; }
    }

    // Lightweight version used inside ApplicationResponseDto
    public class FreelancerSummaryDto {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public double AvgRate { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int NumberOfJobs { get; set; }
        public List<string> Skills { get; set; } = new List<string>();

        public static FreelancerSummaryDto FromFreelancer(Freelancer freelancer) {
            return new FreelancerSummaryDto {
                UserId = freelancer.UserId,
                Username = freelancer.User.UserName,
                Bio = freelancer.Bio,
                AvgRate = freelancer.Applications.Any() ? freelancer.Applications.Average(a => a.Job.Review?.Rate ?? 0) : 0,
                ImageUrl = freelancer.User.ImageUrl,
                NumberOfJobs = freelancer.Applications.Count(),
                Skills = freelancer.FreelancerSkills.Select(fs => fs.Skill.Name).ToList()
            };
        }
    }
}
