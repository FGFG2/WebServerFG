using System;
using System.Collections.Generic;
using System.Linq;
using WebGrease.Css.Extensions;
using WebServer.Models;

namespace WebServer.BusinessLogic.AchievementCalculators
{
    public static class AchievementCalculationHelper
    {
        public static IEnumerable<long> GetDurationOfFlightsWithSmoothRudder(SmartPlaneUser targetUser)
        {
            var allConnections = GetEndAndStartTimesOfAllConnections(targetUser);
            var motorDatasInRange = GetAllMotorDatasWithinConnections(allConnections, targetUser);
            long startTime = motorDatasInRange.First().First().TimeStamp;
            long endTime = motorDatasInRange.First().First().TimeStamp;
            var lastValue = motorDatasInRange.First().First().Value;
            long duration = 0;
            foreach (var connection in motorDatasInRange)
            {                
                foreach (var motorData in connection)
                {
                    if (motorData.Value - lastValue > 30 || motorData.Value - lastValue < -30)
                    {                        
                        duration = endTime - startTime;
                        startTime = motorData.TimeStamp;
                        endTime = motorData.TimeStamp;
                        if (duration > 0)
                        {
                            yield return (long) duration;
                        }
                    }
                    else
                    {
                        endTime = motorData.TimeStamp;
                    }
                    lastValue = motorData.Value;
                }
                duration = endTime - startTime;
                if (duration > 0)
                {
                    yield return (long) duration;
                }
            }
        } 

        /// <summary>
        /// Returns the length of the times a plane was flying with maxminum motor value
        /// </summary>
        /// <param name="targetUser"></param>
        /// <returns></returns>
        public static IEnumerable<long> GetDurationsWithMaxMotor(SmartPlaneUser targetUser)
        {
            const int motorMax = 253; // Tolerance: 100% can be 253 or 255
            var allConnections = GetEndAndStartTimesOfAllConnections(targetUser);
            var motorDatasInRange = GetAllMotorDatasWithinConnections(allConnections, targetUser);
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
        /// <param name="flights"></param>
        /// <param name="targetUser"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<MotorData>> GetAllMotorDatasWithinConnections(IEnumerable<Tuple<long, long>> flights, SmartPlaneUser targetUser)
        {
            return flights.Select(flight => targetUser.MotorDatas.Where(x => x.TimeStamp >= flight.Item1 && x.TimeStamp <= flight.Item2));
        }

        /// <summary>
        /// Returns all RudderData for each passed flight
        /// </summary>
        /// <param name="flights"></param>
        /// <param name="targetUser"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<RudderData>> GetAllRudderDatasWithinConnections(IEnumerable<Tuple<long, long>> flights, SmartPlaneUser targetUser)
        {
            return flights.Select(flight => targetUser.RudderDatas.Where(x => x.TimeStamp >= flight.Item1 && x.TimeStamp <= flight.Item2));
        }

        /// <summary>
        /// Returns a lis of tuples which indicates a start time and a end time of one session
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
                //If there is no en connection for the startconnection and the start connection is the last start connection, the use the last 
                //Motor data time stamp as endtime because the plane is actually flying
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
