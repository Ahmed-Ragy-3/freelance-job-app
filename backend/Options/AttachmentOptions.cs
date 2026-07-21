using Microsoft.Extensions.Options;

namespace backend.Options {
    public class AttachmentOptions : IOptions<AttachmentOptions> {
        public string AllowedExtensions { get; set; }
        public int MaxSizeMB { get; set; }

        public AttachmentOptions Value => throw new NotImplementedException();
    }
}
