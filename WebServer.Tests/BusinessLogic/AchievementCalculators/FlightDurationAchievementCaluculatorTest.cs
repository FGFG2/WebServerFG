using System.Linq;
using NSubstitute;
using NUnit.Framework;
using WebServer.BusinessLogic.AchievementCalculators;
using WebServer.Logging;
using WebServer.Models;

namespace WebServer.Tests.BusinessLogic.AchievementCalculators
{
    public class FlightDurationAchievementCaluculatorrTest : AchievementTestBase<FlightDurationAchievementCaluculator>
    {
        public const int OnePercentStep = FlightDurationAchievementCaluculator.OnePercentStep;
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            SystemUnderTest = new FlightDurationAchievementCaluculator(Substitute.For<ILoggerFacade>());
        }

        [TestCase(0 * OnePercentStep, 0)]
        [TestCase(0 * OnePercentStep + 1, 0)]
        [TestCase(1 * OnePercentStep, 1)]
        [TestCase(99 * OnePercentStep, 99)]
        [TestCase(100 * OnePercentStep, 100)]
        [TestCase(100 * OnePercentStep - 1, 99)]
        [TestCase(101 * OnePercentStep, 100)]
        public void Test_if_calculator_calculates_a_achievement_correct(int endTime, int progress)
        {
            //Arrange 
            const int startTime = 0;
            var user = CreateSmartPlaneUser();

            user.ConnectedDatas.Add(new ConnectedData { Value = true, TimeStamp = startTime });
            user.MotorDatas.Add(new MotorData { Value = 1, TimeStamp = startTime });
            user.MotorDatas.Add(new MotorData { Value = 0, TimeStamp = endTime });
            user.ConnectedDatas.Add(new ConnectedData { Value = false, TimeStamp = endTime });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.First().Progress, Is.EqualTo(progress));
        }

        [Test]
        public void Test_if_calculator_calculates_a_achievement_correct_whith_multible_flights()
        {
            //Arrange 
            var user = CreateSmartPlaneUser();

            user.ConnectedDatas.Add(new ConnectedData { Value = true, TimeStamp = 0 });
            user.MotorDatas.Add(new MotorData { Value = 1, TimeStamp = 0 });
            user.ConnectedDatas.Add(new ConnectedData { Value = false, TimeStamp = 10 * OnePercentStep });

            user.ConnectedDatas.Add(new ConnectedData { Value = true, TimeStamp = 10 * OnePercentStep +1});
            user.MotorDatas.Add(new MotorData { Value = 1, TimeStamp = 10 * OnePercentStep +1 });
            user.ConnectedDatas.Add(new ConnectedData { Value = false, TimeStamp = 50 * OnePercentStep +1 });
            
            user.ConnectedDatas.Add(new ConnectedData { Value = true, TimeStamp = 50 * OnePercentStep +2 });
            user.MotorDatas.Add(new MotorData { Value = 1, TimeStamp = 50 * OnePercentStep +2 });
            user.ConnectedDatas.Add(new ConnectedData { Value = false, TimeStamp = 100 * OnePercentStep +2 });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.First().Progress, Is.EqualTo(100));
        }

        [Test]
        public void Test_if_calculator_calculates_a_achievement_correct_when_the_user_does_not_have_enogh()
        {
            //Arrange 
            var user = CreateSmartPlaneUser();

            user.ConnectedDatas.Add(new ConnectedData { Value = true, TimeStamp = 0 });
            user.MotorDatas.Add(new MotorData { Value = 0, TimeStamp = 0 });
            user.ConnectedDatas.Add(new ConnectedData { Value = false, TimeStamp = 1 });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.First().Progress, Is.EqualTo(0));
        }

        [Test]
        public void Test_if_calculator_calculates_a_achievement_correct_when_no_data()
        {
            //Arrange 
            var user = CreateSmartPlaneUser();

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.First().Progress, Is.EqualTo(0));
        }
    }
}
