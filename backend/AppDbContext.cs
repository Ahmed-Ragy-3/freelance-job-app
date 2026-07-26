using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend {
    public class AppDbContext : DbContext {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {

        }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<JobCategory> jobCategories { get; set; }
        public DbSet<Tag> tags { get; set; }
        public DbSet<JobTag> jobTags{ get; set; }

        public DbSet<Attachment> attachments { get; set; }
        public DbSet<Review> reviews { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Freelancer> Freelancers { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<FreelancerSkill> FreelancerSkills { get; set; }
        public DbSet<JobSkill> JobSkills { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity => {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Role).HasConversion<string>();
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<Notification>(entity => {
                entity.Property(n => n.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<JobCategory>(entity => {
                entity.HasKey(jc => new { jc.JobId, jc.CategoryId });

                entity.HasOne(jc => jc.Job)
                      .WithMany(j => j.JobCategories)
                      .HasForeignKey(jc => jc.JobId)
                      .IsRequired();

                entity.HasOne(jc => jc.Category)
                      .WithMany(c => c.JobCategories)
                      .HasForeignKey(jc => jc.CategoryId)
                      .IsRequired();
            });

            modelBuilder.Entity<JobTag>(entity => {
                entity.HasKey(jt => new { jt.JobId, jt.TagId });

                entity.HasOne(jt => jt.Job)
                      .WithMany()
                      .HasForeignKey(jt => jt.JobId)
                      .IsRequired();

                entity.HasOne(jt => jt.Tag)
                      .WithMany()
                      .HasForeignKey(jt => jt.TagId)
                      .IsRequired();
            });

            modelBuilder.Entity<Attachment>(entity => {
                entity.HasOne(a => a.Job)
                      .WithMany(j => j.Attachments)
                      .HasForeignKey(a => a.JobId)
                      .IsRequired();
            });

            modelBuilder.Entity<Review>(entity => {
                entity.HasOne(r => r.Job)
                      .WithOne(j => j.Review)
                      .HasForeignKey<Review>(r => r.JobId)
                      .IsRequired();
            });

            modelBuilder.Entity<Job>(entity => {
                entity.Property(j => j.Deadline).HasColumnType("date");
                entity.Property(j => j.PostedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<Application>(entity => {
                entity.HasOne(a => a.Job)
                      .WithMany()
                      .HasForeignKey(a => a.JobId)
                      .IsRequired();
                entity.HasOne(a => a.Freelancer)
                      .WithMany()
                      .HasForeignKey(a => a.FreelancerId)
                      .IsRequired();
                entity.Property(a => a.AppStatus).HasConversion<string>();
            });

            modelBuilder.Entity<Bookmark>(entity => {
                entity.HasOne(b => b.Job)
                      .WithMany()
                      .HasForeignKey(b => b.JobId)
                      .IsRequired();
                entity.HasOne(b => b.Freelancer)
                      .WithMany()
                      .HasForeignKey(b => b.FreelancerId)
                      .IsRequired();
            });

            modelBuilder.Entity<FreelancerSkill>(entity => {
                entity.HasKey(fs => new { fs.FreelancerId, fs.SkillId });
                entity.HasOne(fs => fs.Freelancer)
                      .WithMany()
                      .HasForeignKey(fs => fs.FreelancerId)
                      .IsRequired();
                entity.HasOne(fs => fs.Skill)
                      .WithMany(s => s.FreelancerSkills)
                      .HasForeignKey(fs => fs.SkillId)
                      .IsRequired();
            });

            modelBuilder.Entity<JobSkill>(entity => {
                entity.HasKey(js => new { js.JobId, js.SkillId });
                entity.HasOne(js => js.Job)
                      .WithMany()
                      .HasForeignKey(js => js.JobId)
                      .IsRequired();
                entity.HasOne(js => js.Skill)
                      .WithMany(s => s.JobSkills)
                      .HasForeignKey(js => js.SkillId)
                      .IsRequired();
            });
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                                               .SelectMany(e => e.GetForeignKeys())) {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
