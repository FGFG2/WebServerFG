using System.Data.Entity;
using NLog;
using WebServer.Models;

namespace WebServer.DataContext
{
    public class AchievementDb : DbContext
    {
        public virtual DbSet<SmartPlaneUser> SmartPlaneUsers { get; set; }
        public virtual DbSet<LogEntry> LogEntries { get; set; }

        public AchievementDb() : base("DefaultConnection")
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            Database.SetInitializer<AchievementDb>(null);
        }
    }
}