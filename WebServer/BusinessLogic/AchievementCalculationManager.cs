using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Universial.Core.Utilities;
using WebServer.DataContext;
using WebServer.Logging;
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
        private readonly IAchievementDb _achievementDb;
        private readonly ILoggerFacade _logger;
        private int _userIdToUpdate;
        private Task _updateTask;

        #endregion;

        /// <summary>
        /// Creates the Manager using the achievementDetector to find all available achievements.
        /// </summary>
        /// <param name="achievementDetector">Detector used to find the available achievements</param>
        /// <param name="achievementDb">Instance of the achievement database.</param>
        /// <param name="logger">Logger to use.</param>
        public AchievementCalculationManager(IAchievementCalculatorDetector achievementDetector, IAchievementDb achievementDb, ILoggerFacade logger)
        {
            _achievementDb = achievementDb;
            _logger = logger;
            _achievementCalculators = _getAvailableAchievementCalculators(achievementDetector);
            _updateTask = new Task(_updateAchievements);
        }

        private IList<IAchievementCalculator> _getAvailableAchievementCalculators(IAchievementCalculatorDetector achievementDetector)
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
            _achievementDb.SaveChanges();
        }

        private void _calculateAchievementsForUser(SmartPlaneUser user)
        {
            foreach (var achievementCalculator in _achievementCalculators)
            {
                achievementCalculator.CalculateAchievementProgress(user);
            }
        }

        public void UpdateForUser(int userId)
        {
            _addUserToAchievementUpdateQueue(userId);

            var addedUserMessage = $"Added user with ID {userId} to achievement update queue.";
            _logger.Log(addedUserMessage, LogLevel.Info);
        }

        private void _addUserToAchievementUpdateQueue(int userId)
        {
            _userIdToUpdate = userId;
            _updateTask.Start();
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