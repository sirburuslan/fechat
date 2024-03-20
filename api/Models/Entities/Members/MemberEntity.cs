/*
 * @class Member Entity
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-01-26
 *
 * This class is used build the Member entity
 */

// Define the entities models namespace
namespace FeChat.Models.Entities.Members {

    // Import the Annotations for attributes
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Member entity
    /// </summary>
    public class MemberEntity {

        /// <summary>
        /// Member's ID field
        /// </summary>
        [Key]
        [Required]
        public int MemberId { get; set; }

        /// <summary>
        /// Member's first name field
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(50)]
        public string? FirstName { get; set; }

        /// <summary>
        /// Member's last name field
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(50)]
        public string? LastName { get; set; }

        /// <summary>
        /// Member's email field
        /// </summary>
        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        /// <summary>
        /// Member's role field
        /// </summary>
        [Required]
        public int Role { get; set; }

        /// <summary>
        /// Member's password field
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(250)]
        public string? Password { get; set; }

        /// <summary>
        /// Member's reset code field
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(20)]
        public string? ResetCode { get; set; }

        /// <summary>
        /// Member's reset time field
        /// </summary>
        public int ResetTime { get; set; }

        /// <summary>
        /// Member's joined time field
        /// </summary>
        [Required]
        public int Created { get; set; }     

    }

}