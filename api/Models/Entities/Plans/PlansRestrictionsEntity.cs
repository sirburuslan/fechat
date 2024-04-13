/*
 * @class Plan Restrictions Entity
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-15
 *
 * This class is used build the Plan Restrictions entity
 */

// Namespace for the Plans Entities
namespace FeChat.Models.Entities.Plans {

    // System Namespaces
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Plans Restrictions Entity
    /// </summary>
    public class PlansRestrictionsEntity {
        
        /// <summary>
        /// Restriction ID
        /// </summary>
        [Key]
        [Required]
        public int RestrictionId { get; set; }

        /// <summary>
        /// Plan ID
        /// </summary>
        [Required]
        public int PlanId { get; set; }

        /// <summary>
        /// Restriction Name
        /// </summary>
        [Required]
        [MaxLength(250)]
        public required string RestrictionName { get; set; }

        /// <summary>
        /// Restriction Value
        /// </summary>
        public int RestrictionValue { get; set; }

    }

}