using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebServer.DataContext;
using WebServer.Logging;
using WebServer.Models;

namespace WebServer.BusinessLogic
{
    /// <summary>
    /// Handles the achievements of all users which data changed.
    /// </summary>
    public class AchievementCalculationManager : IAchievementCalculationManager
    {
        #region Fields

        private readonly IList<IAchievementCalculator> _achievementCalculators;
        private readonly IAchievementDb _achievementDb;
        private readonly ILoggerFacade _logger;
        private int _userIdToUpdate;
        private readonly Task _updateTask;

        #endregion;

        /// <summary>
        /// Creates the Manager using the achievementDetector to find all available achievements.
        /// </summary>
        /// <param name="achievementDetector">Detector used to find the available achievements</param>
        /// <param name="achievementDb">Instance of the achievement database.</param>
        /// <param name="logger">Logger to use.</param>
        public AchievementCalculationManager(IAchievementCalculatorDetector achievementDetector,
            IAchievementDb achievementDb, ILoggerFacade logger)
        {
            _achievementDb = achievementDb;
            _logger = logger;
            _achievementCalculators = _getAvailableAchievementCalculators(achievementDetector);
            _updateTask = new Task(_updateAchievements);
        }

        private IList<IAchievementCalculator> _getAvailableAchievementCalculators(
            IAchievementCalculatorDetector achievementDetector)
        {
            var availableCalculators = achievementDetector.FindAllAchievementCalculator().ToList();

            var countOfAvailableCalculatorsMessage = $"Got {availableCalculators.Count} available achievements.";
            _logger.Log(countOfAvailableCalculatorsMessage, LogLevel.Info);

            return availableCalculators;
        }

        private void _updateAchievements()
        {
            var user = _achievementDb.GetSmartPlaneUserById(_userIdToUpdate);
            _calculateAchievementsForUser(user);
            _calculateRankingForUser(user);
            _achievementDb.SaveChanges();
        }

        private void _calculateAchievementsForUser(SmartPlaneUser user)
        {
            foreach (var achievementCalculator in _achievementCalculators)
            {
                try
                {
                    _logger.Log($"start calculation with the calculator: {achievementCalculator.GetType().Name} ",
                        LogLevel.Info);
                    achievementCalculator.CalculateAchievementProgress(user);
                }
                catch (Exception e)
                {
                    _logger.Log(
                        $"The calculation of the AchievementCalculator {achievementCalculator.GetType().Name}, throws a exception: {e.Message}",
                        LogLevel.Error);
                }
            }
        }

        private void _calculateRankingForUser(SmartPlaneUser user)
        {
            user.RankingPoints = 0;
            //The first version of ranking calculation uses a constant for each achievement. So all achievements got the same points
            const int achievementPoints = 1;
            foreach (var achievement in user.Achievements)
            {
                user.RankingPoints += achievementPoints*achievement.Progress;
            }
        }

        public void UpdateForUser(int userId)
        {
            _userIdToUpdate = userId;
            _updateTask.Start();

            var addedUserMessage = $"Started updating user with ID {userId}.";
            _logger.Log(addedUserMessage, LogLevel.Info);
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AchievementCalculationManager()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // The update task works on the _achievementDb which needs to be disposed. Therefore we wait for the task to finish 
                // even though we are blocking the GC-thread, because 100ms delay can be ignored.
                while (_updateTask.Status == TaskStatus.Running)
                {
                    Thread.Sleep(100);
                }
                _achievementDb.Dispose();
            }
        }

        #endregion
    }
}