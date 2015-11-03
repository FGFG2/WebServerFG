using WebServer.Models;

namespace WebServer.BusinessLogic
{
    /// <summary>
    /// Interface for calculators defining one single achievement. Each achievement requires its own calculator.
    /// </summary>
    public interface IAchievementCalculator
    {
        /// <summary>
        /// Evaluates based on the CurrentUser if he fulfills the goals and updates the database.
        /// </summary>
        /// <param name="targetUser">The user that needs to be evaluated.</param>
        void CalculateAchievementProgress(SmartPlaneUser targetUser);
    }
}