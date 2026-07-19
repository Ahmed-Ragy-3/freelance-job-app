using backend.Options;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using Cloudinary = CloudinaryDotNet.Cloudinary;

namespace backend.FileUpload {
    public class CloudinaryService : IFileUploadService {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinaryOptions> options) {
            var settings = options.Value;

            var account = new Account(
                settings.NAME,
                settings.API_KEY,
                settings.API_SECRET);

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadAsync(IFormFile file) {
            await using var stream = file.OpenReadStream();

            var uploadParams = new RawUploadParams {
                File = new FileDescription(file.FileName, stream)
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            return result.SecureUrl.ToString();
        }
    }
}
