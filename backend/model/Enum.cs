namespace backend.model
{
    public enum Role
    {
        Admin,
        Freelancer,
        Client
    }
    public enum JobStatus
    {
        Pending,
        Approved,
        Rejected,
        In_Progress,
        Finished,
        Passed,
        Delayed
    }
    public enum AppStatus
    {
        Draft,
        In_Progress,
        Accepted,
        Rejected
    }
}