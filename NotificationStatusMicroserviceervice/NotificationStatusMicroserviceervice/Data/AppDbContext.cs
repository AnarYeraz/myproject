using Microsoft.EntityFrameworkCore;
using NotificationStatusMicroserviceervice.Models.Entity;

namespace NotificationStatusMicroserviceervice.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<NotificationLog> NotificationLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotificationLog>()
                .HasKey(n => n.Id);
        }
    }
}
