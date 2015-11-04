using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebServer.DataContext;
using WebServer.Models;

namespace WebServer.Controllers
{
    /// <summary>
    /// Provides the RESTful API regarding the achievements.
    /// </summary>
    public class AchievementController : ApiController
    {
        private readonly IAchievementDb _achievementDb;

        public AchievementController(IAchievementDb achievementDb)
        {
            _achievementDb = achievementDb;
        }

        // GET: api/AllAchievements
        [Route("api/AllAchievements")]
        public IQueryable<Achievement> GetAllAchievements()
        {
            return _achievementDb.GetSmartPlaneUserById(0).Achievements.AsQueryable();
        }

        // GET: api/ObtainedAchievements
        [Route("api/ObtainedAchievements")]
        public IQueryable<Achievement> GetObtainedAchievements()
        {
            return _achievementDb.GetSmartPlaneUserById(0).Achievements.Where(a => a.Progress == 100).AsQueryable();
        }
    }
}