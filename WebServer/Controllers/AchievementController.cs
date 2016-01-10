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
    public class AchievementController : AuthorizedControllerBase
    {
        /// <summary>
        /// Creates a new instance of the AchievementController class.
        /// </summary>
        /// <param name="achievementDb">Database of the achievement system.</param>
        public AchievementController(IAchievementDb achievementDb) : base(achievementDb)
        {
        }

        // GET: api/AllAchievements
        [Route("api/AllAchievements")]
        public IQueryable<Achievement> GetAllAchievements()
        {
            var currentUser = GetCurrentUser();
            return currentUser.Achievements.AsQueryable();
        }

        // GET: api/ObtainedAchievements
        [Route("api/ObtainedAchievements")]
        public IQueryable<Achievement> GetObtainedAchievements()
        {
            var currentUser = GetCurrentUser();
            return currentUser.Achievements.Where(a => a.Progress == 100).AsQueryable();
        }

        // GET: api/RankingList
        [Route("api/RankingList")]
        public IQueryable<KeyValuePair<string,int>> GetRankingList()
        {
            using (var db = new IdentityDbContext())
            {

                var sortedUser = AchievementDb.GetAllUser().OrderByDescending(x => x.RankingPoints);
                var returnList = new Dictionary<string, int>();
                foreach (var user in sortedUser)
                {
                    var userName = db.Users.First(u => u.Id.Equals(user.ReleatedApplicationUserId)).UserName;
                    returnList.Add(userName, user.RankingPoints);
                }
                return returnList.AsQueryable();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            AchievementDb.Dispose();
        }
    }
}