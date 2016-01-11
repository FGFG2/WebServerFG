using System.Linq;
using NSubstitute;
using NUnit.Framework;
using WebServer.BusinessLogic.AchievementCalculators;
using WebServer.Logging;
using WebServer.Models;

namespace WebServer.Tests.BusinessLogic.AchievementCalculators
{
    public class ConnectionAchievementCalculatorTest : AchievementTestBase<ConnectionAchievementCalculator>
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            SystemUnderTest = new ConnectionAchievementCalculator(Substitute.For<ILoggerFacade>());
        }

        [TestCase(0,0)]
        [TestCase(1,1)]
        [TestCase(99,99)]
        [TestCase(100,100)]
        [TestCase(101,100)]
        public void Test_if_calculator_calculates_a_achievement_correct(int connectionTimes,int progress)
        {
            //Arrange 
            var user = CreateSmartPlaneUser();
            for (var i = 0; i < connectionTimes; i++)
            {
                user.ConnectedDatas.Add(new ConnectedData { Value = true, TimeStamp = i }); 
            }

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(()=>user.Achievements.First().Progress,Is.EqualTo(progress));
        }

        [Test]
        public void Test_if_calculator_calculates_a_achievement_correct_when_the_user_does_not_have_enogh()
        {
            //Arrange 
            var user = CreateSmartPlaneUser();
            user.ConnectedDatas.Add(new ConnectedData { Value = false, TimeStamp = 0 });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.First().Progress, Is.EqualTo(0));
        }
    }
}
