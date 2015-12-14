using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using WebServer.Models;

namespace WebServer.DataContext
{
    public interface IAchievementDb : IDisposable
    {
        /// <summary>
        /// Adds a SmartPlaneUser to the database.
        /// </summary>
        /// <returns></returns>
        void AddNewSmartplaneUser(string id);

        /// <summary>
        /// Get a SmartPlaneUser by its related application userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        SmartPlaneUser GetSmartPlaneUserByApplicationUserId(string userId);

        /// <summary>
        /// Get a SmartPlaneUser by its userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        SmartPlaneUser GetSmartPlaneUserById(int userId);

        /// <summary>
        /// Returns all log saved in the database
        /// </summary>
        IEnumerable<LogEntry> GetAllLogEntries();

        /// <summary>
        /// Save all changes to the Database
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Removes all database entries.
        /// </summary>
        void ResetAllData();
        /// <summary>
        /// Returns all SmartplaneUsers saved in the database
        /// </summary>
        IEnumerable<SmartPlaneUser> GetAllUser();
    }
}
