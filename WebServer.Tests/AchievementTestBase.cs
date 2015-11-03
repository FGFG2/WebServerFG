using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universial.Test;
using WebServer.Models;

namespace WebServer.Tests
{
    public class AchievementTestBase<T> : TestBase<T> where T : class
    {
        protected SmartPlaneUser CreateSmartPlaneUser()
        {
            return new SmartPlaneUser
            {
                Achievements = new List<Achievement>(),
                ConnectedDatas = new List<ConnectedData>(),
                MotorDatas = new List<MotorData>(),
                RudderDatas = new List<RudderData>()
            };
        }
    }
}
