using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServer.Logging;
using WebServer.Models;

namespace WebServer.BusinessLogic.AchievementCalculators
{
    public class RestlessFlightAchievementCalculator : AchievementCalculator
    {
        public const string AchievementName = "Unruhiger Flug";
        public const int OnePercentStep = 3000;

        public RestlessFlightAchievementCalculator(ILoggerFacade logger) : base(AchievementName, logger)
        {            
        }

        protected override int CalculateProgress(SmartPlaneUser targetUser)
        {
            var restlessFlightDurations = AchievementCalculationHelper.GetDurationsOffRestlessFlyingTimes(targetUser);
            int progress = 0;
            long durationsSum = 0;
            foreach (var restlessFlightDuration in restlessFlightDurations)
            {
                if (restlessFlightDuration >= OnePercentStep)
                {
                    durationsSum += restlessFlightDuration;
                }
            }
            progress = (int)durationsSum/OnePercentStep;
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
                Description = "Fliege 10 mal für mindestens 30sek mit ständig wechselnden Motorwerten.",
                Progress = 0,
                ImageUrl = ""
            };
        }
    }
}