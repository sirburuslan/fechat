/*
 * @class Plan Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-13
 *
 * This class is used for plan data sanitization and validation
 */

// Namespace for Plans Dtos
namespace FeChat.Models.Dtos.Plans {

    // System Namespaces
    using System.ComponentModel.DataAnnotations;
    using System.Text.Encodings.Web;
    using System.Web;

    // App Namespaces
    using Utils.General;

    /// <summary>
    /// Dto for Plan
    /// </summary>
    public class PlanDto {

        /// <summary>
        /// Plan name container
        /// </summary>
        private string? _name;

        /// <summary>
        /// Plan price container
        /// </summary>
        private string? _price;   

        /// <summary>
        /// Plan currency container
        /// </summary>
        private string? _currency;         

        /// <summary>
        /// Plan's ID field
        /// </summary>
        public int PlanId { get; set; }

        /// <summary>
        /// Plan name field
        /// </summary>
        [StringLength(200, MinimumLength = 0, ErrorMessageResourceName = "PlanNameLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? Name {
            get => _name;
            set => _name = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();
        }

        /// <summary>
        /// Plan price field
        /// </summary>
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 0, ErrorMessageResourceName = "PlanPriceLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? Price {
            get => _price;
            set => _price = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();            
        }

        /// <summary>
        /// Plan currency field
        /// </summary>
        [DataType(DataType.Text)]
        [StringLength(5, MinimumLength = 0, ErrorMessageResourceName = "PlanCurrencyLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? Currency {
            get => _currency;
            set => _currency = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();               
        }

        /// <summary>
        /// Plan created field
        /// </summary>
        public int? Created { get; set; }

        /// <summary>
        /// Plan Features field
        /// </summary>
        public List<FeatureDto>? Features { get; set; }        

    }

}