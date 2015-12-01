using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.ServiceLocation;
using NLog;
using NLog.Targets;
using WebServer.DataContext;
using WebServer.Models;

namespace WebServer.Logging
{
    [Target(nameof(DatabaseLogger))]
    public class DatabaseLogger : TargetWithLayout
    {
        private readonly AchievementDb _achievementDb;

        public DatabaseLogger()
        {
            _achievementDb = new AchievementDb();
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var entry = new LogEntry {TimeStamp = logEvent.TimeStamp,Message = logEvent.Message};
            _achievementDb.LogEntries.Add(entry);
            _achievementDb.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            _achievementDb.Dispose();
        }
    }
}