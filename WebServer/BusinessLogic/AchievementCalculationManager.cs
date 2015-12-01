﻿using System;
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
        // used as ConcurrentSet with the keys as values. The values of the dictionary are not used.
        // The ConcurrentDictionary is used, because there is no default implementation of a concurrent set and the
        // dictionary serves well enough as a set (there is overhead by the values, but the self implementation would not be worth
        // the trouble. One million users would only lead to a memory overhead of 1 MB.
        private readonly ConcurrentDictionary<SmartPlaneUser, byte> _userWithChangedData;
        private readonly CancellationTokenSource _achievementCalculationCancelSource;
        private readonly IAchievementDb _achievementDb;
        private readonly ILoggerFacade _logger;
        private readonly Task _achievementCalculationTask;

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
            _userWithChangedData = new ConcurrentDictionary<SmartPlaneUser, byte>();

            _achievementCalculators = _getAvailableAchievementCalculators(achievementDetector);

            _achievementCalculationCancelSource = new CancellationTokenSource();
            _achievementCalculationTask = new Task(_achievementCalculationTaskAction, _achievementCalculationCancelSource.Token, TaskCreationOptions.LongRunning);
         }

        private IList<IAchievementCalculator> _getAvailableAchievementCalculators(IAchievementCalculatorDetector achievementDetector)
        {
            var availableCalculators = achievementDetector.FindAllAchievementCalculator().ToList();

            var countOfAvailableCalculatorsMessage = $"Got {availableCalculators.Count} available achievements.";
            _logger.Log(countOfAvailableCalculatorsMessage, LogLevel.Info);

            return availableCalculators;
        }

        #region Achievement calculation task
        private void _achievementCalculationTaskAction()
        {
            _updateAchievements();
        }

        private void _updateAchievements()
        {
            var usersToUpdate = _getUsersToUpdate();
            foreach (var user in usersToUpdate)
            {
                _calculateAchievementsForUser(user);
            }
            _achievementDb.SaveChanges();

            var addedUserMessage = $"Updated the achievements of {usersToUpdate.Count} users.";
            _logger.Log(addedUserMessage, LogLevel.Info);
        }

        private IList<SmartPlaneUser> _getUsersToUpdate()
        {
            //dump keys of concurrent dictionary to use them outside synchronized blocks. 
            //Clear the dictionary, so that already updated user are not going to be updated again when not needed.
            IList<SmartPlaneUser> users;
            lock (_userWithChangedData)
            {
                users = _userWithChangedData.Keys.ToList();
                _userWithChangedData.Clear();
            }

            return users;
        }

        private void _calculateAchievementsForUser(SmartPlaneUser user)
        {
            foreach (var achievementCalculator in _achievementCalculators)
            {
                achievementCalculator.CalculateAchievementProgress(user);
            }
        }
        #endregion


        public void UpdateForUser(int userId)
        {
            _addUserToAchievementUpdateQueue(userId);

            var addedUserMessage = $"Added user with ID {userId} to achievement update queue.";
            _logger.Log(addedUserMessage, LogLevel.Info);
        }

        private void _addUserToAchievementUpdateQueue(int userId)
        {
            lock (_userWithChangedData)
            {
                _userWithChangedData.AddOrUpdate(_achievementDb.GetSmartPlaneUserById(userId), 0, (key, oldValue) => 0);
                _achievementCalculationTask.Start();
            }
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
                TaskHelper.TryToStopTask(_achievementCalculationTask, _achievementCalculationCancelSource);
                _achievementCalculationCancelSource.Dispose();
                _achievementCalculationTask.Dispose();
            }
        }
        #endregion
    }
}