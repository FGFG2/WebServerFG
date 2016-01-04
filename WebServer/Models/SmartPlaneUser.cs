using System.Collections.Generic;

namespace WebServer.Models
{
    /// <summary>
    /// Model class for a single SmartPlaneUser. Stores the achievements and all relevant data to calculate them.
    /// </summary>
    public class SmartPlaneUser : Entity
    {
        public virtual string ReleatedApplicationUserId { get; set; }
        public virtual int RankingPoints { get; set; }
        public virtual IList<Achievement> Achievements { get; set; }
        public virtual IList<MotorData> MotorDatas { get; set; }
        public virtual IList<RudderData> RudderDatas { get; set; }
        public virtual IList<ConnectedData> ConnectedDatas { get; set; }
    }
}