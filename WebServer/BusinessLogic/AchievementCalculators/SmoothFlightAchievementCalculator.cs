﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServer.Models;

namespace WebServer.BusinessLogic.AchievementCalculators
{
    public class SmoothFlightAchievementCalculator : AchievementCalculator
    {
        public const string AchievementName = "Ruhiger Flug 1";
        public const int OnePercentStep =6000;        


        public SmoothFlightAchievementCalculator() : base(AchievementName)
        {
        }

        protected override int CalculateProgress(SmartPlaneUser targetUser)
        {
            var smoothFlightDurations = AchievementCalculationHelper.GetDurationOfFlightsWithSmoothRudder(targetUser);            
            var progress = smoothFlightDurations.Sum() / OnePercentStep;
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