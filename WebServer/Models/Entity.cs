namespace WebServer.Models
{
    /// <summary>
    /// Base class for all entity which will be stored in the db
    /// </summary>
    public abstract class Entity
    {
        public virtual int Id { get; set; }
    }
}