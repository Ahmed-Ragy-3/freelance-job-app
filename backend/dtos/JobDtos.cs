namespace backend.Dtos
{
    // Sent by a Client when posting a new job
    public class JobCreateDto
    {
        public string Title { get; set; }
        public decimal Budget { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public List<int> SkillIds { get; set; }
        public List<int> CategoryIds { get; set; }
        public List<int> TagIds { get; set; }
    }

    // Sent when a Client updates a job they own
    public class JobUpdateDto
    {
        public string Title { get; set; }
        public decimal Budget { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public string JobStatus { get; set; } // Pending, Approved, Rejected, InProgress, Finished, Passed, Delayed
        public List<int> SkillIds { get; set; }
        public List<int> CategoryIds { get; set; }
        public List<int> TagIds { get; set; }
    }

    // Returned for job listings / job details page
    public class JobResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Budget { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public string JobStatus { get; set; }
        public DateTime PostedAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public ClientSummaryDto Client { get; set; }
        public List<string> Skills { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Tags { get; set; }
        public List<AttachmentResponseDto> Attachments { get; set; }
    }

    // Lightweight version used inside ApplicationResponseDto / BookmarkResponseDto
    public class JobSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Budget { get; set; }
        public string JobStatus { get; set; }
    }
}
