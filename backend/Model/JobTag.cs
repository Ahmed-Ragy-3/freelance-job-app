using backend.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Model {
    [Table("JobTags")]
    public class JobTag {
        [Required]
        [ForeignKey(nameof(Job))]
        public int JobId { get; set; }

        public Job Job { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Tag))]
        public int TagId { get; set; }

        public Tag Tag { get; set; } = null!;
    }
}
