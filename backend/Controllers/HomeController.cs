using backend.dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers {
    [ApiController]
    [Route("home")]
    public class HomeController(HomeService homeService) : ControllerBase {

        [HttpGet("stats")]
        [ProducesResponseType(typeof(HomeDtos), 200)]
        public async Task<ActionResult<HomeDtos>> GetHomeStats() {
            var homeStats = await homeService.GetHomeStatsAsync();
            return Ok(homeStats);
        }
        
        [HttpGet("search")]
        [ProducesResponseType(typeof(GlobalSearchDtos), 200)]
        public async Task<ActionResult<GlobalSearchDtos>> GlobalSearch([FromQuery] string searchTerm) {
            var globalSearchStats = await homeService.GetGlobalSearchStatsAsync(searchTerm);
            return Ok(globalSearchStats);
        }
    }
}
