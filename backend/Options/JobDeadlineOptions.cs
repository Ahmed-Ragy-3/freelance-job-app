namespace backend.Options;

public class JobDeadlineOptions
{
    public const string SectionName = "JobDeadline";

    /// <summary>How often the background service checks for overdue jobs, in minutes.</summary>
    public int CheckIntervalMinutes { get; set; } = 60;
}
