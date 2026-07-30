using backend.Model;
using backend.NotificationBuilders;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class JobStatusService(AppDbContext context, NotificationService notificationService)
    {
        public async Task<Job> GetJobOrThrowAsync(int jobId)
        {
            var job = await context.Jobs
                .Include(j => j.Applications)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
                throw new KeyNotFoundException($"Job with ID {jobId} was not found.");

            return job;
        }

        public async Task ApproveJobAsync(int jobId)
        {
            var job = await GetJobOrThrowAsync(jobId);

            if (job.JobStatus != JobStatus.Pending)
                throw new InvalidOperationException($"Only pending jobs can be approved. Current status: {job.JobStatus}.");

            job.JobStatus = JobStatus.Approved;
            await context.SaveChangesAsync();

            var notificationBuilder = new JobApprovedNotificationBuilder(job.ClientId, job.Title);
            await notificationService.SendNotificationAsync(notificationBuilder);
        }

        public async Task RejectJobAsync(int jobId)
        {
            var job = await GetJobOrThrowAsync(jobId);

            if (job.JobStatus != JobStatus.Pending)
                throw new InvalidOperationException($"Only pending jobs can be rejected. Current status: {job.JobStatus}.");

            job.JobStatus = JobStatus.Rejected;
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Client hires a freelancer. Moves job from Approved to In_Progress.
        /// </summary>
        public async Task HireFreelancerAsync(int jobId, int freelancerId, int clientId)
        {
            var job = await GetJobOrThrowAsync(jobId);

            if (job.ClientId != clientId)
                throw new UnauthorizedAccessException("You are not allowed to hire for this job.");

            if (job.JobStatus != JobStatus.Approved)
                throw new InvalidOperationException($"Only approved jobs can have a freelancer hired. Current status: {job.JobStatus}.");

            var application = job.Applications
                .FirstOrDefault(a => a.FreelancerId == freelancerId);

            if (application == null)
                throw new KeyNotFoundException("Application not found for this freelancer.");

            if (application.AppStatus != AppStatus.In_Progress)
                throw new InvalidOperationException("Only pending applications can be accepted.");

            application.AppStatus = AppStatus.Accepted;

            foreach (var other in job.Applications.Where(a => a.FreelancerId != freelancerId && a.AppStatus == AppStatus.In_Progress)) {
                other.AppStatus = AppStatus.Rejected;
                var notificationBuilder1 = new ApplicationRejectedNotificationBuilder(freelancerId, job.Title);
                await notificationService.SendNotificationAsync(notificationBuilder1);
            }

            job.JobStatus = JobStatus.In_Progress;
            job.AcceptedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            var notificationBuilder2 = new ApplicationAcceptedNotificationBuilder(freelancerId, job.Title);
            await notificationService.SendNotificationAsync(notificationBuilder2);   
        }

        public async Task FinishJobAsync(int jobId, int clientId)
        {
            var job = await GetJobOrThrowAsync(jobId);

            if (job.ClientId != clientId)
                throw new UnauthorizedAccessException("You are not allowed to finish this job.");

            if (job.JobStatus is not (JobStatus.In_Progress or JobStatus.Delayed))
                throw new InvalidOperationException($"Only in-progress or delayed jobs can be marked finished. Current status: {job.JobStatus}.");

            var hiredApplication = job.Applications
                .FirstOrDefault(a => a.AppStatus == AppStatus.JobDone || a.AppStatus == AppStatus.Accepted);

            if (hiredApplication == null)
                throw new InvalidOperationException("No active or submitted freelancer application found for this job.");

            job.JobStatus = JobStatus.Finished;
            job.FinishedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
        }

        public async Task MarkDelayedAsync(int jobId, int clientId)
        {
            var job = await GetJobOrThrowAsync(jobId);

            if (job.ClientId != clientId)
                throw new UnauthorizedAccessException("You are not allowed to update this job.");

            if (job.JobStatus != JobStatus.In_Progress)
                throw new InvalidOperationException($"Only in-progress jobs can be marked delayed. Current status: {job.JobStatus}.");

            job.JobStatus = JobStatus.Delayed;

            await context.SaveChangesAsync();
        }

        public async Task MarkPassedAsync(int jobId, int clientId)
        {
            var job = await GetJobOrThrowAsync(jobId);

            if (job.ClientId != clientId)
                throw new UnauthorizedAccessException("You are not allowed to update this job.");

            if (job.JobStatus is not (JobStatus.In_Progress or JobStatus.Delayed))
                throw new InvalidOperationException($"Only in-progress or delayed jobs can be marked passed. Current status: {job.JobStatus}.");

            if (job.Deadline >= DateOnly.FromDateTime(DateTime.UtcNow))
                throw new InvalidOperationException("Job deadline has not passed yet.");

            job.JobStatus = JobStatus.Passed;

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Processes deadline-based job transitions. Called by the background service.
        /// In_Progress → Delayed, Approved (unfilled) → Passed.
        /// </summary>
        public async Task<(int DelayedCount, int PassedCount)> ProcessOverdueJobsAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var overdueInProgress = await context.Jobs
                .Where(j => j.JobStatus == JobStatus.In_Progress && j.Deadline < today)
                .ToListAsync();

            foreach (var job in overdueInProgress)
                job.JobStatus = JobStatus.Delayed;

            var expiredUnfilled = await context.Jobs
                .Where(j => j.JobStatus == JobStatus.Approved && j.Deadline < today)
                .ToListAsync();

            foreach (var job in expiredUnfilled)
                job.JobStatus = JobStatus.Passed;

            var totalUpdated = overdueInProgress.Count + expiredUnfilled.Count;
            if (totalUpdated > 0)
                await context.SaveChangesAsync();

            return (overdueInProgress.Count, expiredUnfilled.Count);
        }
    }
}
