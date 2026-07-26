using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models {
    [Table("Clients")]
    public class Client {
        [Key]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters.")]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Company details cannot exceed 2000 characters.")]
        public string? CompanyDetails { get; set; }

        [Url(ErrorMessage = "Invalid logo URL format.")]
        [StringLength(500, ErrorMessage = "Logo URL is too long.")]
        public string? Logo { get; set; }
        public User User { get; set; } = null!;

        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
