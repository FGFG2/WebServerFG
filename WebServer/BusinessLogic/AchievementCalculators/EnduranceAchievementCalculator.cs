using System.Linq;
using WebServer.Logging;
using WebServer.Models;

namespace WebServer.BusinessLogic.AchievementCalculators
{
    public class EnduranceAchievementCalculator : AchievementCalculator
    {
        public const string AchievementName = "Ausdauer";
        public const int OnePercentStep = 600;

        public EnduranceAchievementCalculator(ILoggerFacade logger) : base(AchievementName, logger)
        {
        }

        protected override int CalculateProgress(SmartPlaneUser targetUser)
        {
            var flights =  AchievementCalculationHelper.GetEndAndStartTimesOfAllConnections(targetUser);
            var flightDurations = AchievementCalculationHelper.GetFlightDurationTimes(flights, targetUser).ToList();
            if (flightDurations.Any() == false)
            {
                return 0;
            }

            var longestFlight = flightDurations.Max();
            var percent = longestFlight/OnePercentStep;
            if (percent >= 100)
            {
                return 100;
            }
            return (int) percent;
        }

        protected override Achievement CreateAchievement()
        {
            return new Achievement
            {
                Name = AchievementName,
                Description = "Fliege 1 Minute am Stück mit deinem Flugzeug!",
                Progress = 0,
                ImageUrl = ""
            };
        }
    }
}