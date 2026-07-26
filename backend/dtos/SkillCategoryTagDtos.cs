using backend.Model;

namespace backend.Dtos {
    public class SkillCreateDto {
        public string Name { get; set; } = string.Empty;
    }

    public class SkillResponseDto {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ExperienceLevel { get; set; } // only filled when returned as part of a Freelancer's skill list
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
