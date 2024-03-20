/*
 * @class Options Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-01-26
 *
 * This class is used for options data transfer
 */

// Namespace for Members Dtos
namespace FeChat.Models.Dtos.Members {

    // Use the DataAnotations library to add support for attributes
    using System.ComponentModel.DataAnnotations;

    // Use General for error messages
    using FeChat.Utils.General;

    // Use the custom validations
    using FeChat.Utils.Validations;

    /// <summary>
    /// Dto for Member
    /// </summary>
    public class OptionsDto {

        /// <summary>
        /// Member's ID
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// Sidebar status
        /// </summary>
        [NumberValidation(Minimum = 0, Maximum = 1, ErrorMessage = "SupportedValueShouldBe")]
        public int? SidebarStatus { get; set; }   
        
        /// <summary>
        /// Members chart time
        /// </summary>
        [NumberValidation(Minimum = 1, Maximum = 4, ErrorMessage = "SupportedValueShouldBe")]
        public int? MembersChartTime { get; set; } 
        
        /// <summary>
        /// Threads chart time
        /// </summary>
        [NumberValidation(Minimum = 1, Maximum = 4, ErrorMessage = "SupportedValueShouldBe")]
        public int? ThreadsChartTime { get; set; }                 

    }

}