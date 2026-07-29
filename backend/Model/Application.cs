using backend.Model;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace backend.Model {
    public class Application {
        [Required(ErrorMessage = "Job ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Job ID must be a positive number")]
        public int JobId { get; set; }

        [Required(ErrorMessage = "Freelancer ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Freelancer ID must be a positive number")]
        public int FreelancerId { get; set; }

        [Required(ErrorMessage = "Cover letter is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Cover letter must be between 10 and 2000 characters")]
        public required string CoverLetter { get; set; }

        [Required(ErrorMessage = "Bid amount is required")]
        [Range(1, 1000000, ErrorMessage = "Bid must be between $1 and $1,000,000")]
        [DataType(DataType.Currency)]
        public int Bid { get; set; }

        [Required(ErrorMessage = "Timeline is required")]
        [Range(1, 365, ErrorMessage = "Timeline must be between 1 and 365 days")]
        public int Timeline { get; set; }

        [Required(ErrorMessage = "Application status is required")]
        [EnumDataType(typeof(AppStatus), ErrorMessage = "Invalid application status")]
        public AppStatus AppStatus { get; set; }

        [Required(ErrorMessage = "Job information is required")]
        [JsonIgnore]
        public required Job Job { get; set; }

        [Required(ErrorMessage = "Freelancer information is required")]
        [JsonIgnore]
        public required Freelancer Freelancer { get; set; }

        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}