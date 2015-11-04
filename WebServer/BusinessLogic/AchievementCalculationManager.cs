using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly ConcurrentQueue<SmartPlaneUser> _userWithChangedData;
        private readonly Task _achievementCalculationTask;
        private readonly CancellationTokenSource _achievementCalculationCancelSource;

        #endregion;

        /// <summary>
        /// Creates the Manager using the achievementDetector to find all available achievements.
        /// </summary>
        /// <param name="achievementDetector">Detector used to find the available achievements</param>
        public AchievementCalculationManager(IAchievementCalculatorDetector achievementDetector)
        {
            _achievementCalculators = achievementDetector.FindAllAchievementCalculator().ToList();
            _userWithChangedData = new ConcurrentQueue<SmartPlaneUser>();

            _achievementCalculationCancelSource = new CancellationTokenSource();
            _achievementCalculationTask = new Task(_achievementCalculation, _achievementCalculationCancelSource.Token, TaskCreationOptions.LongRunning);
            _achievementCalculationTask.Start();
        }

        private void _achievementCalculation()
        {
            while (_achievementCalculationCancelSource.IsCancellationRequested == false)
            {
                while (_userWithChangedData.IsEmpty == false)
                {
                    //TODO: Add Logging if dequeTryDequeue failed

                    SmartPlaneUser user;
                    _userWithChangedData.TryDequeue(out user);
                    _calculateAchievementsForUser(user);
                } 

                // Avoid busy-wait
                Thread.Sleep(100);
            }
        }

        private void _calculateAchievementsForUser(SmartPlaneUser user)
        {
            foreach (var achievementCalculator in _achievementCalculators)
            {
                achievementCalculator.CalculateAchievementProgress(user);
            }
        }

        public void UpdateForUser(SmartPlaneUser userWithChangedData)
        {
            // TODO: IAchievementCalculator, property: typeOfAchievement, otherwise it is not possible to check if achievement exists. Alternative: IsAchieved (user) method
            _userWithChangedData.Enqueue(userWithChangedData);
        }

       
    }
}