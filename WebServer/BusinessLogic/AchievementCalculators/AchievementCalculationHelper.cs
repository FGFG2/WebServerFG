using System;
using System.Collections.Generic;
using System.Linq;
using WebServer.Models;

namespace WebServer.BusinessLogic.AchievementCalculators
{
    /// <summary>
    /// Utility class for achievement calculator specific operations.
    /// </summary>
    public static class AchievementCalculationHelper
    {
        private const int SmoothDataTolerance = 30;

        public static IEnumerable<long> GetDurationOfFlightsWithSmoothRudder(SmartPlaneUser targetUser)
        {
            var rudderDatasInRange = GetAllRudderDatasWithinConnections(targetUser).ToList();
            if (rudderDatasInRange.Any() == false || rudderDatasInRange.First().Any() == false)
            {
                yield break;
            }

            var lastValue = rudderDatasInRange.First().First().Value;
            foreach (var connections in rudderDatasInRange)
            {
                var rudderDatas = connections as IList<RudderData> ?? connections.ToList();
                var timeStampOfCurrentValue = rudderDatas.First().TimeStamp;

                foreach (var rudderData in rudderDatas)
                {
                    //save current length of smooth flying and reset start and end times to start new calculation
                    var timeStampOfPreviousValue = timeStampOfCurrentValue;
                    timeStampOfCurrentValue = rudderData.TimeStamp;

                    if (_isSmoothDifference(rudderData.Value, lastValue))
                    {
                        var durationBetweenMotorValues = timeStampOfCurrentValue - timeStampOfPreviousValue;

                        if (durationBetweenMotorValues > 0)
                        {
                            yield return durationBetweenMotorValues;
                        }
                    }

                    lastValue = rudderData.Value;
                }
            }

        }

        private static bool _isSmoothDifference(int value, int lastValue)
        {
            var difference = Math.Abs(value - lastValue);
            return difference < SmoothDataTolerance;
        }

        public static IEnumerable<long> GetDurationOfFlightsWithSmoothMotor(SmartPlaneUser targetUser)
        {
            var motorDatasInRange = GetAllMotorDatasWithinConnections(targetUser).ToList();
            if (motorDatasInRange.Any() == false || motorDatasInRange.First().Any() == false)
            {
                yield break;
            }
            
            var lastValue = motorDatasInRange.First().First().Value;
            foreach (var connection in motorDatasInRange)
            {
                var motorDatas = connection as IList<MotorData> ?? connection.ToList();
                var timeStampOfCurrentValue = motorDatas.First().TimeStamp;

                foreach (var motorData in motorDatas)
                {
                    //save current length of smooth flying and reset start and end times to start new calculation
                    var timeStampOfPreviousValue = timeStampOfCurrentValue;
                    timeStampOfCurrentValue = motorData.TimeStamp;

                    if (_isSmoothDifference(motorData.Value, lastValue))
                    {
                        var durationBetweenMotorValues = timeStampOfCurrentValue - timeStampOfPreviousValue;

                        if (durationBetweenMotorValues > 0)
                        {
                            yield return durationBetweenMotorValues;
                        }
                    }

                    lastValue = motorData.Value;
                }
            }
        }

        /// <summary>
        /// Returns the Durations of Times without same Motor values
        /// </summary>
        /// <param name="targetUser"></param>
        /// <returns></returns>
        public static IEnumerable<long> GetDurationsOfRestlessFlyingTimes(SmartPlaneUser targetUser)
        {
            var motorDatasInRange = GetAllMotorDatasWithinConnections(targetUser).ToList();
            if (motorDatasInRange.Any() == false || motorDatasInRange.First().Any() == false)
            {
                yield break;
            }

            foreach (var motorDatas in motorDatasInRange)
            {
                long? startTime = 0;
                long endtime = 0;
                long? duration;
                var lastValue = 999; //definitely different from first value encountered

                foreach (var motorData in motorDatas)
                {
                    if (motorData.Value != lastValue)
                    {
                        lastValue = motorData.Value;
                        endtime = motorData.TimeStamp;
                    }
                    else
                    {
                        duration = endtime - startTime;
                        if (duration > 0)
                        {
                            yield return (long)duration;
                        }                        
                        startTime = motorData.TimeStamp;
                    }
                }
                duration = endtime - startTime;
                if (duration > 0)
                {
                    yield return (long) duration;
                }
            }
        } 

        /// <summary>
        /// Returns the length of the times a plane was flying with maximum motor value
        /// </summary>
        /// <param name="targetUser"></param>
        /// <returns></returns>
        public static IEnumerable<long> GetDurationsWithMaxMotor(SmartPlaneUser targetUser)
        {
            const int motorMax = 253; // Tolerance: 100% can be 253 or 255
            var motorDatasInRange = GetAllMotorDatasWithinConnections(targetUser);
            foreach (var motorDatas in motorDatasInRange)
            {
                long? startTime = null;
                long endTime = 0;
                long? duration;
                foreach (var motorData in motorDatas)
                {
                    endTime = motorData.TimeStamp;
                    if (motorData.Value < motorMax)
                    {
                        duration = endTime - startTime;
                        if (duration > 0)
                        {
                            yield return (long)duration;
                        }
                        startTime = null;
                        endTime = 0;
                    }
                    else
                    {
                        if (startTime == null)
                        {
                            startTime = motorData.TimeStamp;
                        }
                    }
                }
                duration = endTime - startTime;
                if (duration > 0)
                {
                    yield return (long) duration;
                }
            }
        }

        /// <summary>
        /// Returns all MotorData for each passed flight
        /// </summary>
        /// <param name="targetUser"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<MotorData>> GetAllMotorDatasWithinConnections(SmartPlaneUser targetUser)
        {
            var flights = GetEndAndStartTimesOfAllConnections(targetUser);
            return flights.Select(flight => targetUser.MotorDatas.Where(x => x.TimeStamp >= flight.Item1 && x.TimeStamp <= flight.Item2));
        }

        /// <summary>
        /// Returns all RudderData for each passed flight
        /// </summary>
        /// <param name="targetUser"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<RudderData>> GetAllRudderDatasWithinConnections(SmartPlaneUser targetUser)
        {
            var flights = GetEndAndStartTimesOfAllConnections(targetUser);
            return flights.Select(flight => targetUser.RudderDatas.Where(x => x.TimeStamp >= flight.Item1 && x.TimeStamp <= flight.Item2));
        }

        /// <summary>
        /// Returns a list of tuples which indicates a start time and a end time of one session
        /// </summary>
        /// <param name="targetUser"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<long, long>> GetEndAndStartTimesOfAllConnections(SmartPlaneUser targetUser)
        {
            var startTimes = targetUser.ConnectedDatas.Where(c => c.Value).Select(c => c.TimeStamp).ToList();
            var endTimes = targetUser.ConnectedDatas.Where(c => c.Value == false).Select(c => c.TimeStamp).ToList();
            foreach (var startTime in startTimes)
            {
                var endTimesAfterStartTime = endTimes.Where(e => e >= startTime).ToList();

                if (endTimesAfterStartTime.Any())
                {
                    var endTime = endTimesAfterStartTime.Min();
                    endTimes.Remove(endTime);
                    yield return new Tuple<long, long>(startTime, endTime);
                    continue;
                }
                //If there is no en connection for the start connection and the start connection is the last start connection, the use the last 
                //Motor data time stamp as end time because the plane is actually flying
                if (startTimes.Any(x => x > startTime))
                {
                    continue;
                }
                var motorDatas = targetUser.MotorDatas.Where(x => x.Value != 0).Where(x => x.TimeStamp > startTime).ToList();
                if (motorDatas.Any() == false)
                {
                    continue;
                }
                var lastMotorData = motorDatas.Max(x => x.TimeStamp);
                yield return new Tuple<long, long>(startTime, lastMotorData);
            }
        }

        /// <summary>
        /// Calculates the time a plane is flying in the passed flight times
        /// </summary>
        /// <param name="flights">The start and end times of possible flights</param>
        /// <param name="targetUser"></param>
        /// <returns></returns>
        public static IEnumerable<long> GetFlightDurationTimes(IEnumerable<Tuple<long, long>> flights, SmartPlaneUser targetUser)
        {
            return flights.Select(flight => CalculateFlightDuration(flight.Item1, flight.Item2, targetUser));
        }

        /// <summary>
        /// Searches for the first and the last motor use and sums up the time between.
        /// The first motor use is when the motor value is not 0
        /// The last motor use is when the motor value is 0
        /// </summary>
        /// <param name="startTime">Uses the startTime as the start of the range.</param>
        /// <param name="endTime">Uses the startTime as the end of the range</param>
        /// <param name="targetUser"></param>
        /// <returns></returns>
        public static long CalculateFlightDuration(long startTime, long endTime, SmartPlaneUser targetUser)
        {
            var motorDatasInTimeRange = targetUser.MotorDatas.Where(m => m.TimeStamp <= endTime && m.TimeStamp >= startTime).ToList();

            //When there are no MotorData in Range return 0
            if (motorDatasInTimeRange.Any() == false)
            {
                return 0;
            }

            // get all motor values which are not 0
            var starts = motorDatasInTimeRange.Where(m => m.Value != 0).ToList();
            if (starts.Any() == false)
            {
                // if no motor data is not 0, the plane was not flying
                return 0;
            }
            var startOfTheFlight = starts.Min(m => m.TimeStamp);

            // get all motor values which are 0
            var ends = motorDatasInTimeRange.Where(m => m.Value == 0).ToList();

            // if no motor data is 0, use the time when the connection was closed
            var endOfTheFlight = ends.Any() ? ends.Min(m => m.TimeStamp) : endTime;

            //To be sure that no negative flight duration will be returned
            if (startOfTheFlight > endOfTheFlight)
            {
                return 0;
            }
            return endOfTheFlight - startOfTheFlight;
        }
    }
}
