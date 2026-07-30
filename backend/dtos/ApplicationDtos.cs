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

        /// <summary>
        /// Optional portfolio/proposal attachments (PDFs or Images, max 10MB per file).
        /// </summary>
        public List<IFormFile>? Attachments { get; set; }
    }

    public class SaveApplicationDraftDto
    {
        [Required(ErrorMessage = "Job ID is required.")]
        public int JobId { get; set; }

        [StringLength(3000, ErrorMessage = "Cover letter cannot exceed 3000 characters.")]
        public string CoverLetter { get; set; } = string.Empty;

        [Range(0, 10_000_000, ErrorMessage = "Bid amount cannot be negative.")]
        public int Bid { get; set; }

        [Range(0, 365, ErrorMessage = "Timeline cannot exceed 365 days.")]
        public int Timeline { get; set; }
    }

    public class SaveApplicationDraftDto
    {
        [Required(ErrorMessage = "Job ID is required.")]
        public int JobId { get; set; }

        [StringLength(3000, ErrorMessage = "Cover letter cannot exceed 3000 characters.")]
        public string CoverLetter { get; set; } = string.Empty;

        [Range(0, 10_000_000, ErrorMessage = "Bid amount cannot be negative.")]
        public int Bid { get; set; }

        [Range(0, 365, ErrorMessage = "Timeline cannot exceed 365 days.")]
        public int Timeline { get; set; }
    }

    public class ApplicationResponseDto
    {
        public int ApplicationId { get; set; }
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
        public List<AttachmentResponseDto> Attachments { get; set; } = new();
        public DateTime? SubmittedAt { get; set; }
    }

    public class JobApplicationClientDto
    {
        public int FreelancerId { get; set; }
        public string FreelancerName { get; set; } = string.Empty;
        public string CoverLetter { get; set; } = string.Empty;
        public int Bid { get; set; }
        public int Timeline { get; set; }
        public AppStatus AppStatus { get; set; }
    }
}
