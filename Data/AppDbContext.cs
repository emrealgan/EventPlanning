using EventPlanning.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventPlanning.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options)
        {
        }
        public DbSet<User>? Users { get; set; }
        public DbSet<Activity>? Activities { get; set; }
        public DbSet<Location>? Locations { get; set; }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Participant>? Participants { get; set; }
        public DbSet<Message>? Messages { get; set; }
        public DbSet<Score>? Scores { get; set; }
        public DbSet<UserCategory>? UserCategories { get; set; }
        public DbSet<ActivityCategory>? ActivityCategories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ActivityCategory>()
                .HasKey(ac => ac.ID);

            modelBuilder.Entity<ActivityCategory>()
                .HasOne(ac => ac.Activity)  
                .WithMany(a => a.ActivityCategories)
                .HasForeignKey(ac => ac.ActivityID);

            modelBuilder.Entity<ActivityCategory>()
                .HasOne(ac => ac.Category)  
                .WithMany(c => c.ActivityCategories)
                .HasForeignKey(ac => ac.CategoryID);

            modelBuilder.Entity<Activity>()
                 .HasMany(a => a.Categories)
                 .WithMany(c => c.Activities)
                 .UsingEntity<ActivityCategory>(
                     j => j
                         .HasOne(ac => ac.Category)
                         .WithMany(c => c.ActivityCategories)
                         .HasForeignKey(ac => ac.CategoryID),
                     j => j
                         .HasOne(ac => ac.Activity)
                         .WithMany(a => a.ActivityCategories)
                         .HasForeignKey(ac => ac.ActivityID)
                 );

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            
           modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Participant>()
               .HasOne(p => p.User)
               .WithMany(u => u.Participants)
               .HasForeignKey(p => p.UserID)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Participant>()
                .HasOne(p => p.Activity)
                .WithMany(a => a.Participants)
                .HasForeignKey(p => p.ActivityID)
                .OnDelete(DeleteBehavior.NoAction);


            base.OnModelCreating(modelBuilder);
        }
    }
}
