using System.Linq;
using NUnit.Framework;
using WebServer.DataContext;
using WebServer.Models;

namespace WebServer.Tests.DataContext
{
    [Explicit]
    public class AchievementDbTest
    {
        [SetUp]
        public void SetUp()
        {
            var db = new AchievementDb();
            if (db.SmartPlaneUsers.Any())
            {
                db.SmartPlaneUsers.RemoveRange(db.SmartPlaneUsers);
                db.SaveChanges(); 
            }
        }

        [TearDown]
        public void TearDown()
        {
            var db = new AchievementDb();
            if (db.SmartPlaneUsers.Any())
            {
                db.SmartPlaneUsers.RemoveRange(db.SmartPlaneUsers);
                db.SaveChanges();
            }
        }

        [Test]
        public void Test_adding_Achievements_to_the_DataBase()
        {
            //Arrange 
            var db = new AchievementDb();

            //Act
            db.SmartPlaneUsers.Add(new SmartPlaneUser());
            db.SaveChanges();
            db.SmartPlaneUsers.First().Achievements.Add(new Achievement());
            db.SmartPlaneUsers.First().ConnectedDatas.Add(new ConnectedData());
            db.SmartPlaneUsers.First().MotorDatas.Add(new MotorData());
            db.SmartPlaneUsers.First().RudderDatas.Add(new RudderData());
            db.SaveChanges();

            //Assert
            Assert.That(()=>new AchievementDb().SmartPlaneUsers.Any(), Is.True);
            Assert.That(()=>new AchievementDb().SmartPlaneUsers.First().Achievements.Any(), Is.True);
            Assert.That(()=>new AchievementDb().SmartPlaneUsers.First().ConnectedDatas.Any(), Is.True);
            Assert.That(()=>new AchievementDb().SmartPlaneUsers.First().MotorDatas.Any(), Is.True);
            Assert.That(()=>new AchievementDb().SmartPlaneUsers.First().RudderDatas.Any(), Is.True);
        }
    }
}
