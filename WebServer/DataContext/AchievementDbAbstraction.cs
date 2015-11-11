using System;
using System.Linq;
using WebServer.Models;

namespace WebServer.DataContext
{
    public class AchievementDbAbstraction : IAchievementDb, IDisposable
    {
        private AchievementDb _db;
        
        public SmartPlaneUser GetSmartPlaneUserById(int userId)
        {
            if (_db == null)
            {
                _db = new AchievementDb();
            }
            return _db.SmartPlaneUsers.FirstOrDefault(/*u => u.Id == userId*/);
        }

        public void SaveChanges()
        {
            if (_db == null)
            {
                return;
            }
            _db.SaveChanges();
            _db.Dispose();
            _db = null;
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}