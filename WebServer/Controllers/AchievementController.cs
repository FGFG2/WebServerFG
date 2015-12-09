﻿using System.Linq;
using System.Threading;
using System.Web.Http;
using WebServer.BusinessLogic;
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
            var currentUser = _getCurrentUser();
            return currentUser.Achievements.AsQueryable();
        }

        // GET: api/ObtainedAchievements
        [Route("api/ObtainedAchievements")]
        public IQueryable<Achievement> GetObtainedAchievements()
        {
            var currentUser = _getCurrentUser();
            return currentUser.Achievements.Where(a => a.Progress == 100).AsQueryable();
        }

        // GET: api/RankingList
        [Route("api/RankingList")]
        public IQueryable<SmartPlaneUser> GetRankingList()
        {
            return _achievementDb.GetAllUser().OrderByDescending(x => x.RankingPoints).AsQueryable();
        }

        private SmartPlaneUser _getCurrentUser()
        {
            var currentUser = _achievementDb.GetSmartPlaneUserById(0);
            return currentUser;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            _achievementDb.Dispose();
        }
    }
}