/*
 * @class Error Messages
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-01-26
 *
 * This class is used to handle the errors messages
 */

// Namespace for General utilities
namespace FeChat.Utils.General {

    /// <summary>
    /// Errors Messages Manager
    /// </summary>
    public class ErrorMessages {

        /// <summary>
        /// Error message for wrong password length
        /// </summary>
        public static string PasswordLength => new Strings().Get("PasswordLength");

        /// <summary>
        /// Error message if password is missing
        /// </summary>
        public static string PasswordRequired => new Strings().Get("PasswordRequired");

        /// <summary>
        /// Error message if password contains empty spaces
        /// </summary>
        public static string PasswordNoSpaces => new Strings().Get("PasswordNoSpaces");

        /// <summary>
        /// Error message if email address is too long
        /// </summary>
        public static string EmailLong => new Strings().Get("EmailLong");        

        /// <summary>
        /// Error message if email address is missing
        /// </summary>
        public static string EmailRequired => new Strings().Get("EmailRequired");

        /// <summary>
        /// Error message if the email is not valid
        /// </summary>
        public static string EmailNotValid => new Strings().Get("EmailNotValid");

        /// <summary>
        /// Error message if the number is not valid
        /// </summary>
        public static string IncorrectNumber => new Strings().Get("IncorrectNumber"); 
        
        /// <summary>
        /// Error message if the website name is too long
        /// </summary>
        public static string WebsiteNameLong => new Strings().Get("WebsiteNameLong");

        /// <summary>
        /// Error message if the website domain is too long
        /// </summary>
        public static string DomainLong => new Strings().Get("DomainLong");        

        /// <summary>
        /// Error message if the url is not valid
        /// </summary>
        public static string UrlNotValid => new Strings().Get("UrlNotValid");        

        /// <summary>
        /// Error message if the url is too long
        /// </summary>
        public static string UrlLong => new Strings().Get("UrlLong");   

        /// <summary>
        /// Error message if the analytics code is too long
        /// </summary>
        public static string AnalyticsCodeLong => new Strings().Get("AnalyticsCodeLong");
        
        /// <summary>
        /// Error message if the smtp host is too long
        /// </summary>
        public static string SmtpHostLong => new Strings().Get("SmtpHostLong");   

        /// <summary>
        /// Error message if the smtp port is too long
        /// </summary>
        public static string SmtpPortLong => new Strings().Get("SmtpPortLong");        
        
        /// <summary>
        /// Error message if the smtp password is too long
        /// </summary>
        public static string SmtpPasswordLong => new Strings().Get("SmtpPasswordLong");     
        
        /// <summary>
        /// Error message if the smtp protection is wrong
        /// </summary>
        public static string SmtpProtectionLong => new Strings().Get("SmtpProtectionLong");    
        
        /// <summary>
        /// Error message if the paypal client id is too long
        /// </summary>
        public static string PayPalClientIdLong => new Strings().Get("PayPalClientIdLong");  
        
        /// <summary>
        /// Error message if the paypal client secret is too long
        /// </summary>
        public static string PayPalClientSecretLong => new Strings().Get("PayPalClientSecretLong");   
        
        /// <summary>
        /// Error message if the first name is too long
        /// </summary>
        public static string FirstNameLong => new Strings().Get("FirstNameLong"); 
        
        /// <summary>
        /// Error message if the last name is too long
        /// </summary>
        public static string LastNameLong => new Strings().Get("LastNameLong"); 

        /// <summary>
        /// Error message if the phone is too long
        /// </summary>
        public static string MemberPhoneLong => new Strings().Get("MemberPhoneLong");         
        
        /// <summary>
        /// Error message if the language word is too long
        /// </summary>
        public static string MemberLanguageLong => new Strings().Get("MemberLanguageLong");   
        
        /// <summary>
        /// Error message for incorrect page number
        /// </summary>
        public static string IncorrectPage => new Strings().Get("IncorrectPage");  
        
        /// <summary>
        /// Error message for incorrect role value
        /// </summary>
        public static string IncorrectRole => new Strings().Get("IncorrectRole");  
        
        /// <summary>
        /// Plan name is too long
        /// </summary>
        public static string PlanNameLong => new Strings().Get("PlanNameLong");      
        
        /// <summary>
        /// Plan price is too long
        /// </summary>
        public static string PlanPriceLong => new Strings().Get("PlanPriceLong");   

        /// <summary>
        /// Plan currency is too long
        /// </summary>
        public static string PlanCurrencyLong => new Strings().Get("PlanCurrencyLong");  
        
        /// <summary>
        /// Reset code is too long
        /// </summary>
        public static string ResetCodeLong => new Strings().Get("ResetCodeLong");

        /// <summary>
        /// Message is too long
        /// </summary>
        public static string MessageLong => new Strings().Get("MessageLong"); 
        
        /// <summary>
        /// Thread secret is too long
        /// </summary>
        public static string ThreadSecretLong => new Strings().Get("ThreadSecretLong"); 

        /// <summary>
        /// Chat header is too long
        /// </summary>
        public static string ChatHeaderLong => new Strings().Get("ChatHeaderLong");         
        
        /// <summary>
        /// Guest name is too long
        /// </summary>
        public static string GuestNameLong => new Strings().Get("GuestNameLong"); 
        
        /// <summary>
        /// Payment source is not valid
        /// </summary>
        public static string PaymentSourceNotValid => new Strings().Get("PaymentSourceNotValid");  
        
        /// <summary>
        /// Order id is not valid
        /// </summary>
        public static string OrderIdNotValid => new Strings().Get("OrderIdNotValid");   

        /// <summary>
        /// Subscription id is not valid
        /// </summary>
        public static string SubscriptionIdNotValid => new Strings().Get("SubscriptionIdNotValid");  

        /// <summary>
        /// ReCaptcha Key is too long
        /// </summary>
        public static string ReCaptchaKeyLong => new Strings().Get("ReCaptchaKeyLong");

        /// <summary>
        /// Google Client Id is too long
        /// </summary>
        public static string GoogleClientIdLong => new Strings().Get("GoogleClientIdLong");          

        /// <summary>
        /// Google Secret Key is too long
        /// </summary>
        public static string GoogleClientSecretLong => new Strings().Get("GoogleClientSecretLong");
        
        /// <summary>
        /// Google Maps Key is too long
        /// </summary>
        public static string GoogleMapsKeyLong => new Strings().Get("GoogleMapsKeyLong");
        
        /// <summary>
        /// Ip2Location Key is too long
        /// </summary>
        public static string Ip2LocationLong => new Strings().Get("Ip2LocationLong"); 
        
        /// <summary>
        /// Code is too long
        /// </summary>
        public static string CodeLong => new Strings().Get("CodeLong");                               

    }

}