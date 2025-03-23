using BallroomManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BallroomManager.Api.Data
{
    public class BallroomDbContext : DbContext
    {
        public BallroomDbContext(DbContextOptions<BallroomDbContext> options) : base(options)
        {
        }

        public DbSet<Ballroom> Ballrooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
    
            base.OnModelCreating(modelBuilder);
        }
    }
}