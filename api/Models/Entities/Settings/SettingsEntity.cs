/*
 * @class Settings Entity
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-01-26
 *
 * This class is used build the Settings entity
 */

// Namespace for Settings Entity
namespace FeChat.Models.Entities.Settings {

    // Use the Adnnotations for attributes
    using System.ComponentModel.DataAnnotations; 

    /// <summary>
    /// Settings Entity
    /// </summary>
    public class SettingsEntity {

        /// <summary>
        /// Option Id Key
        /// </summary>
        [Key]
        [Required]
        public int OptionId { get; set; }

        /// <summary>
        /// Option Name
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(250)]
        public required string OptionName { get; set; }

        /// <summary>
        /// Option Value
        /// </summary>
        [DataType(DataType.Text)]
        public string? OptionValue { get; set; }

    }

}