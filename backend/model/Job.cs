using System.ComponentModel.DataAnnotations;

namespace backend.model {
    public class Job {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int Budget { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateOnly Deadline { get; set; }

        [Required]
        public JobStatus JobStatus { get; set; }

        [Required]
        public DateTime PostedAt { get; set; }

        public DateTime? AcceptedAt { get; set; }

        public DateTime? FinishedAt { get; set; }

        //[ForeignKey(nameof(Client))]
        public int ClientId { get; set; }

        //public User Client { get; set; } = null!;
    }
}
