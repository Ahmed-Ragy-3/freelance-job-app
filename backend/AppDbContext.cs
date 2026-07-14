using backend.model;
using Microsoft.EntityFrameworkCore;

namespace backend {
    public class AppDbContext : DbContext {
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {

        } 

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Attachment> attachments { get; set; }
        public DbSet<Categories> categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<>
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                                                           .SelectMany(e => e.GetForeignKeys())) {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
