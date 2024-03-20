/*
 * @class Event Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-11
 *
 * This class is used for event data transfer
 */

// Namespace for Events Dtos
namespace FeChat.Models.Dtos.Events {

    /// <summary>
    /// Event Dto
    /// </summary>
    public class EventDto {

        /// <summary>
        /// Event's ID field
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// Member's ID field
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// Type's ID field
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// Created field
        /// </summary>
        public int Created { get; set; }  

        /// <summary>
        /// First Name field
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Last Name field
        /// </summary>
        public string? LastName { get; set; }  
        
        /// <summary>
        /// Profile Photo field
        /// </summary>
        public string? ProfilePhoto { get; set; }  
        
        /// <summary>
        /// Email field
        /// </summary>
        public string? Email { get; set; }                      

    }

}