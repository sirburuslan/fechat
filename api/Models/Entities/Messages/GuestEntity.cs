/*
 * @class Guest Entity
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-26
 *
 * This class is used build the Guest entity
 */

// Namespace for Messages Entities
namespace FeChat.Models.Entities.Messages {

    // Use the Adnnotations for attributes
    using System.ComponentModel.DataAnnotations; 

    /// <summary>
    /// Guest Entity
    /// </summary>
    public class GuestEntity {

        /// <summary>
        /// Guest's ID
        /// </summary>
        [Key]
        [Required]
        public int GuestId { get; set; }

        /// <summary>
        /// Guest Name
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(50)]
        public string? Name { get; set; }

        /// <summary>
        /// Guest Email
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(200)]
        public string? Email { get; set; }

        /// <summary>
        /// Ip address
        /// </summary>
        [Required]
        public string? Ip { get; set; }  
        
        /// <summary>
        /// Guest Latitude
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(200)]
        public string? Latitude { get; set; } 
        
        /// <summary>
        /// Guest Longitude
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(200)]
        public string? Longitude { get; set; }                      

        /// <summary>
        /// Created time field
        /// </summary>
        [Required]
        public int Created { get; set; }
        
    }

}