/*
 * @class Mail Sender
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-24
 *
 * This class sends emails
 */

// Namespace for General Utils
namespace FeChat.Utils.General {

    // System Namespaces
    using System.Net;
    using System.Net.Mail; 

    /// <summary>
    /// Mail Sender
    /// </summary>
    public class Sender {

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="optionsList">Website Options</param>
        /// <param name="to">Email Receiver</param>
        /// <param name="subject">Email Subject</param>
        /// <param name="body">Email Body</param>
        /// <returns>Bool true or false</returns>
        public async Task<bool> Send(Dictionary<string, string> optionsList, string to, string subject, string body) {

            try {

                // Get the SMTP Status
                optionsList.TryGetValue("SmtpEnabled", out string? smtpEnabled); 

                // Get the Email Sender
                optionsList.TryGetValue("EmailSender", out string? emailSender);                   
                
                // Get the SMTP Host
                optionsList.TryGetValue("SmtpHost", out string? smtpHost);    
                
                // Get the SMTP Port
                optionsList.TryGetValue("SmtpPort", out string? smtpPort);    
                
                // Get the SMTP Username
                optionsList.TryGetValue("SmtpUsername", out string? smtpUsername);     
                
                // Get the SMTP Username
                optionsList.TryGetValue("SmtpPassword", out string? smtpPassword); 

                // Verify if smtp is enabled
                if ( (smtpEnabled == null) || (smtpEnabled != "1") ) {
                    return false;
                }

                // Verify if email sender exists
                if ( (emailSender == null) || (emailSender == "") ) {
                    return false;
                } 

                // Verify if smtp host exists
                if ( (smtpHost == null) || (smtpHost == "") ) {
                    return false;
                }  
                
                // Verify if smtp port exists
                if ( (smtpPort == null) || (smtpPort == "") ) {
                    return false;
                }  
                
                // Verify if smtp username exists
                if ( (smtpUsername == null) || (smtpUsername == "") ) {
                    return false;
                } 
                
                // Verify if smtp password exists
                if ( (smtpPassword == null) || (smtpPassword == "") ) {
                    return false;
                }                                                              

                // Initialize Simple Mail Transfer Protocol
                using SmtpClient client = new(smtpHost, int.Parse(smtpPort)) {

                    // Set credentials
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),

                    // Support for SSL and TSL
                    EnableSsl = true

                };

                // Create MailMessage object
                MailMessage message = new(emailSender, to, subject, body) {

                    // Add support for html
                    IsBodyHtml = true

                };

                // Send mail
                await client.SendMailAsync(message);

                return true;

            } catch (Exception ex) {

                Console.WriteLine(ex.Message);

                return false;

            }

        }

    }

}