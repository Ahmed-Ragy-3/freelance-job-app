using System.Collections.Generic;

namespace backend.model
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<FreelancerSkill> FreelancerSkills { get; set; }
        public ICollection<JobSkill> JobSkills { get; set; }
    }
}