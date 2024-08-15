using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using API.Models;
using Microsoft.AspNetCore.Identity;


namespace API.Data{
    public class AppDbContext : IdentityDbContext<AppUser>{
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){
         
        }
        public DbSet<task> Tasks { get; set; }
        public DbSet<TaskUser> TaskUsers { get; set; }

        
      protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many relationship using the TaskUser entity
            modelBuilder.Entity<TaskUser>()
                .HasKey(tu => new { tu.TaskId, tu.UserId });

            modelBuilder.Entity<TaskUser>()
                .HasOne(tu => tu.Task)
                .WithMany(t => t.TaskUsers)
                .HasForeignKey(tu => tu.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskUser>()
                .HasOne(tu => tu.User)
                .WithMany(u => u.TaskUsers)
                .HasForeignKey(tu => tu.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Specify the length for key columns in Identity tables
            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.Property(m => m.Id).HasMaxLength(128);
                entity.Property(m => m.NormalizedName).HasMaxLength(128);
            });

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.Property(m => m.Id).HasMaxLength(128);
                entity.Property(m => m.NormalizedUserName).HasMaxLength(128);
                entity.Property(m => m.NormalizedEmail).HasMaxLength(128);
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.Property(m => m.LoginProvider).HasMaxLength(128);
                entity.Property(m => m.ProviderKey).HasMaxLength(128);
            });

            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.Property(m => m.UserId).HasMaxLength(128);
                entity.Property(m => m.RoleId).HasMaxLength(128);
            });

            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.Property(m => m.UserId).HasMaxLength(128);
                entity.Property(m => m.LoginProvider).HasMaxLength(128);
                entity.Property(m => m.Name).HasMaxLength(128);
            });
        }
    
    }

}