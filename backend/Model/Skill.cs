using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace backend.Model {
    public class Skill {
        [Key]
        [Range(1, int.MaxValue, ErrorMessage = "Skill ID must be a positive number")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Skill name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Skill name must be between 2 and 100 characters")]
        [Display(Name = "Skill Name")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Freelancer skills collection is required")]
        [JsonIgnore]
        public required ICollection<FreelancerSkill> FreelancerSkills { get; set; }

        [Required(ErrorMessage = "Job skills collection is required")]
        [JsonIgnore]
        public required ICollection<JobSkill> JobSkills { get; set; }
    }
}