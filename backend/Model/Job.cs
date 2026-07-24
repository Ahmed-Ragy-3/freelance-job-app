using backend.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.model {
    public class Job {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Job title is required.")]
        [StringLength(200, MinimumLength = 5,
            ErrorMessage = "Job title must be between 5 and 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Budget is required.")]
        [Range(1, 10_000_000, ErrorMessage = "Budget must be greater than 0.")]
        public int Budget { get; set; }

        [Required(ErrorMessage = "Job description is required.")]
        [StringLength(5000, MinimumLength = 20, ErrorMessage = "Description must be between 20 and 5000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Deadline is required.")]
        public DateOnly Deadline { get; set; }

        [Required]
        public JobStatus JobStatus { get; set; }

        [Required]
        public DateTime PostedAt { get; set; }

        public DateTime? AcceptedAt { get; set; }

        public DateTime? FinishedAt { get; set; }

        [Required(ErrorMessage = "Client is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid client.")]
        [ForeignKey(nameof(Client))]
        public int ClientId { get; set; }

        public User Client { get; set; } = null!;

        public Review? Review { get; set; }

        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        
        public ICollection<JobTag> Tags { get; set; } = new List<JobTag>();

        public ICollection<JobCategory> Categories { get; set; } = new List<JobCategory>();
        
        public ICollection<JobSkill> Skills { get; set; } = new List<JobSkill>();
    }
}