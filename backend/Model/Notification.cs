using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Model {
    [Table("Notifications")]
    public class Notification {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; } = null!;

        [Required(ErrorMessage = "Notification title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Notification message is required.")]
        [StringLength(300, ErrorMessage = "Message cannot exceed 300 characters.")]
        public string Message { get; set; } = string.Empty;

        [Required]
        public bool IsRead { get; set; } = false; // Defaults to unread

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}