
namespace backend.Models
{
    public class JobSkill
    {
        public int JobId { get; set; }
        public int SkillId { get; set; }

        public Job Job { get; set; }
        public Skill Skill { get; set; }
    }
}