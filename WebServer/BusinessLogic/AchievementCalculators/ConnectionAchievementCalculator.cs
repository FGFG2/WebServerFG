using System.Linq;
using WebServer.Models;

namespace WebServer.BusinessLogic.AchievementCalculators
{
    public class ConnectionAchievementCalculator : AchievementCalculator
    {
        public const string AchievementName = "ConnectionMaster";
        public const int OnePercentStep = 1;

        public ConnectionAchievementCalculator() : base(AchievementName)
        {
        }

        protected override int CalculateProgress(SmartPlaneUser targetUser)
        {
            var connectionTimes = targetUser.ConnectedDatas.Count(c => c.Value);
            var percent = connectionTimes / OnePercentStep;
            return percent > 100 ? 100 : percent;
        }

        protected override Achievement CreateAchievement()
        {
            return new Achievement
            {
                Name = AchievementName,
                Description = "Verbinde dich 100 mal mit deinem SmartPlane",
                Progress = 0,
                ImageUrl = ""
            };
        }

    }
}