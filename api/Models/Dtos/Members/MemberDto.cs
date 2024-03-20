/*
 * @class Member Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used for member data transfer
 */

// Namespace for Members Dtos
namespace FeChat.Models.Dtos.Members {

    /// <summary>
    /// Member Dto
    /// </summary>
    public class MemberDto {         

        /// <summary>
        /// Member's ID field
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// Member's first name field
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Member's last name field
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Member's email field
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Member's phone field
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Member's language field
        /// </summary>
        public string? Language { get; set; }

        /// <summary>
        /// Member's role field
        /// </summary>
        public int Role { get; set; }

        /// <summary>
        /// Member's password field
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Member's repeat password field
        /// </summary>
        public string? RepeatPassword { get; set; }

        /// <summary>
        /// Member's reset code field
        /// </summary>
        public string? ResetCode { get; set; }  
        
        /// <summary>
        /// Reset time
        /// </summary>
        public int ResetTime { get; set; }          

        /// <summary>
        /// Joined time
        /// </summary>
        public int Created { get; set; }

        /// <summary>
        /// Profile Photo
        /// </summary>
        public string? ProfilePhoto { get; set; }

        /// <summary>
        /// Member's plan id field
        /// </summary>
        public int PlanId { get; set; }

        /// <summary>
        /// Notifications Status field
        /// </summary>
        public int NotificationsEnabled { get; set; }

        /// <summary>
        /// Notifications Email field
        /// </summary>
        public string? NotificationsEmail { get; set; }

    }

}