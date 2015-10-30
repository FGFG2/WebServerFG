namespace WebServer.Models
{
    /// <summary>
    /// Base class for achievment data models
    /// </summary>
    public abstract class AchievementData : Entity
    {
        /// <summary>
        /// The time when the data was collected
        /// </summary>
        public virtual int TimeStamp { get; set; }
    }
}