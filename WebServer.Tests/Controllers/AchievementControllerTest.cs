﻿using System.Linq;
using System.Net;
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
        private IAchievementCalculationManager _achievementCalculatorMock;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            _achievementDbMock = Substitute.For<IAchievementDb>();
            _achievementCalculatorMock = Substitute.For<IAchievementCalculationManager>();
            SystemUnderTest = new AchievementController(_achievementDbMock,_achievementCalculatorMock);

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
    }
}
