/*
 * @class Member Options Entity
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-01-26
 *
 * This class is used build the Member Options entity
 */

// Define the entities models namespace
namespace FeChat.Models.Entities.Members {

    // System Namespaces
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Member Options entity
    /// </summary>
    public class MemberOptionsEntity {

        /// <summary>
        /// Option's ID field
        /// </summary>
        [Key]
        [Required]
        public int OptionId { get; set; }

        /// <summary>
        /// Member's ID field
        /// </summary>
        [Required]
        public int MemberId {get; set;}

        /// <summary>
        /// Member's option name field
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(250)]
        public required string OptionName { get; set; }

        /// <summary>
        /// Option's Value
        /// </summary>
        [DataType(DataType.Text)]
        public string? OptionValue { get; set; }        

    }

}