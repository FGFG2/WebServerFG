using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebServer.BusinessLogic;
using WebServer.DataContext;
using WebServer.Logging;
using WebServer.Models;

namespace WebServer.Controllers
{
    /// <summary>
    /// Provides the RESTful API to fill the Database with sensor data of the plane.
    /// The provided data will be saved and used to calculate if the user reached a achievement.
    /// </summary>
    public class PlaneDataController : ApiController
    {
        private readonly IAchievementDb _achievementDb;
        private readonly IAchievementCalculationManager _calculationManager;
        private readonly ILoggerFacade _logger;

        public PlaneDataController(IAchievementDb achievementDb, IAchievementCalculationManager calculationManager, ILoggerFacade logger)
        {
            _achievementDb = achievementDb;
            _calculationManager = calculationManager;
            _logger = logger;
        }

        // POST api/SetMotor
        [Route("api/SetMotor")]
        public HttpResponseMessage SetMotor(Dictionary<int, int> motorMap)
        {
            if (motorMap == null || !motorMap.Any())
            {
                _logger.Log("SetMotor called, but received no data.", LogLevel.Info);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            try
            {
                var currentUser = _achievementDb.GetSmartPlaneUserById(0);
                foreach (var motorData in motorMap)
                {
                    currentUser.MotorDatas.Add(new MotorData { TimeStamp = motorData.Key, Value = motorData.Value });
                }
                _achievementDb.SaveChanges();
                _logger.Log($"Added {motorMap.Count} new entries with motor data to user with ID {currentUser.Id}.", LogLevel.Debug);

                _calculationManager.UpdateForUser(currentUser.Id);
                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exception.Message);                
            }                                            
        }

        // POST api/SetRuder
        [Route("api/SetRuder")]
        public HttpResponseMessage SetRudder(Dictionary<int, int> rudderMap)
        {
            if (rudderMap == null || !rudderMap.Any())
            {
                _logger.Log("SetRudder called, but received no data.", LogLevel.Info);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            try
            {
                var currentUser = _achievementDb.GetSmartPlaneUserById(0);
                foreach (var rudderData in rudderMap)
                {
                    currentUser.RudderDatas.Add(new RudderData { TimeStamp = rudderData.Key, Value = rudderData.Value });
                }
                _achievementDb.SaveChanges();
                _logger.Log($"Added {rudderMap.Count} new entries with rudder data to user with ID {currentUser.Id}.", LogLevel.Debug);

                _calculationManager.UpdateForUser(currentUser.Id);
                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exception.Message);
            }           
        }

        // POST api/SetIsConnected
        [Route("api/SetIsConnected")]
        public HttpResponseMessage SetIsConnected(Dictionary<int, bool> connectionChanges)
        {
            if (connectionChanges == null || !connectionChanges.Any())
            {
                _logger.Log("SetIsConnected called, but received no data.", LogLevel.Info);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            try
            {
                var currentUser = _achievementDb.GetSmartPlaneUserById(0);
                foreach (var connectionChange in connectionChanges)
                {
                    currentUser.ConnectedDatas.Add(new ConnectedData { TimeStamp = connectionChange.Key, IsConnected = connectionChange.Value });
                }
                _achievementDb.SaveChanges();
                _logger.Log($"Added {connectionChanges.Count} new entries with connection changes to user with ID {currentUser.Id}.", LogLevel.Debug);

                _calculationManager.UpdateForUser(currentUser.Id);
                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exception.Message);                
            }            
        }

        // GET: api/MotorDatas
        [Route("api/MotorDatas")]
        public IQueryable<MotorData> GetMotorDatas()
        {
            var currentUser = _getCurrentUser();
            return currentUser.MotorDatas.AsQueryable();
        }

        // GET: api/ConnectedDatas
        [Route("api/ConnectedDatas")]
        public IQueryable<ConnectedData> GetConnectedDatas()
        {
            var currentUser = _getCurrentUser();
            return currentUser.ConnectedDatas.AsQueryable();
        }

        // GET: api/RudderDatas
        [Route("api/RudderDatas")]
        public IQueryable<RudderData> GetRudderDatas()
        {
            var currentUser = _getCurrentUser();
            return currentUser.RudderDatas.AsQueryable();
        }


        private SmartPlaneUser _getCurrentUser()
        {
            var currentUser = _achievementDb.GetSmartPlaneUserById(0);
            return currentUser;
        }
    }
}
