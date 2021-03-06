﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServer.Logging;
using WebServer.Models;

namespace WebServer.BusinessLogic.AchievementCalculators
{
    public class SmoothRudderFlightAchievementCalculator : AchievementCalculator
    {
        public const string AchievementName = "Ruhiger Flug (Ruder)";
        public const int OnePercentStep =6000;        


        public SmoothRudderFlightAchievementCalculator(ILoggerFacade logger) : base(AchievementName, logger)
        {
        }

        protected override int CalculateProgress(SmartPlaneUser targetUser)
        {
            var smoothRudderFlightDurations = AchievementCalculationHelper.GetDurationOfFlightsWithSmoothRudder(targetUser).ToList();
            if (smoothRudderFlightDurations.Any() == false)
            {
                return 0;
            }
            var progress = smoothRudderFlightDurations.Sum() / OnePercentStep;
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
                Description = "Fliege 10 Minuten keine besonders scharfen kurven mit deinem FLugzeug",
                Progress = 0,
                ImageUrl = ""
            };
        }
    }
}