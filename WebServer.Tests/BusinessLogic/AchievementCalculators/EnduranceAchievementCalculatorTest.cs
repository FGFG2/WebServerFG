using System.Linq;
using NUnit.Framework;
using WebServer.BusinessLogic.AchievementCalculators;
using WebServer.Models;

namespace WebServer.Tests.BusinessLogic.AchievementCalculators
{
    public class EnduranceAchievementCalculatorTest : AchievementTestBase<EnduranceAchievementCalculator>
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            SystemUnderTest = new EnduranceAchievementCalculator();
        }

        [TestCase(0, 600000, 100)]//Test border
        [TestCase(0, 300000, 50)]
        [TestCase(0, 0, 0)]//Test border
        [TestCase(1000000, 1360000, 60)]//Test that only the difference matters
        [TestCase(0, 599999, 99)]//Test border and rounding
        [TestCase(0, 5999, 0)]//Test border and rounding
        [TestCase(0, 6000, 1)]//Test border and rounding
        public void Test_if_calculator_calculates_a_achievement_correct(int startTime, int endTime, byte progress)
        {
            //Arrange 
            var user = CreateSmartPlaneUser();
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = startTime, IsConnected = true });
            user.MotorDatas.Add(new MotorData { TimeStamp = startTime, Value = 1 });
            user.MotorDatas.Add(new MotorData { TimeStamp = endTime, Value = 0 });
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = endTime, IsConnected = false });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.First().Progress, Is.EqualTo(progress));
        }

        [Test]
        public void Test_if_calculator_recognizes_two_flights_as_two_flights()
        {
            //Arrange 
            var user = CreateSmartPlaneUser();
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, IsConnected = true });
            user.MotorDatas.Add(new MotorData { TimeStamp = 0, Value = 1 });
            user.MotorDatas.Add(new MotorData { TimeStamp = 300000, Value = 0 });
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 300000, IsConnected = false });//First flight is over
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 300001, IsConnected = true });//Start the secound flight
            user.MotorDatas.Add(new MotorData { TimeStamp = 300002, Value = 1 });
            user.MotorDatas.Add(new MotorData { TimeStamp = 600002, Value = 0 });
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 600002, IsConnected = false });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.First().Progress, Is.EqualTo(50));
        }

        [Test]
        public void Test_if_calculator_recognizes_one_flights_as_one_flights()
        {
            //Arrange 
            var user = CreateSmartPlaneUser();
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, IsConnected = true });
            user.MotorDatas.Add(new MotorData { TimeStamp = 0, Value = 1 });
            user.MotorDatas.Add(new MotorData { TimeStamp = 300000, Value = 0 });//Flight is not over
            user.MotorDatas.Add(new MotorData { TimeStamp = 300002, Value = 1 });
            user.MotorDatas.Add(new MotorData { TimeStamp = 600000, Value = 0 });
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 600000, IsConnected = false });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.First().Progress, Is.EqualTo(100));
        }

        [Test]
        public void Test_if_calculator_reacts_normal_whith_only_one_MotorData()
        {
            //Arrange 
            var user = CreateSmartPlaneUser();
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, IsConnected = true });
            user.MotorDatas.Add(new MotorData { TimeStamp = 0, Value = 1 });
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 600001, IsConnected = false });
            
            //Assert
            Assert.That(() => SystemUnderTest.CalculateAchievementProgress(user),Throws.Nothing);
        }

        [Test]
        public void Test_if_calculator_uses_the_value_of_MotorData()
        {
            //Arrange 
            var user = CreateSmartPlaneUser();
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, IsConnected = true });
            user.MotorDatas.Add(new MotorData { TimeStamp = 0, Value = 0 });
            user.MotorDatas.Add(new MotorData { TimeStamp = 300000, Value = 1 });
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 300000, IsConnected = false });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.First().Progress, Is.EqualTo(0));
        }

        [Test]
        public void Test_if_calculator_Ignores_flights_with_no_Motor_out_data()
        {
            //Arrange 
            var user = CreateSmartPlaneUser();
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, IsConnected = true });
            user.MotorDatas.Add(new MotorData { TimeStamp = 0, Value = 1 });
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 300000, IsConnected = false });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.First().Progress, Is.EqualTo(0));
        }

        [Test]
        public void Test_if_calculator_Ignores_MotorDatas_which_are_out_of_the_connection()
        {
            //Arrange 
            var user = CreateSmartPlaneUser();
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 300000, IsConnected = true });
            user.MotorDatas.Add(new MotorData { TimeStamp = 300000, Value = 1 });
            user.MotorDatas.Add(new MotorData { TimeStamp = 600000, Value = 0 });
            user.MotorDatas.Add(new MotorData { TimeStamp = 0, Value = 1 });
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 600000, IsConnected = false });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.First().Progress, Is.EqualTo(50));
        }
    }
}
