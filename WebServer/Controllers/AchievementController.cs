using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebServer.DataContext;
using WebServer.Models;

namespace WebServer.Controllers
{
    /// <summary>
    /// Provides the RESTful API regarding the achievements.
    /// </summary>
    [Authorize]
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
        public IQueryable<KeyValuePair<string,int>> GetRankingList()
        {
            using (var db = new IdentityDbContext())
            {

                var sortedUser = _achievementDb.GetAllUser().OrderByDescending(x => x.RankingPoints);
                var returnList = new Dictionary<string, int>();
                foreach (var user in sortedUser)
                {
                    var userName = db.Users.First(u => u.Id.Equals(user.ReleatedApplicationUserId)).UserName;
                    returnList.Add(userName, user.RankingPoints);
                }
                return returnList.AsQueryable();
            }
        }

        /// <summary>
        /// Returns the user associated with the token used.
        /// </summary>
        /// <returns></returns>
        private SmartPlaneUser _getCurrentUser()
        {
            var callingUser = RequestContext.Principal.Identity.GetUserId();
            var currentUser = _achievementDb.GetSmartPlaneUserByApplicationUserId(callingUser);
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