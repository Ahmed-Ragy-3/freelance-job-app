namespace backend.Services
{
    public interface IFileValidationService
    {
        void ValidateAttachment(IFormFile file);
    }
}
