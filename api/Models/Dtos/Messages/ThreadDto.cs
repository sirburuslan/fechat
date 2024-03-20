/*
 * @class Thread Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-03
 *
 * This class is used for thread data transfer
 */

// Namespace for Messages Dtos
namespace FeChat.Models.Dtos.Messages {

    /// <summary>
    /// Thread Dto
    /// </summary>
    public class ThreadDto {

        /// <summary>
        /// Thread ID field
        /// </summary>
        public int ThreadId { get; set; }       

        /// <summary>
        /// Thread Secret field
        /// </summary>
        public string? ThreadSecret { get; set; } 

        /// <summary>
        /// Member ID field
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// Website ID field
        /// </summary>
        public int WebsiteId { get; set; }

        /// <summary>
        /// Created Time
        /// </summary>
        public int Created { get; set; }

        /// <summary>
        /// Guest ID field
        /// </summary>
        public int GuestId { get; set; }

        /// <summary>
        /// Guest Name field
        /// </summary>
        public string? GuestName { get; set; }  

        /// <summary>
        /// Guest Email field
        /// </summary>
        public string? GuestEmail { get; set; }  

        /// <summary>
        /// Guest Ip field
        /// </summary>
        public string? GuestIp { get; set; }  

       /// <summary>
        /// Guest Latitude field
        /// </summary>
        public string? GuestLatitude { get; set; } 

        /// <summary>
        /// Guest Longitude field
        /// </summary>
        public string? GuestLongitude { get; set; }  

    }

}