namespace backend.DTOs
{
    // Sent when a Client completes/updates their company profile
    public class ClientCreateDto
    {
        public string CompanyName { get; set; }
        public string CompanyDetails { get; set; }
        public string Logo { get; set; }
    }

    // Returned when viewing a client's profile (e.g. on a job posting)
    public class ClientResponseDto
    {
        public int UserId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyDetails { get; set; }
        public string Logo { get; set; }
    }

    // Lightweight version used inside JobResponseDto so we don't send the full client object
    public class ClientSummaryDto
    {
        public int UserId { get; set; }
        public string CompanyName { get; set; }
        public string Logo { get; set; }
    }
}
