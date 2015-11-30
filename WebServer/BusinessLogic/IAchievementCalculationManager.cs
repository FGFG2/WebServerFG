using System;
using WebServer.Models;

namespace WebServer.BusinessLogic
{
    /// <summary>
    /// Handles the calculations of achievements for the users. 
    /// </summary>
    public interface IAchievementCalculationManager : IDisposable
    {
        /// <summary>
        /// Signals that the achievements of the passed user needs to be re-evaluated.
        /// </summary>
        /// <param name="userId">The Id of the user which needs to be updated</param>
        void UpdateForUser(int userId);
    }
}