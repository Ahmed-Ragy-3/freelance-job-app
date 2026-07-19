using backend.model;

namespace backend.model
{
    public class Bookmark
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int FreelancerId { get; set; }

        public Job Job { get; set; }
        public Freelancer Freelancer { get; set; }
    }
}