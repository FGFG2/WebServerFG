using System.Linq;
using WebServer.Models;

namespace WebServer.BusinessLogic.AchievementCalculators
{
    public class ConnectionAchievementCalculator : AchievementCalculator
    {
        public const string AchievementName = "ConnectionMaster";

        public ConnectionAchievementCalculator() : base(AchievementName)
        {
        }

        protected override int CalculateProgress(SmartPlaneUser targetUser)
        {
            return targetUser.ConnectedDatas.Any(c=>c.IsConnected) ? 100 : 0;
        }

        protected override Achievement CreateAchievement()
        {
            return new Achievement
            {
                Name = AchievementName,
                Description = "Verbinde dich mit deinem SmartPlane",
                Progress = 0,
                ImageUrl = ""
            };
        }

    }
}