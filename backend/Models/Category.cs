using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models {
    [Table("Categories")]
    public class Category {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200, ErrorMessage = "Category name shouldn't exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        public ICollection<JobCategory> JobCategories { get; set; } = new List<JobCategory>();
    }
}
