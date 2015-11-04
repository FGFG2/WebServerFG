using System.Linq;
using NUnit.Framework;
using Universial.Test;
using WebServer.BusinessLogic;

namespace WebServer.Tests.BusinessLogic
{
    public class AchievementCalculatorDetectorTest : TestBase<AchievementCalculatorDetector>
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            SystemUnderTest = new AchievementCalculatorDetector();
        }

        [Test]
        public void Test_if_finds_AchievementCalculators()
        {
            //Act
            var result = SystemUnderTest.FindAllAchievementCalculator();

            //Assert
            var containsAchievement = result.Any(x => x.GetType() == typeof (ConnectionAchievementCalculator));
            Assert.That(() => containsAchievement, Is.True);
        }
    }
}
