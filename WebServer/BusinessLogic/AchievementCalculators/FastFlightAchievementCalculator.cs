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
            var timesWithMaxMotor = AchievementCalculationHelper.GetTimesWithMaxMotor(targetUser);
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