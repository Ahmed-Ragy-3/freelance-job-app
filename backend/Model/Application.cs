using backend.model;

namespace backend.model
{
    public class Application
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int FreelancerId { get; set; }
        public required string CoverLetter { get; set; }
        public int Bid { get; set; }
        public int Timeline { get; set; }
        public AppStatus AppStatus { get; set; }

        public required Job Job { get; set; }
        public required Freelancer Freelancer { get; set; }
    }
}