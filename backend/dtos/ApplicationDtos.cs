namespace backend.DTOs
{
    // Sent by a Freelancer when applying to a job
    public class ApplicationCreateDto
    {
        public int JobId { get; set; }
        public string CoverLetter { get; set; }
        public decimal Bid { get; set; }
        public int TimelineDays { get; set; }
    }

    // Sent by a Client to accept/reject an application
    public class ApplicationStatusUpdateDto
    {
        public string AppStatus { get; set; } // Draft, InProgress, Accepted, Rejected
    }

    // Returned when viewing applications (e.g. Client viewing applicants for their job)
    public class ApplicationResponseDto
    {
        public int Id { get; set; }
        public string CoverLetter { get; set; }
        public decimal Bid { get; set; }
        public int TimelineDays { get; set; }
        public string AppStatus { get; set; }
        public FreelancerSummaryDto Freelancer { get; set; }
        public JobSummaryDto Job { get; set; }
    }
}
