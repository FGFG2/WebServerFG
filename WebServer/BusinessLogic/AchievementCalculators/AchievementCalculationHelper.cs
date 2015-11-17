using System;
using System.Collections.Generic;
using System.Linq;
using WebServer.Models;

namespace WebServer.BusinessLogic.AchievementCalculators
{
    public static class AchievementCalculationHelper
    {
        /// <summary>
        /// Returns a lis of tuples which indicates a start time and a end time of one session
        /// </summary>
        /// <param name="targetUser"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<long, long>> GetEndAndStartTimesOfAllConnections(SmartPlaneUser targetUser)
        {
            var startTimes = targetUser.ConnectedDatas.Where(c => c.Value).Select(c => c.TimeStamp);
            var endTimes = targetUser.ConnectedDatas.Where(c => c.Value == false).Select(c => c.TimeStamp).ToList();
            foreach (var startTime in startTimes)
            {
                if (endTimes.Count == 0)
                {
                    yield break;
                }
                var endTime = endTimes.Where(e => e >= startTime).Min();
                endTimes.Remove(endTime);
                yield return new Tuple<long, long>(startTime, endTime);
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
            if (motorDatasInTimeRange.Count < 2)
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

            // get all motor values which are 0
            var ends = motorDatasInTimeRange.Where(m => m.Value == 0).ToList();
            if (ends.Any() == false)
            {
                // if no motor data is 0, the plane has not stoped flying
                return 0;
            }

            var startOfTheFlight = starts.Min(m => m.TimeStamp);
            var endOfTheFlight = ends.Min(m => m.TimeStamp);

            if (startOfTheFlight > endOfTheFlight)
            {
                return 0;
            }
            return endOfTheFlight - startOfTheFlight;
        }
    }
}
