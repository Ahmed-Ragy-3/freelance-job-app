using backend.DTOs;
using backend.Model;
using backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class FreelancerService : IFreelancerService
    {
        private readonly IFreelancerRepository _freelancerRepository;
        private readonly AppDbContext _appDbContext;

        public FreelancerService(IFreelancerRepository freelancerRepository, AppDbContext appDbContext)
        {
            _freelancerRepository = freelancerRepository;
            _appDbContext = appDbContext;
        }

        public async Task<FreelancerProfileDto?> GetProfileByUserIdAsync(int userId)
        {
            var freelancer = await _freelancerRepository.GetByUserIdAsync(userId);
            if (freelancer == null)
            {
                return null;
            }

            var skills = await _freelancerRepository.GetFreelancerSkillsAsync(userId);
            var avgRate = await _freelancerRepository.GetAverageRatingAsync(userId);

            return MapToProfileDto(freelancer, skills, avgRate);
        }

        public async Task<FreelancerProfileDto?> UpdateProfileAsync(int userId, UpdateFreelancerProfileDto dto)
        {
            var freelancer = await _freelancerRepository.GetByUserIdAsync(userId);
            if (freelancer == null)
            {
                return null;
            }

            // Validate all skill IDs exist in database
            if (dto.Skills.Any())
            {
                var skillIds = dto.Skills.Select(s => s.SkillId);
                var validSkills = await _freelancerRepository.SkillsExistAsync(skillIds);
                if (!validSkills)
                {
                    throw new ArgumentException("One or more provided Skill IDs do not exist.");
                }
            }

            // Update core properties
            freelancer.Bio = dto.Bio;
            freelancer.Link = dto.Link;

            // Map DTO skills to entity skills
            var newSkillsList = dto.Skills.Select(s => new FreelancerSkill
            {
                FreelancerId = userId,
                SkillId = s.SkillId,
                ExperienceLevel = s.ExperienceLevel
            }).ToList();

            await _freelancerRepository.UpdateProfileAsync(freelancer, newSkillsList);

            // Fetch refreshed data for returning updated DTO
            return await GetProfileByUserIdAsync(userId);
        }

        private static FreelancerProfileDto MapToProfileDto(Freelancer freelancer, List<FreelancerSkill> skills, decimal avgRate)
        {
            return new FreelancerProfileDto
            {
                UserId = freelancer.UserId,
                UserName = freelancer.User?.UserName,
                Email = freelancer.User?.Email,
                ImageUrl = freelancer.User?.ImageUrl,
                Bio = freelancer.Bio,
                Link = freelancer.Link,
                AvgRate = avgRate,
                Skills = skills.Select(s => new SkillResponseDto
                {
                    SkillId = s.SkillId,
                    SkillName = s.Skill?.Name ?? string.Empty,
                    ExperienceLevel = s.ExperienceLevel
                }).ToList()
            };
        }
        
         public async Task<List<FreelancerSummaryDto>> GetTopNFreelancersAsync(int n) {
            var freelancers = await _appDbContext.Freelancers
                            .Include(f => f.User)
                            .Include(f => f.Applications)
                                .ThenInclude(a => a.Job)
                                .ThenInclude(j => j.Review)
                            .Include(f => f.FreelancerSkills)
                                .ThenInclude(fs => fs.Skill)
                            .OrderByDescending(f => f.Applications.Count)
                            .Take(n)
                            .ToListAsync();

            return freelancers.Select(FreelancerSummaryDto.FromFreelancer).ToList();
        }

        public async Task<List<FreelancerSummaryDto>> SearchFreelancersAsync(string searchTerm) {
            var freelancers = await _appDbContext.Freelancers
                .Include(f => f.User)
                .Where(f => f.User.UserName.Contains(searchTerm))
                .ToListAsync();

            return freelancers.Select(FreelancerSummaryDto.FromFreelancer).ToList();
        }
    }
}
