using backend.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Model {
    [Table("Tags")]
    public class Tag {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(20, ErrorMessage = "Name must be 20 characters maximum.")]
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<JobTag> JobTags { get; set; } = new List<JobTag>();
    }
}
