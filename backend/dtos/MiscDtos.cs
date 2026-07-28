namespace backend.DTOs
{
    // ---- Review ----
    public class ReviewCreateDto
    {
        public int JobId { get; set; }
        public int Rate { get; set; } // e.g. 1-5
        public string Comment { get; set; } = string.Empty;
    }

    public class ReviewResponseDto
    {
        public int JobId { get; set; }
        public int Rate { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    // ---- Bookmark ----
    public class BookmarkCreateDto
    {
        public int JobId { get; set; }
    }
    public class BookmarkFetchDto {
        public int UserId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 9;
    }

    public class BookmarkResponseDto
    {
        public JobSummaryDto Job { get; set; }
    }

    // ---- Notification ----
    public class NotificationResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool Read { get; set; }
    }

    // ---- Attachment ----
    public class AttachmentCreateDto
    {
        public int JobId { get; set; }
        // actual file comes via IFormFile in the controller, not in this DTO
        public string FileName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    public class AttachmentResponseDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
