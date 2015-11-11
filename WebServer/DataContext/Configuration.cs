using System.Data.Entity.Migrations;

namespace WebServer.DataContext
{
    internal sealed class Configuration : DbMigrationsConfiguration<AchievementDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"DataContext";
        }

        protected override void Seed(AchievementDb context)
        {
        }
    }
}
