using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Universial.Test;
using WebServer.BusinessLogic;
using WebServer.BusinessLogic.AchievementCalculators;
using WebServer.Logging;

namespace WebServer.Tests.BusinessLogic
{
    public class AchievementCalculatorDetectorTest : TestBase<AchievementCalculatorDetector>
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            SystemUnderTest = new AchievementCalculatorDetector(Substitute.For<ILoggerFacade>());
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
