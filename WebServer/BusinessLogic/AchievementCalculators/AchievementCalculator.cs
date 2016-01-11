using System;
using System.IO;
using System.Linq;
using System.Security.Policy;
using WebServer.Logging;
using WebServer.Models;

namespace WebServer.BusinessLogic.AchievementCalculators
{
    /// <summary>
    /// Base class for AchivementCalculators. 
    /// </summary>
    public abstract class AchievementCalculator : IAchievementCalculator
    {
        protected ILoggerFacade Logger;

        private readonly string _achievementName;

        protected AchievementCalculator(string achievementName, ILoggerFacade logger)
        {
            Logger = logger;
            _achievementName = achievementName;
        }

        public void CalculateAchievementProgress(SmartPlaneUser targetUser)
        {
            var relatedAchievement = _addAchievementWhenMissing(targetUser);
            if (relatedAchievement.Progress == 100)
            {
                var skippedMsg =$"Skipped calculation of Achievement {_achievementName} of user {targetUser.Id}: Already achieved.";
                Logger.Log(skippedMsg, LogLevel.Debug);
                return;
            }
            var newProgress = CalculateProgress(targetUser);

            var newProgressMsg = $"Achievement {_achievementName} of user {targetUser.Id}, updated progress to {newProgress}.";
            Logger.Log(newProgressMsg, LogLevel.Info);

            relatedAchievement.Progress = Convert.ToByte(CalculateProgress(targetUser));
        }

        private Achievement _addAchievementWhenMissing(SmartPlaneUser targetUser)
        {
            var achievement = targetUser.Achievements.FirstOrDefault(a => a.Name.Equals(_achievementName));
            if (achievement != null)
            {
                return achievement;
            }
            achievement = CreateAchievement();
            targetUser.Achievements.Add(achievement);

            var msg = $"Added missing Achievement {_achievementName} to user {targetUser.Id}.";
            Logger.Log(msg,LogLevel.Debug);

            return achievement;
        }

        /// <summary>
        /// Method which calculates the progress of this achievement with the passed SmartPlaneUser
        /// </summary>
        /// <param name="targetUser"></param>
        /// <returns></returns>
        protected abstract int CalculateProgress(SmartPlaneUser targetUser);

        /// <summary>
        /// Factory method which creates the Achievement related to this calculator.
        /// </summary>
        /// <returns></returns>
        protected abstract Achievement CreateAchievement();

        protected string GetPathToImage()
        {
            return $"Resources/AchievementImages/{_achievementName}.png";
        }
    }
}