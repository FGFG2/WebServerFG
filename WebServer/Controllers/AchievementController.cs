using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebServer.Models;

namespace WebServer.Controllers
{
    /// <summary>
    /// Provides the RESTful API regarding the achievements.
    /// </summary>
    public class AchievementController : ApiController
    {
        #region Mocked data
        private readonly IEnumerable<Achievement> _mockedAchievements = new[]
        {
            new Achievement {Id = 0, Name = "Amateur", Description = "Finished 10 flights.", Progress = 70},
            new Achievement {Id = 1, Name = "Lucky pilot", Description = "", Progress = 100},
            new Achievement {Id = 2, Name = "Stunt pilot", Description = "", Progress = 0},
            new Achievement
            {
                Id = 3,
                Name = "Racing pilot",
                Description = "Maximum power for 10 seconds.",
                Progress = 20
            },
            new Achievement
            {
                Id = 4,
                Name = "Kamikaze",
                Description = "Managed to be in a power dive for 5 seconds.",
                ImageUrl =
                    @"https://de.wikipedia.org/wiki/Sturzflug#/media/File:F14bomb.jpg",
                Progress = 100
            }
        };
        #endregion
        
        // GET: api/AllAchievements
        [Route("api/AllAchievements")]
        public IEnumerable<Achievement> GetAllAchievements()
        {
            return _mockedAchievements.AsQueryable();
        }

        // GET: api/ObtainedAchievements
        [Route("api/ObtainedAchievements")]
        public IEnumerable<Achievement> GetObtainedAchievements()
        {
            var obtained =
                from achievement in _mockedAchievements
                where achievement.Progress == 100
                select achievement;

            return obtained.AsQueryable();
        }
    }
}