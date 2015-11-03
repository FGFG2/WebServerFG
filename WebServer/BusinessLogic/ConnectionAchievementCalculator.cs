using System;
using System.Linq;
using WebServer.Models;

namespace WebServer.BusinessLogic
{
    public class ConnectionAchievementCalculator : IAchievementCalculator
    {
        public const string AchievementName = "ConnectionMaster";
        public void CalculateAchievementProgress(SmartPlaneUser targetUser)
        {
            var relatedAchievement = _addAchievementWhenMissing(targetUser);
            relatedAchievement.Progress = Convert.ToByte(_calculateProgress(targetUser));
        }

        private int _calculateProgress(SmartPlaneUser targetUser)
        {
            return targetUser.ConnectedDatas.Any(c=>c.IsConnected) ? 100 : 0;
        }

        private Achievement _addAchievementWhenMissing(SmartPlaneUser targetUser)
        {
            var achievement = targetUser.Achievements.FirstOrDefault(a => a.Name.Equals(AchievementName));
            if (achievement != null)
            {
                return achievement;
            }
            achievement = CreateAchievement();
            targetUser.Achievements.Add(achievement);
            return achievement;
        }

        public Achievement CreateAchievement()
        {
            return new Achievement
            {
                Name = AchievementName,
                Description = "Verbinde dich mit deinem SmartPlane",
                Progress = 0
            };
        }
    }
}