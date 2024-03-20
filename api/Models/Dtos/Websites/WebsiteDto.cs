/*
 * @class Website Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-14
 *
 * This class is used for website data transfer
 */

// Namespace for Websites Dtos
namespace FeChat.Models.Dtos.Websites {

    /// <summary>
    /// Dto for Websites
    /// </summary>
    public class WebsiteDto {              

        /// <summary>
        /// Website's ID field
        /// </summary>
        public int WebsiteId { get; set; }

        /// <summary>
        /// Member's ID
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// Chat status
        /// </summary>
        public int Enabled { get; set; }   

        /// <summary>
        /// Chat header field
        /// </summary>
        public string? Header { get; set; }

        /// <summary>
        /// Website name field
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Website url field
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Website domain field
        /// </summary>
        public string? Domain { get; set; }

        /// <summary>
        /// Website created field
        /// </summary>
        public int Created { get; set; }

        /// <summary>
        /// Member ID field
        /// </summary>
        public int MemberID { get; set; }
        
        /// <summary>
        /// Member First Name field
        /// </summary>
        public string? FirstName { get; set; } 

        /// <summary>
        /// Member Last Name field
        /// </summary>
        public string? LastName { get; set; }         
        
        /// <summary>
        /// Member Profile Photo field
        /// </summary>
        public string? ProfilePhoto { get; set; }                        

    }

}