using backend.Model;
using backend.Services;

namespace backend.DTOs {
    // Sent by a Client when posting a new job
    public class JobCreateDto {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public DateOnly Deadline { get; set; }
        public List<int> CategoryIds { get; set; } = new List<int>();
        public List<int> SkillIds { get; set; } = new List<int>();
        public List<int> TagIds { get; set; } = new List<int>();
    }

    // Sent when a Client updates a job they own
    public class JobUpdateDto : JobCreateDto {
        public string JobStatus { get; set; } = string.Empty;  // Pending, Approved, Rejected, InProgress, Finished, Passed, Delayed
    }

    // Job summary DTO for client page
    public class JobClientSummaryDto {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public string JobStatus { get; set; } = string.Empty;
        public DateTime PostedAt { get; set; }
        public int Applicants { get; set; }

        public static JobClientSummaryDto FromJob(Job job) {
            return new JobClientSummaryDto {
                Id = job.Id,
                Title = job.Title,
                Budget = job.Budget,
                JobStatus = job.JobStatus.ToString(),
                PostedAt = job.PostedAt,
                Applicants = job.Applications.Count
            };
        }

        public static JobClientSummaryDto FromJobSummary(JobSummaryDto job) {
            return new JobClientSummaryDto {
                Id = job.Id,
                Title = job.Title,
                Budget = job.Budget,
                JobStatus = job.JobStatus,
                PostedAt = job.PostedAt,
                Applicants = job.Applicants
            };
        }
    }

    // Lightweight version used inside ApplicationResponseDto / BookmarkResponseDto
    public class JobSummaryDto : JobClientSummaryDto {
        public string Description { get; set; } = string.Empty;
        public DateOnly Deadline { get; set; }
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Tag> Tags { get; set; } = new List<Tag>();

        public static new JobSummaryDto FromJob(Job job) {
            return new JobSummaryDto {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Budget = job.Budget,
                JobStatus = job.JobStatus.ToString(),
                Deadline = job.Deadline,
                PostedAt = job.PostedAt,
                Categories = job.Categories.Select(c => c.Category).ToList(),
                Tags = job.Tags.Select(t => t.Tag).ToList(),
                Applicants = job.Applications.Count
            };
        }
    }


    public class JobDto : JobSummaryDto {
        public ClientSummaryDto Client { get; set; }
        public List<Skill> Skills { get; set; } = new List<Skill>();
        public List<string> Attachments { get; set; } = new List<string>();

        public static new JobDto FromJob(Job job) {
            return new JobDto {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Budget = job.Budget,
                JobStatus = job.JobStatus.ToString(),
                Deadline = job.Deadline,
                PostedAt = job.PostedAt,
                Categories = job.Categories.Select(c => c.Category).ToList(),
                Tags = job.Tags.Select(t => t.Tag).ToList(),
                Applicants = job.Applications.Count,
                Client = ClientSummaryDto.FromClient(job.Client),
                Skills = job.Skills.Select(s => s.Skill).ToList(),
                Attachments = job.Attachments.Select(a => a.Url).ToList()
            };
        }
    }

    public class JobFilterDto {
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public JobStatus? Status { get; set; }
        public List<int>? SkillIds { get; set; }
        public decimal? MinBudget { get; set; }
        public decimal? MaxBudget { get; set; }
        public int? ClientId { get; set; }
        public JobSortBy SortBy { get; set; } = JobSortBy.Newest;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 9;
    }
}
