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
            db.Achievements.RemoveRange(db.Achievements);
            db.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            var db = new AchievementDb();
            db.Achievements.RemoveRange(db.Achievements);
            db.SaveChanges();
        }

        [Test]
        public void Test_adding_Achievements_to_the_DataBase()
        {
            //Arrange 
            var db = new AchievementDb();

            //Act
            const string expected = "Test";
            db.Achievements.Add(new Achievement { Name = expected });
            db.SaveChanges();

            //Assert
            Assert.That(()=>new AchievementDb().Achievements.First().Name, Is.EqualTo(expected));
        }
    }
}
