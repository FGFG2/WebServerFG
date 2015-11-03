using System.Linq;
using NUnit.Framework;
using WebServer.BusinessLogic;
using WebServer.Models;

namespace WebServer.Tests.BusinessLogic
{
    public class ConnectionAchievementCalculatorTest : AchievementTestBase<ConnectionAchievementCalculator>
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            SystemUnderTest = new ConnectionAchievementCalculator();
        }
        
        [Test]
        public void Test_if_calculator_adds_a_achievement_to_the_user()
        {
            //Arrange 
            var user = CreateSmartPlaneUser();

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(()=>user.Achievements.Any(a=>a.Name.Equals(ConnectionAchievementCalculator.AchievementName)),Is.True);
        }

        [Test]
        public void Test_if_calculator_calculates_a_achievement_correct()
        {
            //Arrange 
            var user = CreateSmartPlaneUser();
            user.ConnectedDatas.Add(new ConnectedData { IsConnected = true, TimeStamp = 0 });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(()=>user.Achievements.First().Progress,Is.EqualTo(100));
        }

        [Test]
        public void Test_if_calculator_calculates_a_achievement_correct_when_the_user_does_not_have_enogh()
        {
            //Arrange 
            var user = CreateSmartPlaneUser();
            user.ConnectedDatas.Add(new ConnectedData { IsConnected = false, TimeStamp = 0 });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.First().Progress, Is.EqualTo(0));
        }
    }
}
