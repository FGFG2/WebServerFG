using System;
using System.Collections.Generic;
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
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = startTime, Value = true });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = endTime, Value = false });

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
                SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = startTimes[i], Value = true });
                SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = endTimes[i], Value = false });
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
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, Value = true });

            //Act
            var result = AchievementCalculationHelper.GetEndAndStartTimesOfAllConnections(SystemUnderTest);

            //Assert
            Assert.That(() => result.Any(), Is.False);
        }

        [Test]
        public void Test_if_GetEndAndStartTimesOfAllConnections_returns_no_values_with_no_start_Connection()
        {
            //Arrange
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = 1000, Value = false });

            //Act
            var result = AchievementCalculationHelper.GetEndAndStartTimesOfAllConnections(SystemUnderTest);

            //Assert
            Assert.That(() => result.Any(), Is.False);
        }
        [Test]
        public void Test_if_GetEndAndStartTimesOfAllConnections_returns_no_values_with_endConnection_before_start_Connection()
        {
            //Arrange
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = 1000, Value = false });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = 1001, Value = true });

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
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = startTime, Value = true });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = startTime + 1, Value = true });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = endTime, Value = false });

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
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = startTime, Value = true });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = endTime, Value = false });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = endTime + 1, Value = false });

            //Act
            var result = AchievementCalculationHelper.GetEndAndStartTimesOfAllConnections(SystemUnderTest);

            //Assert
            Assert.That(() => result.Count(), Is.EqualTo(1));
            Assert.That(() => result.First().Item1, Is.EqualTo(startTime));
            Assert.That(() => result.First().Item2, Is.EqualTo(endTime));
        }

        [Test]
        public void Test_if_GetEndAndStartTimesOfAllConnections_returns_the_timestamp_of_the_last_MotorData_when_no_disconnect_is_available()
        {
            //Arrange
            const int startTime = 1;
            const int endTime = 10;
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, Value = false });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = startTime, Value = true });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = endTime, Value = 1 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = endTime + 100, Value = 0 });

            //Act
            var result = AchievementCalculationHelper.GetEndAndStartTimesOfAllConnections(SystemUnderTest);

            //Assert
            Assert.That(() => result.First().Item1, Is.EqualTo(startTime));
            Assert.That(() => result.First().Item2, Is.EqualTo(endTime));
        }

        [Test]
        public void Test_if_GetEndAndStartTimesOfAllConnections_ignores_old_start_Connections_with_no_end_Connection()
        {
            //Arrange
            const int startTime = 100;
            const int endTime = 110;
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, Value = true }); // wrong Connection. Should be ignored
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = startTime, Value = true });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = endTime, Value = 1 });

            //Act
            var result = AchievementCalculationHelper.GetEndAndStartTimesOfAllConnections(SystemUnderTest);

            //Assert
            Assert.That(() => result.Count(), Is.EqualTo(1));
            Assert.That(() => result.First().Item1, Is.EqualTo(startTime));
            Assert.That(() => result.First().Item2, Is.EqualTo(endTime));
        }

        [Test]
        public void Test_if_GetEndAndStartTimesOfAllConnections_ignores_uses_end_Connection_if_existing()
        {
            //Arrange
            const int startTime = 0;
            const int endTime = 100;
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = startTime, Value = true });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime + 1, Value = 1 });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = endTime, Value = false });

            //Act
            var result = AchievementCalculationHelper.GetEndAndStartTimesOfAllConnections(SystemUnderTest).ToList();

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
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime - 1, Value = 1 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime + duration, Value = 0 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = endTime + 1, Value = 0 });

            //Act
            var result = AchievementCalculationHelper.CalculateFlightDuration(startTime, endTime, SystemUnderTest);

            //Assert
            Assert.That(() => result, Is.EqualTo(duration));
        }

        [Test]
        public void Test_if_CalculateFlightDuration_uses_end_time_when_no_0_MotorData_can_be_fond()
        {
            //Arrange 
            const int startTime = 0;
            const int duration = 1000;
            const int endTime = duration;

            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime, Value = 1 });

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
            var result = AchievementCalculationHelper.GetFlightDurationTimes(new Tuple<long, long>[] { }, SystemUnderTest);

            //Assert
            Assert.That(() => result.Any(), Is.False);
        }
        #endregion

        #region GetAllMotorDatasWithinConnections

        [Test]
        public void Test_if_GetAllMotorDatasWithinConnections_retrns_only_MotorData_within_the_connection()
        {
            //Arrange 
            const int startTime = 0;
            const int endTime = 100000;

            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = startTime, Value = true });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = endTime, Value = false });

            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime, Value = 1 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime + 1, Value = 0 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime + 2, Value = 1 });

            //Act
            var allMotorDatasWithinConnections = AchievementCalculationHelper.GetAllMotorDatasWithinConnections(SystemUnderTest).ToList();

            //Assert
            Assert.That(() => allMotorDatasWithinConnections.Count, Is.EqualTo(1));
            Assert.That(() => allMotorDatasWithinConnections.First(), Is.EquivalentTo(SystemUnderTest.MotorDatas));
        }

        [Test]
        public void Test_if_GetAllMotorDatasWithinConnections_ignores_MotorData_out_of_the_connection()
        {
            //Arrange 
            const int startTime = 1000;
            const int endTime = 100000;
            
            var expected = new[]
            {
                new MotorData {TimeStamp = startTime, Value = 1},
                new MotorData {TimeStamp = startTime+1, Value = 0},
                new MotorData {TimeStamp = startTime+2, Value = 1}
            };

            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = startTime - 1, Value = 1 });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = startTime, Value = true });
            SystemUnderTest.MotorDatas.Add(expected[0]);
            SystemUnderTest.MotorDatas.Add(expected[1]);
            SystemUnderTest.MotorDatas.Add(expected[2]);
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = endTime, Value = false });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = endTime + 1, Value = 1 });

            //Act
            var allMotorDatasWithinConnections = AchievementCalculationHelper.GetAllMotorDatasWithinConnections(SystemUnderTest).ToList();

            //Assert
            Assert.That(() => allMotorDatasWithinConnections.Count, Is.EqualTo(1));
            Assert.That(() => allMotorDatasWithinConnections.First(), Is.EquivalentTo(expected));
        }

        #endregion

        #region GetDurationsWithMaxMotor

        [Test]
        public void Test_GetDurationsWithMaxMotor_returns_the_correct_duration_whith_one_max_motor_time()
        {
            //Arrange 
            const long duration = 100;

            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, Value = true });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = 0, Value = 255 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = duration, Value = 0 });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = duration, Value = false });


            //Act
            var result = AchievementCalculationHelper.GetDurationsWithMaxMotor(SystemUnderTest).ToList();

            //Assert
            Assert.That(() => result.Count, Is.EqualTo(1));
            Assert.That(() => result.First(), Is.EqualTo(duration));
        }

        [Test]
        public void Test_GetDurationsWithMaxMotor_returns_the_correct_duration_whith_two_max_motor_durations_and_lower_values_between()
        {
            //Arrange 
            const long duration1 = 100;
            const long start2 = 1000;
            const long duration2 = 200;
            const long endTime = 100000;

            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, Value = true });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = 0, Value = 255 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = duration1, Value = 0 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = start2, Value = 255 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = start2 + duration2, Value = 0 });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = endTime, Value = false });


            //Act
            var result = AchievementCalculationHelper.GetDurationsWithMaxMotor(SystemUnderTest).ToList();

            //Assert
            Assert.That(() => result.Count, Is.EqualTo(2));
            Assert.That(() => result.First(), Is.EqualTo(duration1));
            Assert.That(() => result.Last(), Is.EqualTo(duration2));
        }

        [Test]
        public void Test_GetDurationsWithMaxMotor_returns_the_correct_duration_whith_two_max_motor_durations_in_two_connections()
        {
            //Arrange 
            const long duration1 = 100;
            const long start2 = 1000;
            const long duration2 = 200;
            const long endTime = 100000;

            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, Value = true });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = 0, Value = 255 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = duration1, Value = 0 });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = duration1, Value = false });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = start2, Value = true });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = start2, Value = 255 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = start2 + duration2, Value = 0 });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = endTime, Value = false });


            //Act
            var result = AchievementCalculationHelper.GetDurationsWithMaxMotor(SystemUnderTest).ToList();

            //Assert
            Assert.That(() => result.Count, Is.EqualTo(2));
            Assert.That(() => result.First(), Is.EqualTo(duration1));
            Assert.That(() => result.Last(), Is.EqualTo(duration2));
        }

        [Test]
        public void Test_GetDurationsWithMaxMotor_returns_the_correct_duration_whith_many_max_motor_values()
        {
            //Arrange 
            const long duration1 = 100;
            const long endTime = 100000;

            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, Value = true });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = 0, Value = 255 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = 2, Value = 255 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = 4, Value = 255 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = 10, Value = 255 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = 20, Value = 255 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = duration1, Value = 0 });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = endTime, Value = false });

            //Act
            var result = AchievementCalculationHelper.GetDurationsWithMaxMotor(SystemUnderTest).ToList();

            //Assert
            Assert.That(() => result.Count, Is.EqualTo(1));
            Assert.That(() => result.First(), Is.EqualTo(duration1));
        }

        [Test]
        public void Test_GetDurationsWithMaxMotor_returns_the_correct_duration_whith_two_max_motor_values()
        {
            //Arrange 
            const long duration1 = 100;
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, Value = true });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = 0, Value = 255 });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = duration1, Value = 255 });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = duration1, Value = false });

            //Act
            var result = AchievementCalculationHelper.GetDurationsWithMaxMotor(SystemUnderTest).ToList();

            //Assert
            Assert.That(() => result.Count, Is.EqualTo(1));
            Assert.That(() => result.First(), Is.EqualTo(duration1));
        }

        [Test]
        public void Test_GetDurationsWithMaxMotor_returns_the_correct_duration_whith_one_max_motor_value_and_one_disconnect()
        {
            //Arrange 
            const long duration1 = 100;
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, Value = true });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = 0, Value = 255 });
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = duration1, Value = false });

            //Act
            var result = AchievementCalculationHelper.GetDurationsWithMaxMotor(SystemUnderTest).ToList();

            //Assert
            Assert.That(() => result.Any(), Is.False);
        }

        [Test]
        public void Test_GetDurationsWithMaxMotor_returns_the_correct_duration_whith_one_max_motor_value_and_no_disconnect()
        {
            //Arrange 
            SystemUnderTest.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, Value = true });
            SystemUnderTest.MotorDatas.Add(new MotorData { TimeStamp = 0, Value = 255 });

            //Act
            var result = AchievementCalculationHelper.GetDurationsWithMaxMotor(SystemUnderTest).ToList();

            //Assert
            Assert.That(() => result.Any(), Is.False);
        }

        #endregion
    }
}
