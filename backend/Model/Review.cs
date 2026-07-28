using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Model {
    [Table("Reviews")]
    public class Review {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Job))]
        public int JobId { get; set; }

        public Job Job { get; set; } = null!;

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rate { get; set; }

        [StringLength(1000, MinimumLength = 5,
            ErrorMessage = "Comment must be between 5 and 1000 characters.")]
        public string? Comment { get; set; }
    }
}