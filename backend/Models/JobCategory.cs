using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models {
    [Table("JobCategories")]
    public class JobCategory {
        [Required]
        [ForeignKey(nameof(Job))]
        public int JobId { get; set; }

        public Job Job { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;
    }
}