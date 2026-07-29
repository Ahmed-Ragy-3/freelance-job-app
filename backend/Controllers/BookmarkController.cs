using backend.dtos;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers {
    [ApiController]
    [Route("bookmark")]
    public class BookmarkController(BookmarkService bookmarkService) : ControllerBase {

        [HttpGet]
        [ProducesResponseType(typeof(HomeDtos), 200)]
        public async Task<ActionResult<PaginatedResponse<JobSummaryDto>>> GetBookmarks([FromQuery] BookmarkFetchDto dto) {
            // TODO: Get userId from the authenticated user
            var bookmarkedJobs = await bookmarkService.GetBookmarksAsync(dto);
            return Ok(bookmarkedJobs);
        }

        [HttpPost("{jobId:int}")]
        [ProducesResponseType(typeof(GlobalSearchDtos), 200)]
        public async Task<ActionResult> SaveBookmark(int jobId) {
            try {
                // TODO: Get userId from the authenticated user
                await bookmarkService.Save(jobId, 2);
                return Created();

            } catch (ArgumentException ex) {
                return BadRequest(new { message = ex.Message });

            } catch (Exception) {
                // TODO: Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, new {
                    message = "An unexpected error occurred."
                });
            }
        }
 
        [HttpDelete("{jobId:int}")]
        [ProducesResponseType(typeof(GlobalSearchDtos), 200)]
        public async Task<ActionResult> RemoveBookmark(int jobId) {
            try {
                // TODO: Get userId from the authenticated user
                await bookmarkService.Remove(jobId, 2);
                return Created();

            } catch (ArgumentException ex) {
                return BadRequest(new { message = ex.Message });

            } catch (Exception) {
                // TODO: Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, new {
                    message = "An unexpected error occurred."
                });
            }
        }
    }
}
