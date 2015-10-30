namespace WebServer.Models
{
    /// <summary>
    /// Dataclass for a rudder of the plane
    /// </summary>
    public class RudderData : AchievementData
    {
        /// <summary>
        /// The specific calue of a Rudder
        /// </summary>
        public virtual int Value { get; set; }
    }
}