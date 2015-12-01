using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Practices.ObjectBuilder2;
using Newtonsoft.Json.Serialization;
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

        private bool _checkData<T>(Dictionary<long, T> map)
        {
            if (map == null || !map.Any())
            {
                return false;
            }
            return true;
        }

        private HttpResponseMessage _saveChanges(SmartPlaneUser targetUser)
        {
            _achievementDb.SaveChanges();
            _calculationManager.UpdateForUser(targetUser.Id);
            return new HttpResponseMessage(HttpStatusCode.Created);
        }


        private HttpResponseMessage _addDataToUser<DataValueType, AchievementDataType>(SmartPlaneUser targetUser, IList<AchievementDataType> targetList, Dictionary<long, DataValueType> dataMap)
            where AchievementDataType : AchievementData<DataValueType>, new()
        {
            if (!_checkData(dataMap))
            {
                _logger.Log("Setter called, but received no data.", LogLevel.Info);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            try
            {
                foreach (var dataValue in dataMap)
                {
                    targetList.Add(new AchievementDataType { TimeStamp = dataValue.Key, Value = dataValue.Value });
                }
                _logger.Log($"Added {dataMap.Count} new entries to user with ID {targetUser.Id}.", LogLevel.Debug);

                return _saveChanges(targetUser);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exception.Message);
            }

        }

        #region apiSetter
        // POST api/SetMotor
        [Route("api/SetMotor")]
        public HttpResponseMessage SetMotor(Dictionary<long, int> motorMap)
        {
            var currentUser = _getCurrentUser();
            return _addDataToUser(currentUser, currentUser.MotorDatas, motorMap);
        }

        // POST api/SetRuder
        [Route("api/SetRuder")]
        public HttpResponseMessage SetRudder(Dictionary<long, int> rudderMap)
        {

            var currentUser = _getCurrentUser();
            return _addDataToUser(currentUser, currentUser.RudderDatas, rudderMap);
        }

        // POST api/SetIsConnected
        [Route("api/SetIsConnected")]
        public HttpResponseMessage SetIsConnected(Dictionary<long, bool> connectionChanges)
        {
            var currentUser = _getCurrentUser();
            return _addDataToUser(currentUser, currentUser.ConnectedDatas, connectionChanges);
        }

        // POST api/ResetAllData
        [Route("api/ResetAllData")]
        public HttpResponseMessage ResetAllData(Dictionary<long, string> password)
        {
            if (_checkData(password) == false)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "No password");
            }
            if (password.First().Value.Equals("FgFg2") == false)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Wrong password");
            }
            _achievementDb.ResetAllData();
            _calculationManager.UpdateForUser(0);
            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        #endregion

        #region apiGetter
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
        
        // GET: api/Logs
        [Route("api/Logs")]
        public IQueryable<LogEntry> GetLogs()
        {
            return _achievementDb.GetAllLogEntries().AsQueryable(); 
        }
        #endregion


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
            _calculationManager.Dispose();
            _achievementDb.Dispose();
        }
    }
}
