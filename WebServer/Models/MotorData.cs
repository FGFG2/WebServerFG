namespace WebServer.Models
{
    /// <summary>
    /// Dataclass for a motor of a plane
    /// </summary>
    public class MotorData : AchievementData
    {
        /// <summary>
        /// The specific data value of a motor
        /// </summary>
        public virtual int Value { get; set; }
    }
}