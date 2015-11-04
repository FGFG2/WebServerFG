using System.Collections.Generic;
using System.Linq;
using WebServer.Models;

namespace WebServer.BusinessLogic
{
    /// <summary>
    /// Handles the achievements of all users which data changed. Updates them asynchronous with a time interval.
    /// </summary>
    public class AchievementCalculationManager : IAchievementCalculationManager
    {
        #region Fields

        private readonly IList<IAchievementCalculator> _achievementCalculators;

        #endregion;

        /// <summary>
        /// Creates the Manager using the achievementDetector to find all avaialable achievements.
        /// </summary>
        /// <param name="achievementDetector">Detector used to find the available achievements</param>
        public AchievementCalculationManager(IAchievementCalculatorDetector achievementDetector)
        {
            _achievementCalculators = achievementDetector.FindAllAchievementCalculator().ToList();
        }

        public void UpdateForUser(SmartPlaneUser userWithChangedData)
        {
            foreach (var achievementCalculator in _achievementCalculators)
            {
                achievementCalculator.CalculateAchievementProgress(userWithChangedData);
            }
        }
    }
}