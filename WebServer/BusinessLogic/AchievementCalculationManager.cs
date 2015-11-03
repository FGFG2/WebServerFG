using System.Collections.Concurrent;
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

        private readonly ICollection<IAchievementCalculator> _achievementCalculators;

        #endregion;

        /// <summary>
        /// Creates the Manager with a specified list of achievementCalculators that will be checked every time a users data changes.
        /// </summary>
        /// <param name="achievementCalculators">Achievements that should be evaluated.</param>
        public AchievementCalculationManager(ICollection<IAchievementCalculator> achievementCalculators)
        {
            _achievementCalculators = achievementCalculators;
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