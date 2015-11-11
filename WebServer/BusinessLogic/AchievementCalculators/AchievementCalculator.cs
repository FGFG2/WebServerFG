﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServer.Models;

namespace WebServer.BusinessLogic.AchievementCalculators
{
    public abstract class AchievementCalculator : IAchievementCalculator
    {
        private readonly string _achevementName;

        protected AchievementCalculator(string achevementName)
        {
            _achevementName = achevementName;
        }

        public void CalculateAchievementProgress(SmartPlaneUser targetUser)
        {
            var relatedAchievement = _addAchievementWhenMissing(targetUser);
            relatedAchievement.Progress = Convert.ToByte(CalculateProgress(targetUser));
        }

        private Achievement _addAchievementWhenMissing(SmartPlaneUser targetUser)
        {
            var achievement = targetUser.Achievements.FirstOrDefault(a => a.Name.Equals(_achevementName));
            if (achievement != null)
            {
                return achievement;
            }
            achievement = CreateAchievement();
            targetUser.Achievements.Add(achievement);
            return achievement;
        }

        /// <summary>
        /// Method which calculates the progress of this achievement with the passed SmartPlaneUser
        /// </summary>
        /// <param name="targetUser"></param>
        /// <returns></returns>
        protected abstract int CalculateProgress(SmartPlaneUser targetUser);

        /// <summary>
        /// Factory method which creates the Achievement releted to this calculator.
        /// </summary>
        /// <returns></returns>
        protected abstract Achievement CreateAchievement();
    }
}