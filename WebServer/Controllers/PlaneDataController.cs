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
    /// Provides the RESTful API to fill the Database with sensor data of the plane.
    /// The provided data will be saved and used to calculate if the user reached a achievement.
    /// </summary>
    public class PlaneDataController : ApiController
    {
        private IAchievementDb _achievementDb;

        public PlaneDataController(IAchievementDb achievementDb)
        {
            _achievementDb = achievementDb;
        }

        // POST api/SetMotor
        [Route("api/SetMotor")]
        public HttpResponseMessage SetMotor(Dictionary<int, int> motorMap)
        {
            var currentUser = _achievementDb.GetSmartPlaneUserById(0);            
            foreach (var motorData in motorMap)
            {
                currentUser.MotorDatas.Add(new MotorData {TimeStamp = motorData.Key, Value = motorData.Value});
            }
            _achievementDb.SaveChanges();
            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        // POST api/SetRuder
        [Route("api/SetRuder")]
        public HttpResponseMessage SetRudder(Dictionary<int, int> ruderMap)
        {
            var currentUser = _achievementDb.GetSmartPlaneUserById(0);
            foreach (var rudderData in ruderMap)
            {
                currentUser.RudderDatas.Add(new RudderData {TimeStamp = rudderData.Key, Value = rudderData.Value});
            }
            _achievementDb.SaveChanges();
            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        // POST api/SetIsConnected
        [Route("api/SetIsConnected")]
        public HttpResponseMessage SetIsConnected(Dictionary<int, bool> connectionChanges)
        {
            var currentUser = _achievementDb.GetSmartPlaneUserById(0);
            foreach (var connectionChange in connectionChanges)
            {
                currentUser.ConnectedDatas.Add(new ConnectedData {TimeStamp = connectionChange.Key, IsConnected = connectionChange.Value});
            }
            _achievementDb.SaveChanges();
            return new HttpResponseMessage(HttpStatusCode.Created);
        }
    }
}
