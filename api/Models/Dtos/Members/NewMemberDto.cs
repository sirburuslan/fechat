/*
 * @class New Member Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used for new member data sanitization and validation
 */

// Namespace for Members Dtos
namespace FeChat.Models.Dtos.Members {

    // System Namespaces
    using System.ComponentModel.DataAnnotations;
    using System.Text.Encodings.Web;
    using System.Web;
    
    // App Namespaces
    using Utils.General;
    using Utils.Validations;

    /// <summary>
    /// New Member Dto
    /// </summary>
    public class NewMemberDto {

        /// <summary>
        /// First name container
        /// </summary>
        private string? _firstName;

        /// <summary>
        /// Last name container
        /// </summary>
        private string? _lastName;  

        /// <summary>
        /// Email container
        /// </summary>
        private string? _email;      

        /// <summary>
        /// Member's ID field
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// Member's first name field
        /// </summary>
        [StringLength(20, MinimumLength = 0, ErrorMessageResourceName = "FirstNameLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? FirstName {
            get => _firstName;
            set => _firstName = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();
        }

        /// <summary>
        /// Member's last name field
        /// </summary>
        [StringLength(20, MinimumLength = 0, ErrorMessageResourceName = "LastNameLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? LastName {
            get => _lastName;
            set => _lastName = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();
        }

        /// <summary>
        /// Member's email field
        /// </summary>
        [EmailAddress(ErrorMessageResourceName = "EmailNotValid", ErrorMessageResourceType = typeof(ErrorMessages))]
        [StringLength(200, ErrorMessageResourceName = "EmailLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? Email {
            get => _email;
            set => _email = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty)).Trim();
        }

        /// <summary>
        /// Member's role field
        /// </summary>
        [NumberValidation(Minimum = 0, Maximum = 1, ErrorMessage = "SupportedValueShouldBe")]
        public int Role { get; set; }

        /// <summary>
        /// Member's password field
        /// </summary>
        [StringLength(20, MinimumLength = 6, ErrorMessageResourceName = "PasswordLength", ErrorMessageResourceType = typeof(ErrorMessages))]
        [RegularExpression(@"^\S*$", ErrorMessage = "PasswordNoSpaces", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? Password { get; set; }       

        /// <summary>
        /// Joined time
        /// </summary>
        public int Created { get; set; }
    }

}