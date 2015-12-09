using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using NSubstitute;
using NUnit.Framework;
using Universial.Test;
using WebServer.BusinessLogic;
using WebServer.Controllers;
using WebServer.DataContext;
using WebServer.Models;

namespace WebServer.Tests.Controllers
{
    public class AchievementControllerTest : AchievementTestBase<AchievementController>
    {
        private IAchievementDb _achievementDbMock;
        private SmartPlaneUser _smartPlaneTestUser;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            _achievementDbMock = Substitute.For<IAchievementDb>();
            SystemUnderTest = new AchievementController(_achievementDbMock);

            _smartPlaneTestUser = CreateSmartPlaneUser();
            _achievementDbMock.GetSmartPlaneUserById(0).ReturnsForAnyArgs(_smartPlaneTestUser);
        }

        [Test]
        public void Test_GetAllAchievements()
        {
            //Arrange 
            const string name1 = "AchievementName1";
            const string name2 = "AchievementName2";

            _smartPlaneTestUser.Achievements.Add(new Achievement
            {
                Name = name1
            });
            _smartPlaneTestUser.Achievements.Add(new Achievement
            {
                Name = name2
            });

            //Act
            var result = SystemUnderTest.GetAllAchievements();

            //Assert
            Assert.That(() => result.Count(), Is.EqualTo(2));
            Assert.That(() => result.Any(a=>a.Name.Equals(name1)), Is.True);
            Assert.That(() => result.Any(a=>a.Name.Equals(name2)), Is.True);
        }

        [Test]
        public void Test_GetObtainedAchievements()
        {
            //Arrange 
            const string name1 = "AchievementName1";
            const string name2 = "AchievementName2";

            _smartPlaneTestUser.Achievements.Add(new Achievement
            {
                Name = name1,
                Progress = 99
            });
            _smartPlaneTestUser.Achievements.Add(new Achievement
            {
                Name = name1,
                Progress = 0
            });
            _smartPlaneTestUser.Achievements.Add(new Achievement
            {
                Name = name2,
                Progress = 100
            });

            //Act
            var result = SystemUnderTest.GetObtainedAchievements();

            //Assert
            Assert.That(() => result.Count(), Is.EqualTo(1));
            Assert.That(() => result.First().Name, Is.EqualTo(name2));
        }

        [Test]
        public void Test_GetObtainedAchievements_with_no_obtained_achievements()
        {
            //Arrange 
            const string name1 = "AchievementName1";

            _smartPlaneTestUser.Achievements.Add(new Achievement
            {
                Name = name1,
                Progress = 99
            });
            _smartPlaneTestUser.Achievements.Add(new Achievement
            {
                Name = name1,
                Progress = 0
            });

            //Act
            var result = SystemUnderTest.GetObtainedAchievements();

            //Assert
            Assert.That(() => result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Test_if_GetRankingList_returns_a_sorted_list()
        {
            //Arrange 
            _achievementDbMock.GetAllUser().ReturnsForAnyArgs(info => new []
            {
                new SmartPlaneUser {RankingPoints = 100}, 
                new SmartPlaneUser {RankingPoints = 10}, 
                new SmartPlaneUser {RankingPoints = 99}, 
                new SmartPlaneUser {RankingPoints = 0}, 
                new SmartPlaneUser {RankingPoints = 1} 
            });

            //Act
            var result = SystemUnderTest.GetRankingList().ToList();

            //Assert
            Assert.That(()=>result[0].RankingPoints,Is.EqualTo(100));
            Assert.That(()=>result[1].RankingPoints,Is.EqualTo(99));
            Assert.That(()=>result[2].RankingPoints,Is.EqualTo(10));
            Assert.That(()=>result[3].RankingPoints,Is.EqualTo(1));
            Assert.That(()=>result[4].RankingPoints,Is.EqualTo(0));
        }

        [Test]
        public void Test_if_GetRankingList_dont_ignores_to_equal_rank_points()
        {
            //Arrange 
            _achievementDbMock.GetAllUser().ReturnsForAnyArgs(info => new[]
            {
                new SmartPlaneUser {RankingPoints = 100},
                new SmartPlaneUser {RankingPoints = 100}
            });

            //Act
            var result = SystemUnderTest.GetRankingList().ToList();

            //Assert
            Assert.That(() => result[0].RankingPoints, Is.EqualTo(100));
            Assert.That(() => result[1].RankingPoints, Is.EqualTo(100));
        }
    }
}
