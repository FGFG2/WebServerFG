
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using WebServer.BusinessLogic;
using WebServer.Models;

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
            if (context.SmartPlaneUsers.Any()) { return;}
            context.SmartPlaneUsers.Add(new SmartPlaneUser());
            context.SaveChanges();
        }
    }
}
