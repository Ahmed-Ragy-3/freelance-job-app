using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<JobCategory> JobCategories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<JobTag> JobTags { get; set; }

        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Freelancer> Freelancers { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<FreelancerSkill> FreelancerSkills { get; set; }
        public DbSet<JobSkill> JobSkills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                                   .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<User>(entity => {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Role).HasConversion<string>();
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasMany(u => u.Bookmarks)
                      .WithOne(b => b.User)
                      .HasForeignKey(b => b.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Notification>(entity => {
                entity.Property(n => n.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<JobCategory>(entity => {
                entity.HasKey(jc => new { jc.JobId, jc.CategoryId });

                entity.HasOne(jc => jc.Job)
                      .WithMany(j => j.Categories)
                      .HasForeignKey(jc => jc.JobId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();

                entity.HasOne(jc => jc.Category)
                      .WithMany(c => c.JobCategories)
                      .HasForeignKey(jc => jc.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();
            });

            modelBuilder.Entity<JobTag>(entity => {
                entity.HasKey(jt => new { jt.JobId, jt.TagId });

                entity.HasOne(jt => jt.Job)
                      .WithMany(j => j.Tags)
                      .HasForeignKey(jt => jt.JobId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();

                entity.HasOne(jt => jt.Tag)
                      .WithMany(t => t.JobTags)
                      .HasForeignKey(jt => jt.TagId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();
            });

            // Attachment configuration supporting both optional Job & Application links
            modelBuilder.Entity<Attachment>(entity => {
                // Relationship to Job (Optional)
                entity.HasOne(a => a.Job)
                      .WithMany(j => j.Attachments)
                      .HasForeignKey(a => a.JobId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired(false);

                // Composite Foreign Key Relationship to Application
                entity.HasOne(a => a.Application)
                      .WithMany(app => app.Attachments)
                      .HasForeignKey(a => new { a.ApplicationJobId, a.ApplicationFreelancerId })
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired(false);
            });

            modelBuilder.Entity<Review>(entity => {
                entity.HasOne(r => r.Job)
                      .WithOne(j => j.Review)
                      .HasForeignKey<Review>(r => r.JobId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();
            });

            modelBuilder.Entity<Freelancer>(entity => {
                entity.HasKey(f => f.UserId);
                entity.HasOne(f => f.User)
                      .WithOne(u => u.Freelancer)
                      .HasForeignKey<Freelancer>(f => f.UserId)
                      .IsRequired();
            });

            modelBuilder.Entity<Client>(entity => {
                entity.HasKey(c => c.UserId);
                entity.HasOne(c => c.User)
                      .WithOne(u => u.Client)
                      .HasForeignKey<Client>(c => c.UserId)
                      .IsRequired();
            });

            modelBuilder.Entity<Job>(entity => {
                entity.Property(j => j.Deadline).HasColumnType("date");
                entity.Property(j => j.PostedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(j => j.Client)
                      .WithMany()
                      .HasForeignKey(j => j.ClientId)
                      .IsRequired();
            });

            modelBuilder.Entity<Application>(entity => {
                entity.HasKey(a => new { a.JobId, a.FreelancerId });

                entity.HasOne(a => a.Job)
                      .WithMany(j => j.Applications)
                      .HasForeignKey(a => a.JobId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();

                entity.Property(a => a.AppStatus).HasConversion<string>();

                entity.HasOne(a => a.Freelancer)
                      .WithMany(f => f.Applications)
                      .HasForeignKey(a => a.FreelancerId)
                      .HasPrincipalKey(f => f.UserId)
                      .IsRequired();
            });

            modelBuilder.Entity<Bookmark>(entity => {
                entity.HasOne(b => b.Job)
                      .WithMany(j => j.Bookmarks)
                      .HasForeignKey(b => b.JobId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();

                entity.HasOne(b => b.User)
                      .WithMany(u => u.Bookmarks)
                      .HasForeignKey(b => b.UserId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasPrincipalKey(u => u.Id)
                      .IsRequired();
            });

            modelBuilder.Entity<FreelancerSkill>(entity => {
                entity.HasKey(fs => new { fs.FreelancerId, fs.SkillId });
                entity.HasOne(fs => fs.Freelancer)
                      .WithMany(f => f.FreelancerSkills)
                      .HasForeignKey(fs => fs.FreelancerId)
                      .HasPrincipalKey(f => f.UserId)
                      .IsRequired();

                entity.HasOne(fs => fs.Skill)
                      .WithMany(s => s.FreelancerSkills)
                      .HasForeignKey(fs => fs.SkillId)
                      .IsRequired();
            });

            modelBuilder.Entity<JobSkill>(entity => {
                entity.HasKey(js => new { js.JobId, js.SkillId });
                entity.HasOne(js => js.Job)
                      .WithMany(j => j.Skills)
                      .HasForeignKey(js => js.JobId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();

                entity.HasOne(js => js.Skill)
                      .WithMany(s => s.JobSkills)
                      .HasForeignKey(js => js.SkillId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();
            });
        }
    }
}