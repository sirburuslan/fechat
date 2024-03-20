/*
 * @class Restrictions Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-16
 *
 * This class is used for plans restrictions data transfer
 */

// Namespace for the plans restrictions
namespace FeChat.Models.Dtos.Plans {

    // Use the net annotations
    using System.ComponentModel.DataAnnotations;

    // Use the Mvc features
    using Microsoft.AspNetCore.Mvc;

    // Use the Binders
    using FeChat.Models.Binders;

    // Use the custom validations
    using FeChat.Utils.Validations;

    /// <summary>
    /// Dto for Plans Restrictions
    /// </summary>
    [ModelBinder(typeof(RestrictionsDtoBinder))]
    public class RestrictionsDto {

        /// <summary>
        /// Plan's ID
        /// </summary>
        public int PlanId { get; set; }

        /// <summary>
        /// Allowed websites
        /// </summary>
        [NumberValidation(Minimum = 0, Maximum = 50, ErrorMessage = "TooManyWebsites")]
        public int Websites { get; set; }    

    }

}