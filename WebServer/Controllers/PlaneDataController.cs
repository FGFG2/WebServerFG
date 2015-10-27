using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebServer.Controllers
{
    /// <summary>
    /// Provides the RESTful API to fill the Database with sensor data of the plane.
    /// The provided data will be saved and used to calculate if the user reached a achievement.
    /// </summary>
    public class PlaneDataController : ApiController
    {
        // POST api/SetMotor
        [Route("api/SetMotor")]
        public HttpResponseMessage SetMotor(Dictionary<int, int> motorMap)
        {
            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        // POST api/SetRuder
        [Route("api/SetRuder")]
        public HttpResponseMessage SetRuder(Dictionary<int, int> ruderMap)
        {
            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        // POST api/SetIsConnected
        [Route("api/SetIsConnected")]
        public HttpResponseMessage SetIsConnected(Dictionary<int, bool> connectionChanges)
        {
            return new HttpResponseMessage(HttpStatusCode.Created);
        }
    }
}
