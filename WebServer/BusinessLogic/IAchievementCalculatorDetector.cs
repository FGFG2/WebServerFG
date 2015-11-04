using System.Collections.Generic;

namespace WebServer.BusinessLogic
{
    /// <summary>
    /// Searches for all available AchievementCalculator.
    /// </summary>
    public interface IAchievementCalculatorDetector
    {
        /// <summary>
        /// Searches for all available  IAchievementCalculator and instantiates all of them.
        /// </summary>
        /// <returns>Available IAchievementCalculator instances. </returns>
        IEnumerable<IAchievementCalculator> FindAllAchievementCalculator();
    }
}