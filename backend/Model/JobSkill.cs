using backend.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Model {
    public class JobSkill {
        [ForeignKey(nameof(Job))]
        [Required(ErrorMessage = "Job ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Job ID must be a positive number")]
        public int JobId { get; set; }

        [ForeignKey(nameof(Skill))]
        [Required(ErrorMessage = "Skill ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Skill ID must be a positive number")]
        public int SkillId { get; set; }

        [Required(ErrorMessage = "Job information is required")]
        [JsonIgnore]
        public Job Job { get; set; } = null!;

        [Required(ErrorMessage = "Skill information is required")]
        [JsonIgnore]
        public Skill Skill { get; set; } = null!;
    }
}