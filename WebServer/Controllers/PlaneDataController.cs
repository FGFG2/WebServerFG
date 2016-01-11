using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.AspNet.Identity;
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
    [Authorize]
    public class PlaneDataController : AuthorizedControllerBase
    {
        private readonly IAchievementCalculationManager _calculationManager;
        private readonly ILoggerFacade _logger;

        /// <summary>
        /// Creates a new instance of the PlaneDataController class.
        /// </summary>
        /// <param name="achievementDb">Database used for the Achievement system.</param>
        /// <param name="calculationManager">CalculationManager used to update the achievements.</param>
        /// <param name="logger">Logger to use.</param>
        public PlaneDataController(IAchievementDb achievementDb, IAchievementCalculationManager calculationManager, ILoggerFacade logger) : base(achievementDb)
        {
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
            AchievementDb.SaveChanges();
            _calculationManager.UpdateForUser(targetUser.Id);
            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        private HttpResponseMessage _addDataToUser<TDataValueType, TAchievementDataType>(SmartPlaneUser targetUser, IList<TAchievementDataType> targetList, Dictionary<long, TDataValueType> dataMap)
            where TAchievementDataType : AchievementData<TDataValueType>, new()
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
                    targetList.Add(new TAchievementDataType { TimeStamp = dataValue.Key, Value = dataValue.Value });
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
            var currentUser = GetCurrentUser();
            return _addDataToUser(currentUser, currentUser.MotorDatas, motorMap);
        }

        // POST api/SetRuder
        [Route("api/SetRuder")]
        public HttpResponseMessage SetRudder(Dictionary<long, int> rudderMap)
        {

            var currentUser = GetCurrentUser();
            return _addDataToUser(currentUser, currentUser.RudderDatas, rudderMap);
        }

        // POST api/SetIsConnected
        [Route("api/SetIsConnected")]
        public HttpResponseMessage SetIsConnected(Dictionary<long, bool> connectionChanges)
        {
            var currentUser = GetCurrentUser();
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
            AchievementDb.ResetAllData();
            _calculationManager.UpdateForUser(0);
            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        #endregion

        #region apiGetter
        // GET: api/MotorDatas
        [Route("api/MotorDatas")]
        public IQueryable<MotorData> GetMotorDatas()
        {
            var currentUser = GetCurrentUser();
            return currentUser.MotorDatas.AsQueryable();
        }

        // GET: api/ConnectedDatas
        [Route("api/ConnectedDatas")]
        public IQueryable<ConnectedData> GetConnectedDatas()
        {
            var currentUser = GetCurrentUser();
            return currentUser.ConnectedDatas.AsQueryable();
        }

        // GET: api/RudderDatas
        [Route("api/RudderDatas")]
        public IQueryable<RudderData> GetRudderDatas()
        {
            var currentUser = GetCurrentUser();
            return currentUser.RudderDatas.AsQueryable();
        }
        
        // GET: api/Logs
        [Route("api/Logs")]
        public IQueryable<LogEntry> GetLogs()
        {
            return AchievementDb.GetAllLogEntries().AsQueryable(); 
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            _calculationManager.Dispose();
            AchievementDb.Dispose();
        } 
        #endregion
    }
}
