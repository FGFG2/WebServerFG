using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using WebGrease.Css.Extensions;
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

        public void ResetAllData()
        {
            var db = new AchievementDb();
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [RudderDatas]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [MotorDatas]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [ConnectedDatas]");
            var user = db.SmartPlaneUsers.FirstOrDefault();
            user.Achievements.ForEach(x=>x.Progress = 0);
            db.SaveChanges();
        }

        private void _removeAll<T>(IList<T> datas)
        {
            for (int i = 0; i < datas.Count; i++)
            {
                datas.RemoveAt(0);
            }
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}