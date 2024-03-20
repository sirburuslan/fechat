/*
 * @class Plan Entity
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-14
 *
 * This class is used build the Plan entity
 */

// Namespace for Plans Entities
namespace FeChat.Models.Entities.Plans {

    // Use the Adnnotations for attributes
    using System.ComponentModel.DataAnnotations; 

    /// <summary>
    /// Plan Entity
    /// </summary>
    public class PlanEntity {

        /// <summary>
        /// Plan's ID
        /// </summary>
        [Key]
        [Required]
        public int PlanId { get; set; }

        /// <summary>
        /// Plan Name
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(250)]
        public string? Name { get; set; }

        /// <summary>
        /// Plan Price
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(50)]
        public string? Price { get; set; }

        /// <summary>
        /// Plan Currency
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(5)]
        public string? Currency { get; set; }

        /// <summary>
        /// Created time field
        /// </summary>
        [Required]
        public int Created { get; set; }   

    }

}