using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace backend.FileUpload {
    [ApiController]
    [Route("files")]
    [Authorize]
    public class FilesController : ControllerBase {
        private readonly IFileUploadService _uploadService;

        public FilesController(IFileUploadService uploadService) {
            _uploadService = uploadService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file) {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var url = await _uploadService.UploadAsync(file);

            return Ok(new {
                Url = url
            });
        }
    }
}
