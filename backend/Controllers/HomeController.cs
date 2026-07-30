using backend.dtos;
using backend.DTOs;
using backend.Model;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers {
    [ApiController]
    [Route("api/home")]
    public class HomeController(HomeService homeService, AppDbContext appDbContext, CategoryService categoryService) : ControllerBase {

        [HttpGet("stats")]
        [ProducesResponseType(typeof(HomeDtos), 200)]
        public async Task<ActionResult<HomeDtos>> GetHomeStats() {
            var homeStats = await homeService.GetHomeStatsAsync();
            return Ok(homeStats);
        }
        
        [HttpGet("search")]
        [ProducesResponseType(typeof(GlobalSearchDtos), 200)]
        public async Task<ActionResult<GlobalSearchDtos>> GlobalSearch([FromQuery] string searchTerm) {
            var globalSearchStats = await homeService.GetGlobalSearchStatsAsync(searchTerm ?? "");
            return Ok(globalSearchStats);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<CategorySummaryDto>>> GetCategories() {
            return Ok(await categoryService.GetAllCategoriesAsync());
        }

        [HttpGet("skills")]
        public async Task<ActionResult<List<object>>> GetSkills() {
            var skills = await appDbContext.Skills
                .OrderBy(s => s.Name)
                .Select(s => new { id = s.Id, name = s.Name })
                .ToListAsync();
            return Ok(skills);
        }

        [HttpGet("tags")]
        public async Task<ActionResult<List<TagResponseDto>>> GetTags() {
            var tags = await appDbContext.Tags
                .OrderBy(t => t.Name)
                .Select(t => new TagResponseDto { Id = t.Id, Name = t.Name })
                .ToListAsync();
            return Ok(tags);
        }
    }
}
