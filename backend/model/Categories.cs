using System.ComponentModel.DataAnnotations;

namespace backend.model {
    public class Categories {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
    }
}
