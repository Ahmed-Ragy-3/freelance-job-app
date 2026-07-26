namespace backend.Models
{
    public class FreelancerSkill
    {
        public int FreelancerId { get; set; }
        public int SkillId { get; set; }
        public int ExperienceLevel { get; set; }

        public Freelancer Freelancer { get; set; }
        public Skill Skill { get; set; }
    }
}