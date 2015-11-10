using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebServer.DataContext;
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
        private readonly Task _achievementCalculationTask;
        private readonly CancellationTokenSource _achievementCalculationCancelSource;
        private readonly IAchievementDb _achievementDb;

        #endregion;

        /// <summary>
        /// Creates the Manager using the achievementDetector to find all available achievements.
        /// </summary>
        /// <param name="achievementDetector">Detector used to find the available achievements</param>
        /// <param name="achievementDb"></param>
        public AchievementCalculationManager(IAchievementCalculatorDetector achievementDetector, IAchievementDb achievementDb)
        {
            _achievementDb = achievementDb;
            _achievementCalculators = achievementDetector.FindAllAchievementCalculator().ToList();
            _userWithChangedData = new ConcurrentDictionary<SmartPlaneUser,byte>();

            _achievementCalculationCancelSource = new CancellationTokenSource();
            _achievementCalculationTask = new Task(_achievementCalculation, _achievementCalculationCancelSource.Token, TaskCreationOptions.LongRunning);
            _achievementCalculationTask.Start();
        }

        private void _achievementCalculation()
        {
            while (_achievementCalculationCancelSource.IsCancellationRequested == false)
            {
                var usersToUpdate = _getUsersToUpdate();
                foreach (var user in usersToUpdate)
                { 
                    _calculateAchievementsForUser(user);
                }

                _achievementDb.SaveChanges();

                // Avoid busy-wait, Calculate each seconds to limit the load for the server.
                Thread.Sleep(1000);
            }
        }

        private IEnumerable<SmartPlaneUser> _getUsersToUpdate()
        {
            //dump keys of concurrent dictionary to use them outside synchronized blocks. 
            //Clear the dictionary, so that already updated user are not going to be updated again when not needed.
            IEnumerable<SmartPlaneUser> users;
            lock (_userWithChangedData) 
            {
                users = _userWithChangedData.Keys;
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


        public void UpdateForUser(int userId)
        {
            lock (_userWithChangedData)
            {
                _userWithChangedData.AddOrUpdate(_achievementDb.GetSmartPlaneUserById(userId), 0, (key, oldValue) => 0);
            }
        }
    }
}