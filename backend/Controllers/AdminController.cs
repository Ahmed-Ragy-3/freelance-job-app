using backend.Model;
using backend.DTOs;
using backend.Services;
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
        private readonly JobStatusService _jobStatusService;
        private readonly CategoryService _categoryService;

        public AdminController(AppDbContext context, JobStatusService jobStatusService, CategoryService categoryService)
        {
            _context = context;
            _jobStatusService = jobStatusService;
            _categoryService = categoryService;
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

        [HttpGet("jobs/pending")]
        public async Task<ActionResult<List<PendingJobAdminDto>>> GetPendingJobs()
        {
            var pendingJobs = await _context.Jobs
                .Where(j => j.JobStatus == JobStatus.Pending)
                .OrderByDescending(j => j.PostedAt)
                .Select(j => new PendingJobAdminDto
                {
                    Id = j.Id,
                    Title = j.Title,
                    Budget = j.Budget,
                    Deadline = j.Deadline,
                    PostedAt = j.PostedAt,
                    JobStatus = j.JobStatus,
                    ClientCompanyName = _context.Clients
                        .Where(c => c.UserId == j.ClientId)
                        .Select(c => c.CompanyName)
                        .FirstOrDefault() ?? string.Empty
                })
                .ToListAsync();

            return Ok(pendingJobs);
        }

        [HttpPut("jobs/{jobId:int}/approve")]
        public async Task<IActionResult> ApproveJob(int jobId)
        {
            try
            {
                await _jobStatusService.ApproveJobAsync(jobId);
                return Ok(new { message = "Job approved successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("jobs/{jobId:int}/reject")]
        public async Task<IActionResult> RejectJob(int jobId)
        {
            try
            {
                await _jobStatusService.RejectJobAsync(jobId);
                return Ok(new { message = "Job rejected successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("jobs")]
        public async Task<IActionResult> GetAllJobs()
        {
            var jobs = await _context.Jobs
                .Include(j => j.Applications)
                .Include(j => j.Categories).ThenInclude(jc => jc.Category)
                .Include(j => j.Tags).ThenInclude(jt => jt.Tag)
                .OrderByDescending(j => j.PostedAt)
                .ToListAsync();

            return Ok(jobs.Select(JobSummaryDto.FromJob).ToList());
        }

        [HttpDelete("jobs/{jobId:int}")]
        public async Task<IActionResult> DeleteJob(int jobId)
        {
            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == jobId);
            if (job == null)
                return NotFound(new { message = $"Job with ID {jobId} was not found." });

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Job deleted successfully." });
        }

        [HttpDelete("users/{userId:int}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return NotFound(new { message = $"User with ID {userId} was not found." });

            if (user.Role == Role.Admin)
                return BadRequest(new { message = "Admin accounts cannot be deleted." });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "User deleted successfully." });
        }

        [HttpPut("tags/{tagId:int}")]
        public async Task<IActionResult> UpdateTag(int tagId, [FromBody] CreateNamedEntityDto dto)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == tagId);
            if (tag == null)
                return NotFound(new { message = $"Tag with ID {tagId} was not found." });

            var normalizedName = dto.Name.Trim();
            if (string.IsNullOrWhiteSpace(normalizedName))
                return BadRequest(new { message = "Tag name is required." });

            var exists = await _context.Tags.AnyAsync(t => t.Name == normalizedName && t.Id != tagId);
            if (exists)
                return BadRequest(new { message = $"Tag '{normalizedName}' already exists." });

            tag.Name = normalizedName;
            await _context.SaveChangesAsync();
            return Ok(new TagResponseDto { Id = tag.Id, Name = tag.Name });
        }

        [HttpPut("skills/{skillId:int}")]
        public async Task<IActionResult> UpdateSkill(int skillId, [FromBody] CreateNamedEntityDto dto)
        {
            var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == skillId);
            if (skill == null)
                return NotFound(new { message = $"Skill with ID {skillId} was not found." });

            var normalizedName = dto.Name.Trim();
            if (string.IsNullOrWhiteSpace(normalizedName))
                return BadRequest(new { message = "Skill name is required." });

            var exists = await _context.Skills.AnyAsync(s => s.Name == normalizedName && s.Id != skillId);
            if (exists)
                return BadRequest(new { message = $"Skill '{normalizedName}' already exists." });

            skill.Name = normalizedName;
            await _context.SaveChangesAsync();
            return Ok(new { id = skill.Id, name = skill.Name });
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<CategorySummaryDto>>> GetCategories()
        {
            return Ok(await _categoryService.GetAllCategoriesAsync());
        }

        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateNamedEntityDto dto)
        {
            try
            {
                var created = await _categoryService.CreateAsync(dto.Name);
                return Ok(created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("categories/{categoryId:int}")]
        public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] CreateNamedEntityDto dto)
        {
            try
            {
                var updated = await _categoryService.UpdateAsync(categoryId, dto.Name);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("categories/{categoryId:int}")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            try
            {
                await _categoryService.DeleteAsync(categoryId);
                return Ok(new { message = "Category deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}