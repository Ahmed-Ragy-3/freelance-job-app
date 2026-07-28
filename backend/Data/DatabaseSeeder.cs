using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.Data {
    public class DatabaseSeeder(AppDbContext context) {
        private readonly AppDbContext _context = context;
        private readonly Random _random = new(42);

        public async Task SeedAsync() {
            var skills = await SeedSkillsAsync();
            var categories = await SeedCategoriesAsync();
            var tags = await SeedTagsAsync();

            if (!await _context.Users.AnyAsync()) {
                await SeedUsersAsync();
            }

            var users = await _context.Users.ToListAsync();
            if (!await _context.Clients.AnyAsync()) {
                await SeedClientsAsync(users);
            }

            if (!await _context.Freelancers.AnyAsync()) {
                await SeedFreelancersAsync(users);
            }

            var seededFreelancers = await _context.Freelancers.ToListAsync();
            if (!await _context.FreelancerSkills.AnyAsync()) {
                await SeedFreelancerSkillsAsync(seededFreelancers, skills);
            }

            var seededClients = await _context.Clients.ToListAsync();
            if (!await _context.Jobs.AnyAsync()) {
                await SeedJobsAsync(seededClients, categories, tags, skills);
            }

            var seededJobs = await _context.Jobs.ToListAsync();
            if (!await _context.Applications.AnyAsync()) {
                await SeedApplicationsAsync(seededFreelancers, seededJobs);
            }

            if (!await _context.Bookmarks.AnyAsync()) {
                await SeedBookmarksAsync(seededFreelancers, seededJobs);
            }

            if (!await _context.Notifications.AnyAsync()) {
                await SeedNotificationsAsync(users);
            }

            if (!await _context.Attachments.AnyAsync()) {
                await SeedAttachmentsAsync(seededJobs);
            }

            if (!await _context.Reviews.AnyAsync()) {
                await SeedReviewsAsync(seededJobs);
            }

            if (!await _context.JobCategories.AnyAsync()) {
                await SeedJobCategoriesAsync(seededJobs, categories);
            }

            if (!await _context.JobTags.AnyAsync()) {
                await SeedJobTagsAsync(seededJobs, tags);
            }

            if (!await _context.JobSkills.AnyAsync()) {
                await SeedJobSkillsAsync(seededJobs, skills);
            }
        }

        private async Task SeedUsersAsync() {
            var users = new List<User>();
            users.Add(CreateUser("admin", Role.Admin, "admin@freelance.test"));

            for (int i = 1; i <= 8; i++) {
                users.Add(CreateUser($"client{i}", Role.Client, $"client{i}@freelance.test"));
            }

            for (int i = 1; i <= 12; i++) {
                users.Add(CreateUser($"freelancer{i}", Role.Freelancer, $"freelancer{i}@freelance.test"));
            }

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();
        }

        private async Task SeedClientsAsync(List<User> users) {
            var clients = new List<Client>();

            foreach (var user in users.Where(u => u.Role == Role.Client).ToList()) {
                clients.Add(new Client {
                    UserId = user.Id,
                    CompanyName = $"{GetRandomWord()} Labs",
                    CompanyDetails = $"{GetRandomWord()} delivers modern digital solutions.",
                    Logo = $"https://example.com/logos/{user.UserName}.png"
                });
            }

            await _context.Clients.AddRangeAsync(clients);
            await _context.SaveChangesAsync();
        }

        private async Task SeedFreelancersAsync(List<User> users) {
            var freelancers = new List<Freelancer>();

            foreach (var user in users.Where(u => u.Role == Role.Freelancer).ToList()) {
                freelancers.Add(new Freelancer {
                    UserId = user.Id,
                    Bio = $"{GetRandomWord()} specialist with {GetRandomWord()} expertise across remote delivery.",
                    Link = $"https://portfolio.example/{user.UserName}"
                });
            }

            await _context.Freelancers.AddRangeAsync(freelancers);
            await _context.SaveChangesAsync();
        }

        private async Task SeedFreelancerSkillsAsync(List<Freelancer> freelancers, List<Skill> skills) {
            foreach (var freelancer in freelancers) {
                var selectedSkills = skills.OrderBy(_ => _random.Next()).Take(3 + _random.Next(3)).ToList();
                foreach (var skill in selectedSkills) {
                    _context.FreelancerSkills.Add(new FreelancerSkill {
                        FreelancerId = freelancer.UserId,
                        SkillId = skill.Id,
                        ExperienceLevel = 1 + _random.Next(5)
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedJobsAsync(List<Client> clients, List<Category> categories, List<Tag> tags, List<Skill> skills) {
            var seededJobs = new List<Job>();
            for (int i = 1; i <= 25; i++) {
                var client = clients[_random.Next(clients.Count)];
                var job = new Job {
                    Title = $"{GetRandomWord()} {GetRandomWord()} Project",
                    Budget = 500 + _random.Next(20000),
                    Description = $"We need a skilled professional for {GetRandomWord()} work with {GetRandomWord()} deliverables and clear milestones.",
                    Deadline = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7 + _random.Next(60))),
                    JobStatus = (JobStatus)_random.Next(Enum.GetValues<JobStatus>().Length),
                    PostedAt = DateTime.UtcNow.AddDays(-_random.Next(90)),
                    AcceptedAt = DateTime.UtcNow.AddDays(-_random.Next(30)),
                    FinishedAt = DateTime.UtcNow.AddDays(-_random.Next(10)),
                    ClientId = client.UserId
                };

                seededJobs.Add(job);
            }

            await _context.Jobs.AddRangeAsync(seededJobs);
            await _context.SaveChangesAsync();

            await SeedJobCategoriesAsync(seededJobs, categories);
            await SeedJobTagsAsync(seededJobs, tags);
            await SeedJobSkillsAsync(seededJobs, skills);
            await SeedAttachmentsAsync(seededJobs);
            await SeedReviewsAsync(seededJobs);
        }

        private async Task SeedJobCategoriesAsync(List<Job> jobs, List<Category> categories) {
            foreach (var job in jobs) {
                foreach (var category in categories.OrderBy(_ => _random.Next()).Take(1 + _random.Next(2))) {
                    _context.JobCategories.Add(new JobCategory { JobId = job.Id, CategoryId = category.Id });
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedJobTagsAsync(List<Job> jobs, List<Tag> tags) {
            foreach (var job in jobs) {
                foreach (var tag in tags.OrderBy(_ => _random.Next()).Take(2 + _random.Next(3))) {
                    _context.JobTags.Add(new JobTag { JobId = job.Id, TagId = tag.Id });
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedJobSkillsAsync(List<Job> jobs, List<Skill> skills) {
            foreach (var job in jobs) {
                foreach (var skill in skills.OrderBy(_ => _random.Next()).Take(2 + _random.Next(3))) {
                    _context.JobSkills.Add(new JobSkill { JobId = job.Id, SkillId = skill.Id });
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedAttachmentsAsync(List<Job> jobs) {
            foreach (var job in jobs) {
                if (_random.Next(100) < 70) {
                    _context.Attachments.Add(new Attachment {
                        JobId = job.Id,
                        Url = $"https://example.com/files/{job.Id}-attachment.pdf",
                        FileName = $"{job.Id}_brief.pdf",
                        Type = "pdf"
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedReviewsAsync(List<Job> jobs) {
            foreach (var job in jobs) {
                if (job.JobStatus == JobStatus.Finished || job.JobStatus == JobStatus.Passed) {
                    _context.Reviews.Add(new Review {
                        JobId = job.Id,
                        Rate = 3 + _random.Next(3),
                        Comment = $"Great collaboration on {GetRandomWord()} deliverables."
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedApplicationsAsync(List<Freelancer> freelancers, List<Job> jobs) {
            foreach (var freelancer in freelancers.OrderBy(_ => _random.Next()).Take(10)) {
                foreach (var job in jobs.OrderBy(_ => _random.Next()).Take(3)) {
                    _context.Applications.Add(new Application {
                        JobId = job.Id,
                        FreelancerId = freelancer.UserId,
                        CoverLetter = $"I am excited to contribute to {job.Title} with a focused, reliable approach.",
                        Bid = 100 + _random.Next(5000),
                        Timeline = 3 + _random.Next(14),
                        AppStatus = (AppStatus)_random.Next(Enum.GetValues<AppStatus>().Length),
                        Job = job,
                        Freelancer = freelancer
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedBookmarksAsync(List<Freelancer> freelancers, List<Job> jobs) {
            foreach (var freelancer in freelancers.OrderBy(_ => _random.Next()).Take(8)) {
                foreach (var job in jobs.OrderBy(_ => _random.Next()).Take(2)) {
                    _context.Bookmarks.Add(new Bookmark {
                        JobId = job.Id,
                        FreelancerId = freelancer.UserId,
                        Job = job,
                        Freelancer = freelancer
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedNotificationsAsync(List<User> users) {
            foreach (var user in users.OrderBy(_ => _random.Next()).Take(12)) {
                _context.Notifications.Add(new Notification {
                    Title = $"New update for {GetRandomWord()}",
                    Read = _random.Next(100) < 50,
                    CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(30)),
                    UserId = user.Id,
                    User = user
                });
            }

            await _context.SaveChangesAsync();
        }

        private async Task<List<Skill>> SeedSkillsAsync() {
            if (await _context.Skills.AnyAsync()) {
                return await _context.Skills.ToListAsync();
            }

            var skills = new[] {
                "C#", "ASP.NET Core", "React", "Angular", "SQL Server", "Azure", "Docker", "DevOps",
                "UI/UX", "Mobile", "Python", "Node.js", "TypeScript", "Testing", "Security", "AI", "Data Engineering", "Product Design"
            };

            var entities = skills.Select(name => new Skill {
                Name = name,
                FreelancerSkills = new List<FreelancerSkill>(),
                JobSkills = new List<JobSkill>()
            }).ToList();
            await _context.Skills.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        private async Task<List<Category>> SeedCategoriesAsync() {
            if (await _context.Categories.AnyAsync()) {
                return await _context.Categories.ToListAsync();
            }

            var categories = new[] { "Web Development", "Mobile", "Design", "Data", "Cloud", "Marketing", "Writing", "Support" };
            var entities = categories.Select(name => new Category { Name = name }).ToList();
            await _context.Categories.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        private async Task<List<Tag>> SeedTagsAsync() {
            if (await _context.Tags.AnyAsync()) {
                return await _context.Tags.ToListAsync();
            }

            var tags = new[] { "Urgent", "Remote", "Full-Time", "Part-Time", "React", "Backend", "Startup", "Enterprise", "API", "Cloud", "AI", "Design" };
            var entities = tags.Select(name => new Tag { Name = name }).ToList();
            await _context.Tags.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        private User CreateUser(string userName, Role role, string email) {
            return new User {
                UserName = userName,
                Email = email,
                Password = "Password123!",
                Role = role,
                ImageUrl = $"https://example.com/images/{userName}.jpg"
            };
        }

        private string GetRandomWord() {
            var words = new[] { "Digital", "Modern", "Creative", "Reliable", "Smart", "Agile", "Cloud", "Launch", "Growth", "Data", "Product", "Studio" };
            return words[_random.Next(words.Length)];
        }
    }
}