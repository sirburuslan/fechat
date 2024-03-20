/*
 * @class Guest Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-04
 *
 * This class is used for guest data transfer
 */

// Namespace for Messages Dtos
namespace FeChat.Models.Dtos.Messages {

    /// <summary>
    /// Guest Dto
    /// </summary>
    public class GuestDto {

        /// <summary>
        /// Guest Id field
        /// </summary>
        public int GuestId { get; set; }                     

        /// <summary>
        /// Guest Name field
        /// </summary>
        public string? Name { get; set; }        

        /// <summary>
        /// Guest Email field
        /// </summary>
        public string? Email { get; set; } 

        /// <summary>
        /// Guest Ip field
        /// </summary>
        public string? Ip { get; set; } 

        /// <summary>
        /// Guest Latitude field
        /// </summary>
        public string? Latitude { get; set; }

        /// <summary>
        /// Guest Longitude field
        /// </summary>
        public string? Longitude { get; set; }   

    }

}