
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models {
    [Table("Tags")]
    public class Tag {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Job))]
        public int JobId { get; set; }

        public Job Job { get; set; } = null!;

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(20, ErrorMessage = "Name must be 20 characters maximum.")]
        public int Name { get; set; }
    }
}
