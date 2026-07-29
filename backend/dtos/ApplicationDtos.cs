using System.ComponentModel.DataAnnotations;
using backend.Model;

namespace backend.DTOs {
    public class ApplicationCreateDto {
        public int JobId { get; set; }
        public string CoverLetter { get; set; } = string.Empty;
        public decimal Bid { get; set; }
        public int TimelineDays { get; set; }
    }

    public class ApplicationStatusUpdateDto
    {
        public string? AppStatus { get; set; }
    }

    public class ApplyJobDto
    {
        [Required(ErrorMessage = "Job ID is required.")]
        public int JobId { get; set; }

        [Required(ErrorMessage = "Cover letter is required.")]
        [StringLength(3000, MinimumLength = 20, ErrorMessage = "Cover letter must be between 20 and 3000 characters.")]
        public string CoverLetter { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bid amount is required.")]
        [Range(1, 10_000_000, ErrorMessage = "Bid amount must be greater than 0.")]
        public int Bid { get; set; }

        [Required(ErrorMessage = "Timeline (in days) is required.")]
        [Range(1, 365, ErrorMessage = "Timeline must be between 1 and 365 days.")]
        public int Timeline { get; set; }
    }

    public class ApplicationResponseDto
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public int JobBudget { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyLogo { get; set; }
        public string CoverLetter { get; set; } = string.Empty;
        public int Bid { get; set; }
        public int Timeline { get; set; }
        public AppStatus AppStatus { get; set; }
        public DateOnly JobDeadline { get; set; }
    }
}
