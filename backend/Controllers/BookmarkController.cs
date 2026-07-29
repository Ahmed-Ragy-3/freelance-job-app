using backend.dtos;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Auth;

namespace backend.Controllers {
    [ApiController]
    [Route("bookmark")]
    [Authorize]
    public class BookmarkController(BookmarkService bookmarkService) : ControllerBase {

        [HttpGet]
        [ProducesResponseType(typeof(HomeDtos), 200)]
        public async Task<ActionResult<PaginatedResponse<JobSummaryDto>>> GetBookmarks([FromQuery] BookmarkFetchDto dto) {
            var userId = User.GetUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });

            dto.UserId = userId.Value;
            var bookmarkedJobs = await bookmarkService.GetBookmarksAsync(dto);
            return Ok(bookmarkedJobs);
        }

        [HttpPost("{jobId:int}")]
        [ProducesResponseType(typeof(GlobalSearchDtos), 200)]
        public async Task<ActionResult> SaveBookmark(int jobId) {
            try {
                var userId = User.GetUserId();
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });

                await bookmarkService.Save(jobId, userId.Value);
                return Created();

            } catch (ArgumentException ex) {
                return BadRequest(new { message = ex.Message });

            } catch (Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError, new {
                    message = "An unexpected error occurred."
                });
            }
        }
 
        [HttpDelete("{jobId:int}")]
        [ProducesResponseType(typeof(GlobalSearchDtos), 200)]
        public async Task<ActionResult> RemoveBookmark(int jobId) {
            try {
                var userId = User.GetUserId();
                if (!userId.HasValue)
                    return Unauthorized(new { message = "Invalid or missing user identity in JWT token." });

                await bookmarkService.Remove(jobId, userId.Value);
                return Created();

            } catch (ArgumentException ex) {
                return BadRequest(new { message = ex.Message });

            } catch (Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError, new {
                    message = "An unexpected error occurred."
                });
            }
        }
    }
}
