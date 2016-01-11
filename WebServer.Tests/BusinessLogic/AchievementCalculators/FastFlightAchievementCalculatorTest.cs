using System.Linq;
using NSubstitute;
using NUnit.Framework;
using WebServer.BusinessLogic.AchievementCalculators;
using WebServer.Logging;
using WebServer.Models;

namespace WebServer.Tests.BusinessLogic.AchievementCalculators
{
    public class FastFlightAchievementCalculatorTest : AchievementTestBase<FastFlightAchievementCalculator>
    {
        public const float OnePercentStep = FastFlightAchievementCalculator.OnePercentStep;
        public const int NeededDurationWithMaxMotor = FastFlightAchievementCalculator.NeededDurationWithMaxMotor;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            SystemUnderTest = new FastFlightAchievementCalculator(Substitute.For<ILoggerFacade>());
        }

        [TestCase(0)]
        [TestCase(10)]
        [TestCase(20)]
        [TestCase(30)]
        [TestCase(40)]
        [TestCase(50)]
        [TestCase(60)]
        [TestCase(70)]
        [TestCase(80)]
        [TestCase(90)]
        [TestCase(100)]
        public void Test_if_calculator_calculates_achievement_correct(int percent)
        {
            //Arrange 
            const long timeBetween = 100000;
            var user = CreateSmartPlaneUser();
            for (int i = 0; i < percent/10; i++)
            {
                user.ConnectedDatas.Add(new ConnectedData { TimeStamp = i * timeBetween, Value = true });
                user.MotorDatas.Add(new MotorData { TimeStamp = i * timeBetween, Value = 255 });
                user.MotorDatas.Add(new MotorData { TimeStamp = i * timeBetween + NeededDurationWithMaxMotor, Value = 0 });
            }
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 20 * timeBetween, Value = false });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.First().Progress, Is.EqualTo(percent));
        }

        [Test]
        public void Test_if_calculator_calculates_achievement_correct_with_over_10_succeded_flights()
        {
            //Arrange 
            const long timeBetween = 100000;
            var user = CreateSmartPlaneUser();
            for (int i = 0; i < 11; i++)
            {
                user.ConnectedDatas.Add(new ConnectedData { TimeStamp = i * timeBetween, Value = true });
                user.MotorDatas.Add(new MotorData { TimeStamp = i * timeBetween, Value = 255 });
                user.MotorDatas.Add(new MotorData { TimeStamp = i * timeBetween + NeededDurationWithMaxMotor, Value = 0 });
            }
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 20 * timeBetween, Value = false });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.First().Progress, Is.EqualTo(100));
        }
    }
}
