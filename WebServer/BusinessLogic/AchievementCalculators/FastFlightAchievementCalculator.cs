using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServer.Logging;
using WebServer.Models;

namespace WebServer.BusinessLogic.AchievementCalculators
{
    public class FastFlightAchievementCalculator : AchievementCalculator
    {

        public const string AchievementName = "Geschwindigkeit";
        public const float OnePercentStep = 0.1f;
        public const int NeededDurationWithMaxMotor = 5000;

        public FastFlightAchievementCalculator(ILoggerFacade logger) : base(AchievementName, logger)
        {
        }

        protected override int CalculateProgress(SmartPlaneUser targetUser)
        {
            var timesWithMaxMotor = AchievementCalculationHelper.GetDurationsWithMaxMotor(targetUser).ToList();
            var maxMotorTimeDuration = timesWithMaxMotor.Where(d => d >= NeededDurationWithMaxMotor);
            var progress = maxMotorTimeDuration.Count() / OnePercentStep;
            if (progress > 100)
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
                Description = "Fliege 10 mal für mindestens 5 sekunden mit voller Motordrehzahl",
                Progress = 0,
                ImageUrl = GetPathToImage()
            };
        }
    }
}