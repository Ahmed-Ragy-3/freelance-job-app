namespace backend.Model {
    public enum Role {
        Admin,
        Freelancer,
        Client
    }
    public enum JobStatus {
        Pending,
        Approved,
        Rejected,
        In_Progress,
        Finished,
        Passed,
        Delayed
    }
    public enum JobSortBy {
        Newest,
        Oldest,
        HighestBudget,
        MostApplicants,
        ClosestDeadline
    }

    public enum AppStatus {
        Draft,
        In_Progress,
        Accepted,
        Rejected,
        Withdrawn
    }

    public enum NotificationType {
        ApplicationReceived,
        ApplicationAccepted,
        ApplicationRejected,
        
        JobApproved,
        JobEdited,
        JobCompleted,
        ReviewReceived
    }
}