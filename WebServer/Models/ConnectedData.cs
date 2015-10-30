using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models
{
    public class ConnectedData : AchievementData
    {
        public virtual bool IsConnected { get; set; }
    }
}