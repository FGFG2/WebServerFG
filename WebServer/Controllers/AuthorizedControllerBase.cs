using System.Web.Http;
using Microsoft.AspNet.Identity;
using WebServer.DataContext;
using WebServer.Models;

namespace WebServer.Controllers
{
    /// <summary>
    /// Base class for all controller needing authorization. Authorization is provided using the passed bearer token.
    /// </summary>
    /// Class is inheriting ApiController instead of being an utility class, because it won't be possible to map the token to a user.
    /// Therefore inheritance is being used, even though polymorphism is not being used at this moment.
    [Authorize]
    public abstract class AuthorizedControllerBase : ApiController
    {
        protected readonly IAchievementDb AchievementDb;
        
        protected AuthorizedControllerBase(IAchievementDb achievementDb)
        {
            AchievementDb = achievementDb;
        }

        /// <summary>
        /// Returns the user associated with the token used.
        /// </summary>
        /// <returns></returns>
        protected SmartPlaneUser GetCurrentUser()
        {
            var callingUser = RequestContext.Principal.Identity.GetUserId();
            var currentUser = AchievementDb.GetSmartPlaneUserByApplicationUserId(callingUser);
            return currentUser;
        }
    }
}