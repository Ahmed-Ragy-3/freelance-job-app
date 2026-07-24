using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.model {
    [Table("Freelancers")]
    public class Freelancer {
        [Key]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Bio is required.")]
        [StringLength(1000, ErrorMessage = "Bio cannot exceed 1000 characters.")]
        public string Bio { get; set; } = string.Empty;

        [Url(ErrorMessage = "Invalid portfolio link format.")]
        [StringLength(500, ErrorMessage = "Link is too long.")]
        public string? Link { get; set; }

        // Computed property
        [NotMapped]
        public decimal AvgRate { get; set; }

        public User User { get; set; } = null!;

        public ICollection<Application> Applications { get; set; } = new List<Application>();
        public ICollection<FreelancerSkill> FreelancerSkills { get; set; } = new List<FreelancerSkill>();
    }
}
