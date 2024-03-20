/*
 * @class Options Dto
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-18
 *
 * This class is used for options data transfer
 */

// Namespace for Settings Dtos
namespace FeChat.Models.Dtos.Settings {

    // Use the Annotations for attributes
    using System.ComponentModel.DataAnnotations;

    // Use Web system for html sanitizing
    using System.Web;

    // Text Enconding for Javascript sanitizing
    using System.Text.Encodings.Web;
    
    // Use General for error messages
    using FeChat.Utils.General;

    // Use the custom validations
    using FeChat.Utils.Validations;

    /// <summary>
    /// Options Data Transfer
    /// </summary>
    public class OptionsDto {

        /// <summary>
        /// Website Name Value
        /// </summary>
        private string? _websiteName;

        /// <summary>
        /// Home Page Logo value
        /// </summary>
        private string? _homePageLogo;

        /// <summary>
        /// Sign In Page Logo value
        /// </summary>
        private string? _signInPageLogo;  

        /// <summary>
        /// Dashboard Logo Small value
        /// </summary>
        private string? _dashboardLogoSmall;  

        /// <summary>
        /// Dashboard Logo Large value
        /// </summary>
        private string? _dashboardLogoLarge;  

        /// <summary>
        /// Privacy Policy value
        /// </summary>
        private string? _privacyPolicy;  

        /// <summary>
        /// Cookies value
        /// </summary>
        private string? _cookies;  

        /// <summary>
        /// Terms of Service value
        /// </summary>
        private string? _termsOfService;  

        /// <summary>
        /// Demo Video value
        /// </summary>
        private string? _demoVideo;  

        /// <summary>
        /// PayPal client id value
        /// </summary>
        private string? _payPalClientId;  
        
        /// <summary>
        /// PayPal client secret value
        /// </summary>
        private string? _payPalClientSecret;  
        
        /// <summary>
        /// Api 2 Location Key value
        /// </summary>
        private string? _ip2LocationKey; 

        /// <summary>
        /// Google Maps Key value
        /// </summary>
        private string? _googleMapsKey;  

        /// <summary>
        /// Google Secret Id value
        /// </summary>
        private string? _googleClientId;  

        /// <summary>
        /// Google Secret Key value
        /// </summary>
        private string? _googleClientSecret;   
        
        /// <summary>
        /// Google ReCaptcha Key value
        /// </summary>
        private string? _reCAPTCHAKey;                                 

        /// <summary>
        /// Smtp protocol value
        /// </summary>
        private string? _smtpProtocol;        

        /// <summary>
        /// Smtp host value
        /// </summary>
        private string? _smtpHost;

        /// <summary>
        /// Smtp port value
        /// </summary>
        private string? _smtpPort;  
        
        /// <summary>
        /// Smtp username value
        /// </summary>
        private string? _smtpUsername;
        
        /// <summary>
        /// Smtp password value
        /// </summary>
        private string? _smtpPassword;          

        /// <summary>
        /// Website Name Holder
        /// </summary>
        [StringLength(150, ErrorMessageResourceName = "WebsiteNameLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? WebsiteName {
            get => _websiteName;
            set => _websiteName = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty));
        }

        /// <summary>
        /// Logo for home page
        /// </summary>
        [UrlValidation(ErrorMessageResourceName = "UrlNotValid", ErrorMessageResourceType = typeof(ErrorMessages))]
        [StringLength(500, ErrorMessageResourceName = "UrlLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? HomePageLogo {
            get => _homePageLogo;
            set => _homePageLogo = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty));
        }

        /// <summary>
        /// Logo for sign in page
        /// </summary>
        [UrlValidation(ErrorMessageResourceName = "UrlNotValid", ErrorMessageResourceType = typeof(ErrorMessages))]
        [StringLength(500, ErrorMessageResourceName = "UrlLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? SignInPageLogo {
            get => _signInPageLogo;
            set => _signInPageLogo = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty));
        }
        
        /// <summary>
        /// Logo for dashboard page
        /// </summary>
        [UrlValidation(ErrorMessageResourceName = "UrlNotValid", ErrorMessageResourceType = typeof(ErrorMessages))]
        [StringLength(500, ErrorMessageResourceName = "UrlLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? DashboardLogoSmall {
            get => _dashboardLogoSmall;
            set => _dashboardLogoSmall = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty));
        }       
        
        /// <summary>
        /// Logo for dashboard page
        /// </summary>
        [UrlValidation(ErrorMessageResourceName = "UrlNotValid", ErrorMessageResourceType = typeof(ErrorMessages))]
        [StringLength(500, ErrorMessageResourceName = "UrlLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? DashboardLogoLarge {
            get => _dashboardLogoLarge;
            set => _dashboardLogoLarge = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty));
        }                

        /// <summary>
        /// Website Analytics Code
        /// </summary>
        [StringLength(2000, ErrorMessageResourceName = "AnalyticsCodeLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? AnalyticsCode { get; set; } // No sanitization rules because is expected the analytics code 

        /// <summary>
        /// Privacy Policy page
        /// </summary>
        [UrlValidation(ErrorMessageResourceName = "UrlNotValid", ErrorMessageResourceType = typeof(ErrorMessages))]
        [StringLength(500, ErrorMessageResourceName = "UrlLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? PrivacyPolicy {
            get => _privacyPolicy;
            set => _privacyPolicy = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty));
        }  

        /// <summary>
        /// Cookies page
        /// </summary>
        [UrlValidation(ErrorMessageResourceName = "UrlNotValid", ErrorMessageResourceType = typeof(ErrorMessages))]
        [StringLength(500, ErrorMessageResourceName = "UrlLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? Cookies {
            get => _cookies;
            set => _cookies = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty));
        } 
        
        /// <summary>
        /// Terms Of Service page
        /// </summary>
        [UrlValidation(ErrorMessageResourceName = "UrlNotValid", ErrorMessageResourceType = typeof(ErrorMessages))]
        [StringLength(500, ErrorMessageResourceName = "UrlLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? TermsOfService {
            get => _termsOfService;
            set => _termsOfService = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty));
        } 

        /// <summary>
        /// Demo Video page
        /// </summary>
        [UrlValidation(ErrorMessageResourceName = "UrlNotValid", ErrorMessageResourceType = typeof(ErrorMessages))]
        [StringLength(500, ErrorMessageResourceName = "UrlLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? DemoVideo {
            get => _demoVideo;
            set => _demoVideo = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty));
        }                

        /// <summary>
        /// Registration Status
        /// </summary>
        [NumberValidation(Minimum = 0, Maximum = 1, ErrorMessage = "SupportedValueShouldBe")]
        public int? RegistrationEnabled { get; set; }  

        /// <summary>
        /// Registration Confirmation Status
        /// </summary>
        [NumberValidation(Minimum = 0, Maximum = 1, ErrorMessage = "SupportedValueShouldBe")]
        public int? RegistrationConfirmationEnabled { get; set; }

        /// <summary>
        /// PayPal Enabled Status
        /// </summary>
        [NumberValidation(Minimum = 0, Maximum = 1, ErrorMessage = "SupportedValueShouldBe")]
        public int? PayPalEnabled { get; set; } 
        
        /// <summary>
        /// PayPal Client ID
        /// </summary>
        [StringLength(100, ErrorMessageResourceName = "PayPalClientIdLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? PayPalClientId {
            get => _payPalClientId;
            set => _payPalClientId = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty));            
        }

        /// <summary>
        /// PayPal Client Secret
        /// </summary>
        [StringLength(100, ErrorMessageResourceName = "PayPalClientSecretLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? PayPalClientSecret {
            get => _payPalClientSecret;
            set => _payPalClientSecret = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty));            
        }

        /// <summary>
        /// PayPal Sandbox Enabled Status
        /// </summary>
        [NumberValidation(Minimum = 0, Maximum = 1, ErrorMessage = "SupportedValueShouldBe")]
        public int? PayPalSandboxEnabled { get; set; }     
        
        /// <summary>
        /// Ip 2 Location Enabled Status
        /// </summary>
        [NumberValidation(Minimum = 0, Maximum = 1, ErrorMessage = "SupportedValueShouldBe")]
        public int? Ip2LocationEnabled { get; set; } 

        /// <summary>
        /// Ip 2 Location Key
        /// </summary>
        [StringLength(200, ErrorMessageResourceName = "Ip2LocationLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? Ip2LocationKey {
            get => _ip2LocationKey;
            set => _ip2LocationKey = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty));            
        }
        
        /// <summary>
        /// Google Maps Enabled Status
        /// </summary>
        [NumberValidation(Minimum = 0, Maximum = 1, ErrorMessage = "SupportedValueShouldBe")]
        public int? GoogleMapsEnabled { get; set; }  
        
        /// <summary>
        /// Google Maps Key
        /// </summary>
        [StringLength(100, ErrorMessageResourceName = "GoogleMapsKeyLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? GoogleMapsKey {
            get => _googleMapsKey;
            set => _googleMapsKey = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty));            
        }    

        /// <summary>
        /// Google Auth Enabled Status
        /// </summary>
        [NumberValidation(Minimum = 0, Maximum = 1, ErrorMessage = "SupportedValueShouldBe")]
        public int? GoogleAuthEnabled { get; set; }  

        /// <summary>
        /// Google Client ID
        /// </summary>
        [StringLength(200, ErrorMessageResourceName = "GoogleClientIdLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? GoogleClientId {
            get => _googleClientId;
            set => _googleClientId = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty));            
        } 

        /// <summary>
        /// Google Client Secret
        /// </summary>
        [StringLength(200, ErrorMessageResourceName = "GoogleClientSecretLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? GoogleClientSecret {
            get => _googleClientSecret;
            set => _googleClientSecret = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty));            
        } 

        /// <summary>
        /// Google ReCaptcha Enabled Status
        /// </summary>
        [NumberValidation(Minimum = 0, Maximum = 1, ErrorMessage = "SupportedValueShouldBe")]
        public int? ReCAPTCHAEnabled { get; set; } 
        
        /// <summary>
        /// Google ReCaptcha Id Key
        /// </summary>
        [StringLength(200, ErrorMessageResourceName = "ReCaptchaKeyLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? ReCAPTCHAKey {
            get => _reCAPTCHAKey;
            set => _reCAPTCHAKey = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty));            
        } 
        
        /// <summary>
        /// SMTP Enabled Status
        /// </summary>
        [NumberValidation(Minimum = 0, Maximum = 1, ErrorMessage = "SupportedValueShouldBe")]
        public int? SmtpEnabled { get; set; } 

        /// <summary>
        /// SMTP Protocol
        /// </summary>
        [StringLength(8, ErrorMessageResourceName = "SmtpProtocolLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? SmtpProtocol {
            get => _smtpProtocol;
            set => _smtpProtocol = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty));            
        }        
        
        /// <summary>
        /// SMTP Host
        /// </summary>
        [StringLength(250, ErrorMessageResourceName = "SmtpHostLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? SmtpHost {
            get => _smtpHost;
            set => _smtpHost = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty));
        } 
        
        /// <summary>
        /// SMTP Port
        /// </summary>
        [StringLength(4, ErrorMessageResourceName = "SmtpPortLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? SmtpPort {
            get => _smtpPort;
            set => _smtpPort = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty));            
        }

        /// <summary>
        /// SMTP Username
        /// </summary>
        [StringLength(50, ErrorMessageResourceName = "SmtpUsernameLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? SmtpUsername {
            get => _smtpUsername;
            set => _smtpUsername = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty));
        }
        
        /// <summary>
        /// SMTP Password
        /// </summary>
        [StringLength(250, ErrorMessageResourceName = "SmtpPasswordLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? SmtpPassword {
            get => _smtpPassword;
            set => _smtpPassword = HttpUtility.HtmlEncode(JavaScriptEncoder.Default.Encode(value ?? string.Empty));
        }              

        /// <summary>
        /// SMTP Sending
        /// </summary>
        [StringLength(3, ErrorMessageResourceName = "SmtpProtectionLong", ErrorMessageResourceType = typeof(ErrorMessages))]
        public string? SmtpSending { get; set; }                              

    }

}