using System;
using System.Linq;
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

        public void SaveChanges()
        {
            _db.SaveChanges();
        }
    }
}