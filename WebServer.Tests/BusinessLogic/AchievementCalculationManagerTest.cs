using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Universial.Test;
using WebServer.BusinessLogic;
using WebServer.DataContext;
using WebServer.Models;

namespace WebServer.Tests.BusinessLogic
{
    class AchievementCalculationManagerTest : TestBase<AchievementCalculationManager>
    {
        private SmartPlaneUser _dummySmartPlaneUser;
        private ICollection<IAchievementCalculator> _calculators;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            _dummySmartPlaneUser = new SmartPlaneUser();

            _calculators = new List<IAchievementCalculator> { Substitute.For<IAchievementCalculator>(), Substitute.For<IAchievementCalculator> ()};
            var detector = Substitute.For<IAchievementCalculatorDetector>();
            detector.FindAllAchievementCalculator().Returns(_calculators);
            var achievementDbMock = Substitute.For<IAchievementDb>();

            SystemUnderTest = new AchievementCalculationManager(detector, achievementDbMock);
        }

        [Test]
        public void Test_if_calls_calculators_for_added_users()
        {
            //Act
            SystemUnderTest.UpdateForUser(_dummySmartPlaneUser);

            //Assert
            Assert.That(() => _calculators.First().Received(1).CalculateAchievementProgress(_dummySmartPlaneUser), Throws.Nothing.After(2000, 30));
        }

        [Test]
        public void Test_if_calls_all_calculators()
        {
            //Act
            SystemUnderTest.UpdateForUser(_dummySmartPlaneUser);

            //Assert
            foreach (var achievementCalculator in _calculators)
            {
                Assert.That(()=> achievementCalculator.Received(1).CalculateAchievementProgress(_dummySmartPlaneUser), Throws.Nothing.After(2000,30));
            }
        }

        [Test]
        public void Test_if_only_calls_calculators_for_added_users()
        {
            //Act
            SystemUnderTest.UpdateForUser(new SmartPlaneUser());

            //Assert
            Assert.That(() => _calculators.First().Received(0).CalculateAchievementProgress(_dummySmartPlaneUser), Throws.Nothing.After(2000, 30));
        }
    }
}
