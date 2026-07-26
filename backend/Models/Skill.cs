using System.Collections.Generic;

namespace backend.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<FreelancerSkill> FreelancerSkills { get; set; }
        public ICollection<JobSkill> JobSkills { get; set; }
    }
}