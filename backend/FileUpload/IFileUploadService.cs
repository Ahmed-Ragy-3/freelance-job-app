namespace backend.FileUpload {
    public interface IFileUploadService {
        Task<string> UploadAsync(IFormFile file);
    }
}
