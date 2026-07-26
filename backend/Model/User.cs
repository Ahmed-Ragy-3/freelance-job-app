using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Model {
    [Table("Users")]
    public class User {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public required string Password { get; set; }

        [Url(ErrorMessage = "Invalid image URL format.")]
        [StringLength(500, ErrorMessage = "Image URL is too long.")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "User role is required.")]
        [EnumDataType(typeof(Role), ErrorMessage = "Invalid Role value provided.")]
        public Role Role { get; set; } = Role.Freelancer;

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public Freelancer? Freelancer { get; set; }
        public Client? Client { get; set; }
    }
}