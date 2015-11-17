namespace WebServer.Models
{
    /// <summary>
    /// Base class for achievment data models
    /// </summary>
    public abstract class AchievementData<T> : Entity
    {
        /// <summary>
        /// The time when the data was collected
        /// </summary>
        public virtual long TimeStamp { get; set; }

        public virtual T Value { get; set; }
    }
}