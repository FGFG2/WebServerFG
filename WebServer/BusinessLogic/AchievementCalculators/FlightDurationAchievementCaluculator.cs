using System;
using System.Linq;
using WebServer.Logging;
using WebServer.Models;

namespace WebServer.BusinessLogic.AchievementCalculators
{
    public class FlightDurationAchievementCaluculator : AchievementCalculator
    {
        public const string AchievementName = "Flugmeilen";
        public const int OnePercentStep = 6000; // One minute in mili secounds

        public FlightDurationAchievementCaluculator(ILoggerFacade logger) : base(AchievementName, logger)
        {
        }

        protected override int CalculateProgress(SmartPlaneUser targetUser)
        {
            var flights = AchievementCalculationHelper.GetEndAndStartTimesOfAllConnections(targetUser);
            var flightDurations = AchievementCalculationHelper.GetFlightDurationTimes(flights, targetUser).ToList();

            var flightDuration = flightDurations.Sum();
            var percent = flightDuration / OnePercentStep;
            if (percent >= 100)
            {
                return 100;
            }
            return (int)percent;
        }

        protected override Achievement CreateAchievement()
        {
            return new Achievement
            {
                Name = AchievementName,
                Description = "Fliege insgesamt 10 Minuten mit deinem SmartPlane",
                Progress = 0,
                ImageUrl = GetPathToImage()
            };
        }
    }
}