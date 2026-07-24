using backend.model;

namespace backend.model
{
    public class FreelancerSkill
    {
        public int FreelancerId { get; set; }
        public int SkillId { get; set; }
        public int ExperienceLevel { get; set; }

        public Freelancer Freelancer { get; set; } = null!;
        public Skill Skill { get; set; } = null!;
    }
}