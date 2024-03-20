/*
 * @class Website Entity
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-22
 *
 * This class is used build the Website entity
 */

// Namespace for Websites Entities
namespace FeChat.Models.Entities.Websites {

    // Use the Annotations
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Entity for Websites
    /// </summary>
    public class WebsiteEntity {

        /// <summary>
        /// Website ID AutoIncrement Key
        /// </summary>
        [Key]
        [Required]
        public int WebsiteId { get; set; }

        /// <summary>
        /// Member's ID
        /// </summary>
        [Required]
        public int MemberId { get; set; }

        /// <summary>
        /// Enabled Status
        /// </summary>
        public int Enabled { get; set; }

        /// <summary>
        /// Chat Header
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(20)]
        public string? Header { get; set; }        

        /// <summary>
        /// Website Name
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(250)]
        public string? Name { get; set; }

        /// <summary>
        /// Website Url
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(250)]
        public string? Url { get; set; }

        /// <summary>
        /// Website Domain
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(250)]
        public string? Domain { get; set; }        

        /// <summary>
        /// Created time field
        /// </summary>
        [Required]
        public int Created { get; set; }   

    }

}