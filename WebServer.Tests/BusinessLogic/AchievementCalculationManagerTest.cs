using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NSubstitute;
using NUnit.Framework;
using Universial.Test;
using WebServer.BusinessLogic;
using WebServer.DataContext;
using WebServer.Logging;
using WebServer.Models;

namespace WebServer.Tests.BusinessLogic
{
    class AchievementCalculationManagerTest : AchievementTestBase<AchievementCalculationManager>
    {
        private SmartPlaneUser _dummySmartPlaneUser;
        private ICollection<IAchievementCalculator> _calculators;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            _dummySmartPlaneUser = CreateSmartPlaneUser();

            _calculators = new List<IAchievementCalculator> { Substitute.For<IAchievementCalculator>(), Substitute.For<IAchievementCalculator>() };
            var detector = Substitute.For<IAchievementCalculatorDetector>();
            detector.FindAllAchievementCalculator().Returns(_calculators);

            var achievementDbMock = Substitute.For<IAchievementDb>();
            achievementDbMock.GetSmartPlaneUserById(0).ReturnsForAnyArgs(_dummySmartPlaneUser);

            SystemUnderTest = new AchievementCalculationManager(detector, achievementDbMock, Substitute.For<ILoggerFacade>());
        }

        protected override void TearDown()
        {
            base.TearDown();
            SystemUnderTest.Dispose();
        }

        [Test]
        public void Test_if_calls_calculators_for_added_users()
        {
            //Act
            SystemUnderTest.UpdateForUser(_dummySmartPlaneUser.Id);

            //Assert
            Assert.That(() => _calculators.First().Received(1).CalculateAchievementProgress(_dummySmartPlaneUser), Throws.Nothing.After(2000, 30));
        }

        [Test]
        public void Test_if_calls_all_calculators()
        {
            //Act
            SystemUnderTest.UpdateForUser(_dummySmartPlaneUser.Id);

            //Assert
            foreach (var achievementCalculator in _calculators)
            {
                Assert.That(() => achievementCalculator.Received(1).CalculateAchievementProgress(_dummySmartPlaneUser), Throws.Nothing.After(2000, 30));
            }
        }

        [TestCase(0,0)]
        [TestCase(1,1)]
        [TestCase(99,99)]
        [TestCase(100,100)]
        public void Test_if_manager_calculates_the_ranking_points(byte progress,int expectedPoints)
        {
            //Arrange 
            _dummySmartPlaneUser.Achievements.Add(new Achievement { Progress = progress });
            //Act

            SystemUnderTest.UpdateForUser(_dummySmartPlaneUser.Id);

            //Assert
            Assert.That(()=>_dummySmartPlaneUser.RankingPoints,Is.EqualTo(expectedPoints).After(3000,50));
        }

        [Test]
        public void Test_the_calculation_with_many_achievements()
        {
            //Arrange 
            _dummySmartPlaneUser.Achievements.Add(new Achievement { Progress = 100 });
            _dummySmartPlaneUser.Achievements.Add(new Achievement { Progress = 99 });
            _dummySmartPlaneUser.Achievements.Add(new Achievement { Progress = 1 });
            _dummySmartPlaneUser.Achievements.Add(new Achievement { Progress = 0 });

            //Act
            SystemUnderTest.UpdateForUser(_dummySmartPlaneUser.Id);

            //Assert
            Assert.That(() => _dummySmartPlaneUser.RankingPoints, Is.EqualTo(200).After(3000, 50));
        }

        [Test]
        public void Test_if_calculation_of_the_ranking_points_reset_previous_ranking_points()
        {
            //Arrange 
            _dummySmartPlaneUser.Achievements.Add(new Achievement { Progress = 100 });
            _dummySmartPlaneUser.RankingPoints = 1000;

            //Act
            SystemUnderTest.UpdateForUser(_dummySmartPlaneUser.Id);

            //Assert
            Assert.That(() => _dummySmartPlaneUser.RankingPoints, Is.EqualTo(100).After(3000, 50));
        }
    }
}
