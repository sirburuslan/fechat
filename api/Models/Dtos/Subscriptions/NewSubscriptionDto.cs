/*
 * @class New Subscription Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-08
 *
 * This class is used to validate the subscriptions data
 */

// Namespace for Subscriptions dtos
namespace FeChat.Models.Dtos.Subscriptions {

    // System Namespaces
    using System.ComponentModel.DataAnnotations;
    using System.Text.Encodings.Web;
    using System.Web;

    // App Namespaces
    using Utils.General;

    /// <summary>
    /// New Subscription Dto
    /// </summary>
    public class NewSubscriptionDto {

        /// <summary>
        /// Payment Source Container
        /// </summary>
        private string? _source;

        /// <summary>
        /// Order ID Container
        /// </summary>
        private string? _orderId;

        /// <summary>
        /// Subscription ID Container
        /// </summary>
        private string? _subscriptionId;   

        /// <summary>
        /// Plan ID
        /// </summary>
        public int PlanId { get; set; }

        /// <summary>
        /// Payment Source Field
        /// </summary>
        [Required(ErrorMessageResourceName = "PaymentSourceNotValid", ErrorMessageResourceType = typeof(ErrorMessages))]
        [StringLength(50, MinimumLength = 2, ErrorMessageResourceName = "PaymentSourceNotValid", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string Source {
            get => _source!;
            set => _source = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();
        }

        /// <summary>
        /// Order ID Field
        /// </summary>
        [Required(ErrorMessageResourceName = "OrderIdNotValid", ErrorMessageResourceType = typeof(ErrorMessages))]
        [StringLength(200, MinimumLength = 2, ErrorMessageResourceName = "OrderIdNotValid", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string OrderId {
            get => _orderId!;
            set => _orderId = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();
        }  

        /// <summary>
        /// Subscription ID Field
        /// </summary>
        [Required(ErrorMessageResourceName = "SubscriptionIdNotValid", ErrorMessageResourceType = typeof(ErrorMessages))]
        [StringLength(200, MinimumLength = 2, ErrorMessageResourceName = "SubscriptionIdNotValid", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string SubscriptionId {
            get => _subscriptionId!;
            set => _subscriptionId = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();
        }  

    }

}