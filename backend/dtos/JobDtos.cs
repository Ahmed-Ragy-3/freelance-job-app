using backend.Model;

namespace backend.Dtos {
    // Sent by a Client when posting a new job
    public class JobCreateDto {
        public string Title { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateOnly Deadline { get; set; }
        public List<int> SkillIds { get; set; } = new List<int>();
        public List<int> CategoryIds { get; set; } = new List<int>();
        public List<int> TagIds { get; set; } = new List<int>();
    }

    // Sent when a Client updates a job they own
    public class JobUpdateDto {
        public string Title { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public string Description { get; set; } = string.Empty; 
        public DateOnly Deadline { get; set; }
        public string JobStatus { get; set; } = string.Empty;  // Pending, Approved, Rejected, InProgress, Finished, Passed, Delayed
        public List<int> SkillIds { get; set; } = new List<int>();
        public List<int> CategoryIds { get; set; } = new List<int>();
        public List<int> TagIds { get; set; } = new List<int>();
    }

    // Returned for job listings / job details page
    public class JobResponseDto {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateOnly Deadline { get; set; }
        public string JobStatus { get; set; } = string.Empty;
        public DateTime PostedAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public ClientSummaryDto Client { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
        public List<string> Categories { get; set; } = new List<string>();
        public List<string> Tags { get; set; } = new List<string>();
        public List<AttachmentResponseDto> Attachments { get; set; } = new List<AttachmentResponseDto>();
    }

    // Lightweight version used inside ApplicationResponseDto / BookmarkResponseDto
    public class JobSummaryDto {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public string JobStatus { get; set; } = string.Empty;
        public DateOnly Deadline { get; set; }
        public List<string> Tags { get; set; } = new List<string>();

        public static JobSummaryDto FromJob(Job job) {
            return new JobSummaryDto {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Budget = job.Budget,
                JobStatus = job.JobStatus.ToString(),
                Deadline = job.Deadline,
                Tags = job.Tags.Select(t => t.Tag.Name).ToList()
            };
        }
    }
}
