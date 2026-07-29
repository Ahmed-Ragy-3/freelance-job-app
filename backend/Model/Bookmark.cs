using backend.Model;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace backend.Model {
    public class Bookmark {
        [Key]
        [Range(1, int.MaxValue, ErrorMessage = "Bookmark ID must be a positive number")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Job ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Job ID must be a positive number")]
        public int JobId { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive number")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Job information is required")]
        [JsonIgnore]
        public Job Job { get; set; }

        [Required(ErrorMessage = "User information is required")]
        [JsonIgnore]
        public User User { get; set; }
    }
}