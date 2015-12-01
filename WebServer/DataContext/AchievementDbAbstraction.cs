using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using WebGrease.Css.Extensions;
using WebServer.Models;

namespace WebServer.DataContext
{
    public class AchievementDbAbstraction : IAchievementDb
    {
        private readonly AchievementDb _db;

        public AchievementDbAbstraction()
        {
            _db = new AchievementDb();
        }

        public SmartPlaneUser GetSmartPlaneUserById(int userId)
        {
            return _db.SmartPlaneUsers.FirstOrDefault(/*u => u.Id == userId*/);
        }

        public IEnumerable<LogEntry> GetAllLogEntries()
        {
            return _db.LogEntries;
        }

        public void SaveChanges()
        {
            _db.SaveChanges();
        }

        public void ResetAllData()
        {
            var db = new AchievementDb();
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [RudderDatas]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [MotorDatas]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [ConnectedDatas]");
            var user = db.SmartPlaneUsers.FirstOrDefault();
            user.Achievements.ForEach(x => x.Progress = 0);
            db.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AchievementDbAbstraction()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
        }
    }
}