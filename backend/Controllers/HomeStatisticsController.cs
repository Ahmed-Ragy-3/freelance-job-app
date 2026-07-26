using backend.dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers {
    [ApiController]
    [Route("home/stats")]
    public class HomeStatisticsController(HomeStatisticsService homeStatsService) : ControllerBase {

        [HttpGet]
        [ProducesResponseType(typeof(HomeStatsDto), 200)]
        public async Task<ActionResult<HomeStatsDto>> GetHomeStats() {
            var homeStats = await homeStatsService.GetHomeStatsAsync();
            return Ok(homeStats);
        }
    }
}
