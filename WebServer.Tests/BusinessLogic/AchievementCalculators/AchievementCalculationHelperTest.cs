using System;
using System.Linq;
using NUnit.Framework;
using WebServer.BusinessLogic.AchievementCalculators;
using WebServer.Models;

namespace WebServer.Tests.BusinessLogic.AchievementCalculators
{
    public class AchievementCalculationHelperTest : AchievementTestBase<SmartPlaneUser>
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            SystemUnderTest = CreateSmartPlaneUser();
        }

        #region GetEndAndStartTimesOfAllConnections
        [Test]
        public void Test_if_GetEndAndStartTimesOfAllConnections_returns_correct_values_for_one_connection()
        {
            //Arrange
            const int startTime = 0;
            const int endTime = 10;
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = startTime, IsConnected = true });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = endTime, IsConnected = false });

            //Act
            var result = AchievementCalculationHelper.GetEndAndStartTimesOfAllConnections(SystemUnderTest);

            //Assert
            Assert.That(() => result.First().Item1, Is.EqualTo(startTime));
            Assert.That(() => result.First().Item2, Is.EqualTo(endTime));
        }

        [Test]
        public void Test_if_GetEndAndStartTimesOfAllConnections_returns_correct_values_for_multiple_connection()
        {
            //Arrange

            var startTimes = new[] { 0, 10, 100, 1000 };
            var endTimes = new[] { 9, 99, 999, 1001 };
            for (var i = 0; i < startTimes.Length; i++)
            {
                SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = startTimes[i], IsConnected = true });
                SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = endTimes[i], IsConnected = false });
            }

            //Act
            var result = AchievementCalculationHelper.GetEndAndStartTimesOfAllConnections(SystemUnderTest);

            //Assert
            for (var i = 0; i < startTimes.Length; i++)
            {
                Assert.That(() => result.ToList()[i].Item1, Is.EqualTo(startTimes[i]));
                Assert.That(() => result.ToList()[i].Item2, Is.EqualTo(endTimes[i]));
            }
        }

        [Test]
        public void Test_if_GetEndAndStartTimesOfAllConnections_returns_no_values_with_no_end_Connection()
        {
            //Arrange
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, IsConnected = true });

            //Act
            var result = AchievementCalculationHelper.GetEndAndStartTimesOfAllConnections(SystemUnderTest);

            //Assert
            Assert.That(() => result.Any(), Is.False);
        }

        [Test]
        public void Test_if_GetEndAndStartTimesOfAllConnections_returns_no_values_with_no_start_Connection()
        {
            //Arrange
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = 1000, IsConnected = false });

            //Act
            var result = AchievementCalculationHelper.GetEndAndStartTimesOfAllConnections(SystemUnderTest);

            //Assert
            Assert.That(() => result.Any(), Is.False);
        }

        [Test]
        public void Test_if_GetEndAndStartTimesOfAllConnections_returns_correct_values_for_one_connection_with_two_start_connections()
        {
            //Arrange
            const int startTime = 0;
            const int endTime = 10;
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = startTime, IsConnected = true });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = startTime + 1, IsConnected = true });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = endTime, IsConnected = false });

            //Act
            var result = AchievementCalculationHelper.GetEndAndStartTimesOfAllConnections(SystemUnderTest);

            //Assert
            Assert.That(() => result.Count(), Is.EqualTo(1));
            Assert.That(() => result.First().Item1, Is.EqualTo(startTime));
            Assert.That(() => result.First().Item2, Is.EqualTo(endTime));
        }

        [Test]
        public void Test_if_GetEndAndStartTimesOfAllConnections_returns_correct_values_for_one_connection_with_two_end_connections()
        {
            //Arrange
            const int startTime = 0;
            const int endTime = 10;
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = startTime, IsConnected = true });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = endTime, IsConnected = false });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = endTime + 1, IsConnected = false });

            //Act
            var result = AchievementCalculationHelper.GetEndAndStartTimesOfAllConnections(SystemUnderTest);

            //Assert
            Assert.That(() => result.Count(), Is.EqualTo(1));
            Assert.That(() => result.First().Item1, Is.EqualTo(startTime));
            Assert.That(() => result.First().Item2, Is.EqualTo(endTime));
        }
        #endregion

        #region CalculateFlightDuration

        [Test]
        public void Test_if_CalculateFlightDuration_uses_MotorDatas_to_calclate_flight_duration()
        {
            //Arrange 
            const int startTime = 0;
            const int endTime = 100000;
            const int duration = 1000;

            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime, Value = 1 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime + duration, Value = 0 });

            //Act
            var result = AchievementCalculationHelper.CalculateFlightDuration(startTime, endTime, SystemUnderTest);

            //Assert
            Assert.That(() => result, Is.EqualTo(duration));
        }

        [Test]
        public void Test_if_CalculateFlightDuration_with_many_MototDatas_in_range()
        {
            //Arrange 
            const int startTime = 0;
            const int endTime = 100000;
            const int duration = 1000;

            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime, Value = 1 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime + 1, Value = 1 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime + 2, Value = 1 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime + 3, Value = 1 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime + duration, Value = 0 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime + duration + 1, Value = 0 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime + duration + 2, Value = 0 });

            //Act
            var result = AchievementCalculationHelper.CalculateFlightDuration(startTime, endTime, SystemUnderTest);

            //Assert
            Assert.That(() => result, Is.EqualTo(duration));
        }
        
        [Test]
        public void Test_if_CalculateFlightDuration_with_only_motor_out_values()
        {
            //Arrange 
            const int startTime = 0;
            const int endTime = 100000;
            const int duration = 1000;

            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime, Value = 0 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime + duration, Value = 0 });

            //Act
            var result = AchievementCalculationHelper.CalculateFlightDuration(startTime, endTime, SystemUnderTest);

            //Assert
            Assert.That(() => result, Is.EqualTo(0));
        }

        [Test]
        public void Test_if_CalculateFlightDuration_ignores_values_out_of_time_range()
        {
            //Arrange 
            const int startTime = 100;
            const int endTime = 100000;
            const int duration = 1000;

            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime, Value = 1 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime -1, Value = 1 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime + duration, Value = 0 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = endTime + 1 , Value = 0 });

            //Act
            var result = AchievementCalculationHelper.CalculateFlightDuration(startTime, endTime, SystemUnderTest);

            //Assert
            Assert.That(() => result, Is.EqualTo(duration));
        }

        #endregion

        #region GetFlightDurationTimes

        [Test]
        public void Test_if_GetFlightDurationTimes_calculates_the_flight_duration()
        {
            //Arrange 
            const int startTime1 = 0;
            const int startTime2 = 100001;
            const int endTime1 = 100000;
            const int endTime2 = 200000;
            const int duration = 1000;
            var flights = new[] { new Tuple<long, long>(startTime1, endTime1), new Tuple<long, long>(startTime2, endTime2) };

            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime1, Value = 1 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime1 + duration, Value = 0 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime2, Value = 1 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime2 + duration, Value = 0 });

            //Act
            var result = AchievementCalculationHelper.GetFlightDurationTimes(flights, SystemUnderTest).ToList();

            //Assert
            Assert.That(() => result.Count, Is.EqualTo(2));
            Assert.That(() => result.First(), Is.EqualTo(duration));
            Assert.That(() => result.Last(), Is.EqualTo(duration));
        }

        [Test]
        public void Test_if_returns_nothing_when_null_is_passed()
        {
            //Act
            var result = AchievementCalculationHelper.GetFlightDurationTimes(new Tuple<long,long>[] {}, SystemUnderTest);

            //Assert
            Assert.That(() => result.Any(), Is.False);
        }
        #endregion
    }
}
