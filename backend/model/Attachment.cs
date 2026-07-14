using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.model {
    public class Attachment {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Url { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Type { get; set; } = string.Empty;

        [ForeignKey(nameof(Job))]
        public int JobId { get; set; }

        public Job Job { get; set; } = null!;
    }
}
