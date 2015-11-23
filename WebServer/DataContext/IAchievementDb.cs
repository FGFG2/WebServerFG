using WebServer.Models;

namespace WebServer.DataContext
{
    public interface IAchievementDb
    {
        /// <summary>
        /// Get a SmartPlaneUser by íts userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        SmartPlaneUser GetSmartPlaneUserById(int userId);

        /// <summary>
        /// Save all changes to the Database
        /// </summary>
        void SaveChanges();

        void ResetAllData();
    }
}
