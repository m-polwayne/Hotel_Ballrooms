using BallroomManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BallroomManager.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ballroom> Ballrooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Ballroom)
                .WithMany()
                .HasForeignKey(b => b.BallroomId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 