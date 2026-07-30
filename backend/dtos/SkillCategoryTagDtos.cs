using backend.Model;

namespace backend.DTOs {
    public class SkillCreateDto {
        public string Name { get; set; } = string.Empty;
    }

    public class SkillResponseDto
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public int ExperienceLevel { get; set; }
    }

    public class CategoryCreateDto {
        public string Name { get; set; } = string.Empty;
    }

    public class CategoryResponseDto {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CategorySummaryDto {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int NumberOfJobs { get; set; }

        public static CategorySummaryDto FromCategory(Category category) {
            return new CategorySummaryDto {
                Id = category.Id,
                Name = category.Name,
                NumberOfJobs = category.JobCategories?.Count ?? 0
            };
        }
    }

    public class TagCreateDto {
        public string Name { get; set; } = string.Empty;
    }

    public class TagResponseDto {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
