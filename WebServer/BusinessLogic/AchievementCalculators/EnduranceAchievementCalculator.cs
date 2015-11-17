using System;
using System.Collections.Generic;
using System.Linq;
using WebServer.Models;

namespace WebServer.BusinessLogic.AchievementCalculators
{
    public class EnduranceAchievementCalculator : AchievementCalculator
    {
        public const string AchievementName = "Ausdauer";
        public const int OnePercentStep = 600;

        public EnduranceAchievementCalculator() : base(AchievementName)
        {
        }

        protected override int CalculateProgress(SmartPlaneUser targetUser)
        {
            var flights = _getEndTimesAndStartTimesOfAllConnections(targetUser);
            var flightTimes = _getFlightTimes(flights, targetUser).ToList();
            if (flightTimes.Any() == false)
            {
                return 0;
            }
            var longestFlight = flightTimes.Max();
            var percent = longestFlight/OnePercentStep;
            if (percent >= 100)
            {
                return 100;
            }
            return (int) percent;
        }

        /// <summary>
        /// Returns a lis of tuples which indicates a start time and a end time of one session
        /// </summary>
        /// <param name="targetUser"></param>
        /// <returns></returns>
        private IEnumerable<Tuple<long, long>> _getEndTimesAndStartTimesOfAllConnections(SmartPlaneUser targetUser)
        {
            var startTimes = targetUser.ConnectedDatas.Where(c => c.Value).Select(c => c.TimeStamp);
            var endTimes = targetUser.ConnectedDatas.Where(c => c.Value == false).Select(c => c.TimeStamp).ToList();
            if(endTimes.Count == 0)
            {
                yield break;
            }
            foreach (var startTime in startTimes)
            {
                var endTime = endTimes.Where(e => e >= startTime).Min();
                yield return new Tuple<long, long>(startTime, endTime);
            }
        }

        /// <summary>
        /// Calculates the time a plane is flying in the passed flight times
        /// </summary>
        /// <param name="flights">The start and end times of possible flights</param>
        /// <param name="targetUser"></param>
        /// <returns></returns>
        private IEnumerable<long> _getFlightTimes(IEnumerable<Tuple<long, long>> flights, SmartPlaneUser targetUser)
        {
            return flights.Select(flight => _calculateFlightTime(flight.Item1, flight.Item2, targetUser));
        }

        /// <summary>
        /// Searches for the first and the last motor use and sums up the time between
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="targetUser"></param>
        /// <returns></returns>
        private long _calculateFlightTime(long startTime, long endTime, SmartPlaneUser targetUser)
        {
            var motorDatasInTimeRange = targetUser.MotorDatas.Where(m => m.TimeStamp <= endTime && m.TimeStamp >= startTime).ToList();
            if (motorDatasInTimeRange.Count < 2)
            {
                return 0;
            }
            var startOfTheFlight = motorDatasInTimeRange.Where(m=>m.Value!=0).Min(m => m.TimeStamp);
            var endOfTheFlight = motorDatasInTimeRange.Max(m => m.TimeStamp);
            
            if (startOfTheFlight > endOfTheFlight)
            {
                return 0;
            }
            return endOfTheFlight - startOfTheFlight;
        }

        protected override Achievement CreateAchievement()
        {
            return new Achievement
            {
                Name = AchievementName,
                Description = "Fliege 10 Minuten am Stück mit deinem Flugzeug!",
                Progress = 0,
                ImageUrl = ""
            };
        }
    }
}