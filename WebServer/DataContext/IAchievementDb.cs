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
        /// Get a SmartPlaneUser by íts userId
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
        /// Returns all SmartplaneUsers saved in the database
        /// </summary>
        void ResetAllData();
        IEnumerable<SmartPlaneUser> GetAllUser();
    }
}
