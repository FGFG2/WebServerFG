using System.ComponentModel.DataAnnotations;

namespace WebServer.Models
{
    /// <summary>
    /// Model class for a single achievement. Contains detailed information and the progress.
    /// </summary>
    public class Achievement : Entity
    {
        /// <summary>
        /// Name of the achievement.
        /// </summary>
        [Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Detailed description containing the goals needed to obtain the achievement.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// URL to the image used for the visual representation.
        /// </summary>
        public virtual string ImageUrl { get; set; }

        /// <summary>
        /// Progress of the achievement in percent. A value of 100 means it has been finished.
        /// </summary>
        public virtual byte Progress { get; set; }
    }
}