using System.Data.Entity;
using WebServer.Models;

namespace WebServer.DataContext
{
    public class AchievementDb : DbContext
    {
        public virtual DbSet<Achievement> Achievements { get; set; }

        public virtual DbSet<MotorData> MotorDatas { get; set; }

        public virtual DbSet<RudderData> RudderDatas { get; set; }

        public AchievementDb() : base("DefaultConnection")
        {
            
        }
    }
}