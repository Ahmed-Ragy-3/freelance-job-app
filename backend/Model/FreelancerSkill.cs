using backend.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Model {
    public class FreelancerSkill {
        [ForeignKey(nameof(Freelancer))]
        [Required(ErrorMessage = "Freelancer ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Freelancer ID must be a positive number")]
        public int FreelancerId { get; set; }

        [ForeignKey(nameof(Skill))]
        [Required(ErrorMessage = "Skill ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Skill ID must be a positive number")]
        public int SkillId { get; set; }

        [Required(ErrorMessage = "Experience level is required")]
        [Range(0, 5, ErrorMessage = "Experience level must be between 0 and 5")]
        [Display(Name = "Experience Level")]
        public int ExperienceLevel { get; set; }

        [Required(ErrorMessage = "Freelancer information is required")]
        [JsonIgnore]
        public Freelancer Freelancer { get; set; } = null!;

        [Required(ErrorMessage = "Skill information is required")]
        [JsonIgnore]
        public Skill Skill { get; set; } = null!;
    }
}