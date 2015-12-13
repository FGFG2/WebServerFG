using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
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