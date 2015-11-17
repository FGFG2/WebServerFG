using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServer.Models;

namespace WebServer.BusinessLogic.AchievementCalculators
{
    public class FastFlightAchievementCalculator : AchievementCalculator
    {

        public const string AchievementName = "Geschwindigkeit"; 

        public FastFlightAchievementCalculator() : base(AchievementName)
        {
        }

        protected override int CalculateProgress(SmartPlaneUser targetUser)
        {
            var timesWithMaxMotor = _getTimesWithMaxMotor(targetUser);
            foreach (var time in timesWithMaxMotor)
            {
                if ((time.Item2 - time.Item1) < 5000)
                {
                    timesWithMaxMotor = timesWithMaxMotor.Where(t => !t.Equals(time)).ToList();
                }
            }            
            double progress = timesWithMaxMotor.Count()/10.0;
            if (progress > 1.0)
            {
                progress = (float) 1.0;
            }
            return (int) (progress*100);
        }

        private List<MotorData> _getAllMotorDatasWithinConnections(SmartPlaneUser targetUser)
        {
            var connectionTimes = _getEndTimesAndStartTimesOfAllConnections(targetUser);
            var motorDatasInRange = new List<MotorData>();
            foreach (var connectionTime in connectionTimes)
            {
                motorDatasInRange.AddRange(targetUser.MotorDatas.Where(m => m.TimeStamp >= connectionTime.Item1 && m.TimeStamp <= connectionTime.Item2));
            }
            return motorDatasInRange;
        }

        private IEnumerable<Tuple<long, long>> _getTimesWithMaxMotor(SmartPlaneUser targetUser)
        {
            var motorDatasInRange = _getAllMotorDatasWithinConnections(targetUser);
            var entriesWithMaxMotor = motorDatasInRange.Where(m => m.Value >= 255).Select(m => m).ToList();

            var startTimes = new List<long>();
            foreach (var entry in entriesWithMaxMotor)
            {
                var prevValue = targetUser.MotorDatas.IndexOf(entry)-1;
                if (prevValue < 0) continue;
                if (targetUser.MotorDatas[prevValue].Value < 255)
                {
                    startTimes.Add(entry.TimeStamp);
                }
            }

            var endTimes = new List<long>();
            foreach (var entry in entriesWithMaxMotor)
            {
                var nextEntry = targetUser.MotorDatas.IndexOf(entry) + 1;
                if (nextEntry >= targetUser.MotorDatas.Count()) continue;
                if (targetUser.MotorDatas[nextEntry].Value < 255)
                {
                    endTimes.Add(entry.TimeStamp);
                }
            }

            var startAndEndTimes = startTimes.Zip(endTimes, (s, e) => new { startTime = s, endTime = e });
            foreach (var time in startAndEndTimes)
            {
                yield return new Tuple<long, long>(time.startTime, time.endTime);
            }
        }

        /// <summary>
        /// Returns a lis of tuples which indicates a start time and a end time of one session
        /// </summary>
        /// <param name="targetUser"></param>
        /// <returns></returns>
        private IEnumerable<Tuple<long, long>> _getEndTimesAndStartTimesOfAllConnections(SmartPlaneUser targetUser)
        {
            var startTimes = targetUser.ConnectedDatas.Where(c => c.IsConnected).Select(c => c.TimeStamp);
            var endTimes = targetUser.ConnectedDatas.Where(c => c.IsConnected == false).Select(c => c.TimeStamp).ToList();
            if (endTimes.Count == 0)
            {
                yield break;
            }
            foreach (var startTime in startTimes)
            {
                var endTime = endTimes.Where(e => e >= startTime).Min();
                yield return new Tuple<long, long>(startTime, endTime);
            }
        }

        protected override Achievement CreateAchievement()
        {
            return new Achievement
            {
                Name = AchievementName,
                Description = "Fliege 10 mal für mindestens 5 sekunden mit voller Motordrehzahl",
                Progress = 0,
                ImageUrl = ""
            };
        }
    }
}