using WebServer.Models;

namespace WebServer.BusinessLogic
{
    /// <summary>
    /// Handles the calculations of achievements for the users. 
    /// </summary>
    public interface IAchievementCalculationManager
    {
        /// <summary>
        /// Signals that the achievements of the passed user needs to be re-evaluated.
        /// </summary>
        /// <param name="userWithChangedData">User that needs to be evaluated.</param>
        void UpdateForUser(SmartPlaneUser userWithChangedData);
    }
}