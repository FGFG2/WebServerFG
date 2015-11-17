using System.Linq;
using NUnit.Framework;
using WebServer.BusinessLogic.AchievementCalculators;
using WebServer.Models;

namespace WebServer.Tests.BusinessLogic.AchievementCalculators
{
    class FastFlightAchievementCalculatorTest : AchievementTestBase<FastFlightAchievementCalculator>
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            SystemUnderTest = new FastFlightAchievementCalculator();
        }
       
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
        public void Test_if_achievement_is_correctly_calculated(int progress)
        {
            //Arrange
            var user = CreateSmartPlaneUser();
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, IsConnected = true });
            for (int i = 0; i < progress/10; i++)
            {                
                user.MotorDatas.Add(new MotorData { TimeStamp = 0, Value = 25 });
                user.MotorDatas.Add(new MotorData { TimeStamp = 1000, Value = 255 });
                user.MotorDatas.Add(new MotorData { TimeStamp = 15000, Value = 255 });
                user.MotorDatas.Add(new MotorData { TimeStamp = 15000, Value = 25 });
            }
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 15000, IsConnected = false });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.First().Progress, Is.EqualTo(progress));
        }

        [Test]
        public void Test_if_no_achievement_is_added_with_a_break_between_two_flights()
        {
            //Arrange
            var user = CreateSmartPlaneUser();
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, IsConnected = true });
            user.MotorDatas.Add(new MotorData { TimeStamp = 0, Value = 25 });
            //1st Achievement
            user.MotorDatas.Add(new MotorData { TimeStamp = 500, Value = 255 });
            user.MotorDatas.Add(new MotorData { TimeStamp = 800, Value = 255 });
            user.MotorDatas.Add(new MotorData { TimeStamp = 8000, Value = 255 });
            user.MotorDatas.Add(new MotorData { TimeStamp = 5000, Value = 0 });
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 6000, IsConnected = false });

            //2nd achievement
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 7000, IsConnected = true });
            user.MotorDatas.Add(new MotorData { TimeStamp = 8000, Value = 255 });
            user.MotorDatas.Add(new MotorData { TimeStamp = 9000, Value = 255 });
            user.MotorDatas.Add(new MotorData { TimeStamp = 15000, Value = 255 });
            user.MotorDatas.Add(new MotorData { TimeStamp = 20000, Value = 0 });
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 21000, IsConnected = false });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.First().Progress, Is.EqualTo(20));
        }

        [Test]
        public void Test_if_calculator_reacts_normal_with_unnormal_motordata()
        {
            //Arrange
            var user = CreateSmartPlaneUser();
            user.MotorDatas.Add(new MotorData { TimeStamp = 0, Value = 255 });           
            user.MotorDatas.Add(new MotorData { TimeStamp = 500, Value = 255 });
            user.MotorDatas.Add(new MotorData { TimeStamp = 5000, Value = 255 });
            user.MotorDatas.Add(new MotorData { TimeStamp = 10000, Value = 255 });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.First().Progress, Is.EqualTo(0));
        }

        [Test]
        public void Test_if_calculator_reacts_normal_with_single_peaks()
        {
            //Arrange
            var user = CreateSmartPlaneUser();
            user.MotorDatas.Add(new MotorData { TimeStamp = 0, Value = 0 });
            user.MotorDatas.Add(new MotorData { TimeStamp = 500, Value = 100 });
            user.MotorDatas.Add(new MotorData { TimeStamp = 5000, Value = 255 });
            user.MotorDatas.Add(new MotorData { TimeStamp = 10000, Value = 50 });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.First().Progress, Is.EqualTo(0));
        }
    }
}
