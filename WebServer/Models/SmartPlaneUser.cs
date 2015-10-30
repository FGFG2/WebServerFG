using System.Collections.Generic;

namespace WebServer.Models
{
    public class SmartPlaneUser : Entity
    {
        public virtual int ReleatedApplicationUserId { get; set; }
        public virtual IList<Achievement> Achievements { get; set; }
        public virtual IList<MotorData> MotorDatas { get; set; }
        public virtual IList<RudderData> RudderDatas { get; set; }
        public virtual IList<ConnectedData> ConnectedDatas { get; set; }
    }
}