namespace backend.Services
{
    public class FileValidationService : IFileValidationService
    {
        private static readonly string[] AllowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB limit

        public void ValidateAttachment(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Uploaded file cannot be empty.");
            }

            if (file.Length > MaxFileSizeBytes)
            {
                throw new ArgumentException($"File '{file.FileName}' exceeds the maximum allowed size of 10 MB.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                throw new ArgumentException($"File '{file.FileName}' has an invalid format. Allowed formats: PDF, JPG, JPEG, PNG, WEBP.");
            }
        }
    }
}