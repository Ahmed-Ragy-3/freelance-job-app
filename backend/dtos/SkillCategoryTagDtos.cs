namespace backend.DTOs
{
    // ---- Skill ----
    public class SkillCreateDto
    {
        public string Name { get; set; }
    }

    public class SkillResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ExperienceLevel { get; set; } // only filled when returned as part of a Freelancer's skill list
    }

    // ---- Category ----
    public class CategoryCreateDto
    {
        public string Name { get; set; }
    }

    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    // ---- Tag ----
    public class TagCreateDto
    {
        public string Name { get; set; }
    }

    public class TagResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
