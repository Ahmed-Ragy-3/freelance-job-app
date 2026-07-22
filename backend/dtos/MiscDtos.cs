namespace backend.Dtos
{
    // ---- Review ----
    public class ReviewCreateDto
    {
        public int JobId { get; set; }
        public int Rate { get; set; } // e.g. 1-5
        public string Comment { get; set; }
    }

    public class ReviewResponseDto
    {
        public int JobId { get; set; }
        public int Rate { get; set; }
        public string Comment { get; set; }
    }

    // ---- Bookmark ----
    public class BookmarkCreateDto
    {
        public int JobId { get; set; }
    }

    public class BookmarkResponseDto
    {
        public JobSummaryDto Job { get; set; }
    }

    // ---- Notification ----
    public class NotificationResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Read { get; set; }
    }

    // ---- Attachment ----
    public class AttachmentCreateDto
    {
        public int JobId { get; set; }
        // actual file comes via IFormFile in the controller, not in this DTO
        public string FileName { get; set; }
        public string Type { get; set; }
    }

    public class AttachmentResponseDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string FileName { get; set; }
        public string Type { get; set; }
    }
}
