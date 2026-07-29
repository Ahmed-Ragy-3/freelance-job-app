using backend.DTOs;
using backend.FileUpload;
using backend.Model;
using backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class FreelancerApplicationService : IFreelancerApplicationService
    {
        private readonly IFreelancerApplicationRepository _repository;
        private readonly IFileUploadService _fileUploadService;
        private readonly IFileValidationService _fileValidationService;
        private readonly AppDbContext _context;

        public FreelancerApplicationService(
            IFreelancerApplicationRepository repository,
            IFileUploadService fileUploadService,
            IFileValidationService fileValidationService,
            AppDbContext context)
        {
            _repository = repository;
            _fileUploadService = fileUploadService;
            _fileValidationService = fileValidationService;
            _context = context;
        }

        public async Task<ApplicationResponseDto> ApplyToJobAsync(int freelancerId, ApplyJobDto dto)
        {
            var job = await _repository.GetJobByIdAsync(dto.JobId);
            if (job == null)
            {
                throw new KeyNotFoundException($"Job with ID {dto.JobId} was not found.");
            }

            if (job.JobStatus != JobStatus.Approved)
            {
                throw new InvalidOperationException("This job is not currently open for applications.");
            }

            bool alreadyApplied = await _repository.HasAlreadyAppliedAsync(freelancerId, dto.JobId);
            if (alreadyApplied)
            {
                throw new InvalidOperationException("You have already submitted an active application for this job.");
            }

            var application = new Application
            {
                JobId = dto.JobId,
                FreelancerId = freelancerId,
                CoverLetter = dto.CoverLetter,
                Bid = dto.Bid,
                Timeline = dto.Timeline,
                AppStatus = AppStatus.In_Progress,
                Job = job,
                Freelancer = null!
            };

            // Process attachment uploads
            if (dto.Attachments != null && dto.Attachments.Any())
            {
                foreach (var file in dto.Attachments)
                {
                    // 1. Validate file size and type (PDF / Images)
                    _fileValidationService.ValidateAttachment(file);

                    // 2. Upload file to Cloudinary
                    string uploadUrl = await _fileUploadService.UploadAsync(file);

                    // 3. Determine attachment type
                    string ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    string fileType = ext == ".pdf" ? "PDF" : "Image";

                    // 4. Attach entity
                    application.Attachments.Add(new Attachment
                    {
                        Url = uploadUrl,
                        FileName = file.FileName,
                        Type = fileType
                    });
                }
            }

            var createdApp = await _repository.CreateApplicationAsync(application);
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
                JobDeadline = job.Deadline,
                Attachments = createdApp.Attachments.Select(att => new AttachmentResponseDto
                {
                    Id = att.Id,
                    Url = att.Url,
                    FileName = att.FileName,
                    Type = att.Type
                }).ToList()
            };
        }

        public async Task<List<ApplicationResponseDto>> GetMyApplicationsAsync(int freelancerId)
        {
            var applications = await _repository.GetApplicationsByFreelancerIdAsync(freelancerId);

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
                    JobDeadline = a.Job.Deadline,
                    Attachments = a.Attachments.Select(att => new AttachmentResponseDto
                    {
                        Id = att.Id,
                        Url = att.Url,
                        FileName = att.FileName,
                        Type = att.Type
                    }).ToList()
                };
            }).ToList();
        }

        public async Task<bool> WithdrawApplicationAsync(int freelancerId, int jobId)
        {
            var application = await _repository.GetApplicationAsync(jobId, freelancerId);
            if (application == null)
            {
                throw new KeyNotFoundException($"Job with ID {jobId} was not found.");
            }

            if (application.FreelancerId != freelancerId)
            {
                throw new UnauthorizedAccessException("You are not authorized to withdraw this application.");
            }

            if (application.AppStatus != AppStatus.In_Progress)
            {
                throw new InvalidOperationException($"Cannot withdraw application with status '{application.AppStatus}'.");
            }

            application.AppStatus = AppStatus.Withdrawn;
            await _repository.UpdateApplicationAsync(application);

            return true;
        }
    }
}
