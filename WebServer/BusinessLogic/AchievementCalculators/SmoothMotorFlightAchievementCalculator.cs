using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServer.Logging;
using WebServer.Models;

namespace WebServer.BusinessLogic.AchievementCalculators
{
    public class SmoothMotorFlightAchievementCalculator : AchievementCalculator
    {
        public const string AchievementName = "Ruhiger Flug (Motor)";
        public const int OnePercentStep =6000;        


        public SmoothMotorFlightAchievementCalculator(ILoggerFacade logger) : base(AchievementName, logger)
        {
        }

        protected override int CalculateProgress(SmartPlaneUser targetUser)
        {
            var smoothMotorFlightDurations = AchievementCalculationHelper.GetDurationOfFlightsWithSmoothMotor(targetUser).ToList();
            if (smoothMotorFlightDurations.Any() == false)
            {
                return 0;
            }
            var progress = smoothMotorFlightDurations.Sum() / OnePercentStep;
            if (progress >= 100)
            {
                return 100;
            }
            return (int)progress;
        }

        protected override Achievement CreateAchievement()
        {
            return new Achievement
            {
                Name = AchievementName,
                Description = "Fliege 10 Minuten ohne besonders schnelle Geschwindigkeitsänderungen",
                Progress = 0,
                ImageUrl = ""
            };
        }
    }
}