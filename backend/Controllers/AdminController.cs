using backend.Model;
using backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { message = "You are an admin!" });
        }

        [HttpGet("overview/stats")]
        public async Task<IActionResult> GetStats()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalJobs = await _context.Jobs.CountAsync();
            var totalApplications = await _context.Applications.CountAsync();

            // Assumption: revenue = sum of budgets for finished jobs
            var totalRevenue = await _context.Jobs
                .Where(j => j.JobStatus == JobStatus.Finished)
                .SumAsync(j => (int?)j.Budget) ?? 0;

            return Ok(new
            {
                totalUsers,
                totalJobs,
                totalApplications,
                totalRevenue
            });
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<AdminUserDto>>> GetAllUsers()
        {
            var users = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new AdminUserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Role = u.Role,
                    IsSuspended = u.IsSuspended,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("users/{userId:int}/suspension")]
        public async Task<IActionResult> SetUserSuspension(int userId, [FromBody] SuspendUserDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound(new { message = $"User with ID {userId} was not found." });
            }

            user.IsSuspended = dto.IsSuspended;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = dto.IsSuspended ? "User suspended successfully." : "User unsuspended successfully.",
                userId = user.Id,
                isSuspended = user.IsSuspended
            });
        }

        [HttpGet("tags")]
        public async Task<ActionResult<List<TagResponseDto>>> GetAllTags()
        {
            var tags = await _context.Tags
                .OrderBy(t => t.Name)
                .Select(t => new TagResponseDto
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToListAsync();

            return Ok(tags);
        }

        [HttpPost("tags")]
        public async Task<ActionResult<TagResponseDto>> AddTag([FromBody] CreateNamedEntityDto dto)
        {
            var normalizedName = dto.Name.Trim();
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                return BadRequest(new { message = "Tag name is required." });
            }
            var exists = await _context.Tags.AnyAsync(t => t.Name == normalizedName);
            if (exists)
            {
                return BadRequest(new { message = $"Tag '{normalizedName}' already exists." });
            }

            var tag = new Tag { Name = normalizedName };
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return Ok(new TagResponseDto { Id = tag.Id, Name = tag.Name });
        }

        [HttpDelete("tags/{tagId:int}")]
        public async Task<IActionResult> DeleteTag(int tagId)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == tagId);
            if (tag == null)
            {
                return NotFound(new { message = $"Tag with ID {tagId} was not found." });
            }

            var usedByJobs = await _context.JobTags.Where(jt => jt.TagId == tagId).ToListAsync();
            if (usedByJobs.Count > 0)
            {
                _context.JobTags.RemoveRange(usedByJobs);
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tag deleted successfully." });
        }

        [HttpGet("skills")]
        public async Task<ActionResult<List<AdminSkillDto>>> GetAllSkills()
        {
            var skills = await _context.Skills
                .OrderBy(s => s.Name)
                .Select(s => new AdminSkillDto
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToListAsync();

            return Ok(skills);
        }

        [HttpPost("skills")]
        public async Task<IActionResult> AddSkill([FromBody] CreateNamedEntityDto dto)
        {
            var normalizedName = dto.Name.Trim();
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                return BadRequest(new { message = "Skill name is required." });
            }
            var exists = await _context.Skills.AnyAsync(s => s.Name == normalizedName);
            if (exists)
            {
                return BadRequest(new { message = $"Skill '{normalizedName}' already exists." });
            }

            var skill = new Skill
            {
                Name = normalizedName,
                FreelancerSkills = new List<FreelancerSkill>(),
                JobSkills = new List<JobSkill>()
            };
            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            return Ok(new { id = skill.Id, name = skill.Name });
        }

        [HttpDelete("skills/{skillId:int}")]
        public async Task<IActionResult> DeleteSkill(int skillId)
        {
            var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == skillId);
            if (skill == null)
            {
                return NotFound(new { message = $"Skill with ID {skillId} was not found." });
            }

            var freelancerLinks = await _context.FreelancerSkills.Where(fs => fs.SkillId == skillId).ToListAsync();
            var jobLinks = await _context.JobSkills.Where(js => js.SkillId == skillId).ToListAsync();

            if (freelancerLinks.Count > 0)
            {
                _context.FreelancerSkills.RemoveRange(freelancerLinks);
            }
            if (jobLinks.Count > 0)
            {
                _context.JobSkills.RemoveRange(jobLinks);
            }

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Skill deleted successfully." });
        }

        [HttpGet("applications/pending")]
        public async Task<ActionResult<List<PendingApplicationAdminDto>>> GetPendingApplications()
        {
            var pendingApplications = await _context.Applications
                .Where(a => a.AppStatus == AppStatus.In_Progress)
                .Include(a => a.Freelancer)
                    .ThenInclude(f => f.User)
                .Include(a => a.Job)
                .Select(a => new PendingApplicationAdminDto
                {
                    JobId = a.JobId,
                    FreelancerId = a.FreelancerId,
                    FreelancerName = a.Freelancer.User.UserName,
                    JobTitle = a.Job.Title,
                    ClientCompanyName = _context.Clients
                        .Where(c => c.UserId == a.Job.ClientId)
                        .Select(c => c.CompanyName)
                        .FirstOrDefault() ?? string.Empty,
                    CoverLetter = a.CoverLetter,
                    Bid = a.Bid,
                    Timeline = a.Timeline,
                    AppStatus = a.AppStatus
                })
                .OrderBy(x => x.JobId)
                .ThenBy(x => x.FreelancerId)
                .ToListAsync();

            return Ok(pendingApplications);
        }

        [HttpPut("applications/{jobId:int}/freelancers/{freelancerId:int}/accept")]
        public async Task<IActionResult> AcceptPendingApplication(int jobId, int freelancerId)
        {
            var application = await _context.Applications
                .FirstOrDefaultAsync(a => a.JobId == jobId && a.FreelancerId == freelancerId);

            if (application == null)
            {
                return NotFound(new { message = "Application not found." });
            }

            if (application.AppStatus != AppStatus.In_Progress)
            {
                return BadRequest(new { message = "Only pending applications can be accepted by admin." });
            }

            application.AppStatus = AppStatus.Accepted;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Application accepted successfully." });
        }

        [HttpDelete("applications/{jobId:int}/freelancers/{freelancerId:int}")]
        public async Task<IActionResult> DeletePendingApplication(int jobId, int freelancerId)
        {
            var application = await _context.Applications
                .FirstOrDefaultAsync(a => a.JobId == jobId && a.FreelancerId == freelancerId);

            if (application == null)
            {
                return NotFound(new { message = "Application not found." });
            }

            if (application.AppStatus != AppStatus.In_Progress)
            {
                return BadRequest(new { message = "Only pending applications can be deleted by admin." });
            }

            application.AppStatus = AppStatus.Rejected;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Pending application rejected successfully." });
        }
    }
}