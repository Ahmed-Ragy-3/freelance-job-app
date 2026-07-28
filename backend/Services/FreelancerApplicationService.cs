using backend.Dtos;
using backend.Model;
using backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class FreelancerApplicationService : IFreelancerApplicationService
    {
        private readonly IFreelancerApplicationRepository _repository;
        private readonly AppDbContext _context;

        public FreelancerApplicationService(IFreelancerApplicationRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<List<ApplicationResponseDto>> GetMyApplicationsAsync(int freelancerId)
        {
            var applications = await _repository.GetApplicationsByFreelancerIdAsync(freelancerId);

            // Extract client info for mapping company details
            var clientIds = applications.Select(a => a.Job.ClientId).Distinct().ToList();
            var clients = await _context.Clients
                .Where(c => clientIds.Contains(c.UserId))
                .ToDictionaryAsync(c => c.UserId);

            return applications.Select(a =>
            {
                clients.TryGetValue(a.Job.ClientId, out var client);

                return new ApplicationResponseDto
                {
                    JobId = a.JobId,
                    JobTitle = a.Job.Title,
                    JobBudget = a.Job.Budget,
                    CompanyName = client?.CompanyName ?? string.Empty,
                    CompanyLogo = client?.Logo,
                    CoverLetter = a.CoverLetter,
                    Bid = a.Bid,
                    Timeline = a.Timeline,
                    AppStatus = a.AppStatus,
                    JobDeadline = a.Job.Deadline
                };
            }).ToList();
        }

        public async Task<ApplicationResponseDto> ApplyToJobAsync(int freelancerId, ApplyJobDto dto)
        {
            var job = await _repository.GetJobByIdAsync(dto.JobId);
            if (job == null)
            {
                throw new KeyNotFoundException($"Job with ID {dto.JobId} was not found.");
            }

            // Verify job status allows receiving applications (Approved)
            if (job.JobStatus != JobStatus.Approved)
            {
                throw new InvalidOperationException("This job is not currently open for applications.");
            }

            // Check for duplicate active application
            bool alreadyApplied = await _repository.HasAlreadyAppliedAsync(freelancerId, dto.JobId);
            if (alreadyApplied)
            {
                throw new InvalidOperationException("You have already submitted an active application for this job.");
            }

            var newApplication = new Application
            {
                JobId = dto.JobId,
                FreelancerId = freelancerId,
                CoverLetter = dto.CoverLetter,
                Bid = dto.Bid,
                Timeline = dto.Timeline,
                AppStatus = AppStatus.In_Progress,
                Job = job,
                Freelancer = null! // Handled by EF Core via FreelancerId
            };

            var createdApp = await _repository.CreateApplicationAsync(newApplication);

            // Fetch Client information for response object
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == job.ClientId);

            return new ApplicationResponseDto
            {
                JobId = createdApp.JobId,
                JobTitle = job.Title,
                JobBudget = job.Budget,
                CompanyName = client?.CompanyName ?? string.Empty,
                CompanyLogo = client?.Logo,
                CoverLetter = createdApp.CoverLetter,
                Bid = createdApp.Bid,
                Timeline = createdApp.Timeline,
                AppStatus = createdApp.AppStatus,
                JobDeadline = job.Deadline
            };
        }

        public async Task<bool> WithdrawApplicationAsync(int freelancerId, int jobId)
        {
            var application = await _repository.GetApplicationAsync(jobId, freelancerId);
            if (application == null)
            {
                throw new KeyNotFoundException($"Application for Job ID {jobId} was not found.");
            }

            // Verify application ownership
            if (application.FreelancerId != freelancerId)
            {
                throw new UnauthorizedAccessException("You are not authorized to withdraw this application.");
            }

            // Only applications pending review (In_Progress) can be withdrawn
            if (application.AppStatus != AppStatus.In_Progress)
            {
                throw new InvalidOperationException($"Cannot withdraw application with status '{application.AppStatus}'. Only pending applications can be withdrawn.");
            }

            application.AppStatus = AppStatus.Withdrawn;
            await _repository.UpdateApplicationAsync(application);

            return true;
        }
    }
}
