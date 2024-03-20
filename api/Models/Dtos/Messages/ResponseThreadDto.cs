/*
 * @class Response Thread Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-05
 *
 * This class is used for responses with threads list
 */

// Namespace for Messages Dtos
namespace FeChat.Models.Dtos.Messages {

    /// <summary>
    /// Response Thread Dto
    /// </summary>
    public class ResponseThreadDto {

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

        /// <summary>
        /// Message Id field
        /// </summary>
        public int MessageId { get; set; }  

        /// <summary>
        /// Message field
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Message Seen field
        /// </summary>
        public int MessageSeen { get; set; }          
        
        /// <summary>
        /// Message Created Time field
        /// </summary>
        public int MessageCreated { get; set; }  

        /// <summary>
        /// Total Messages in a Thread field
        /// </summary>
        public int TotalMessages { get; set; }

    }

}