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
        public string Name { get; set; }

        /// <summary>
        /// Detailed description containing the goals needed to obtain the achievement.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// URL to the image used for the visual representation.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Progress of the achievement in percent. A value of 100 means it has been finished.
        /// </summary>
        public byte Progress { get; set; }
    }
}