/*
 * @file Language Errors
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains the words for errors messages
 */

// Create a list with words for the errors pages
const words = (): object => {

    return {
        error_errors_session_expired: 'Session Expired',
        error_your_session_expired: 'Your session has been expired.',
        error_go_home: 'Go Home',
        error_csrf_token_not_generated: 'The CSRF token was not generated successfully.',
        error_confirmation_button_not_configured: 'Confirmation button not configured correctly.',
        errors_only_3_images_allowed: 'A message could have up to 3 images attached.',
        error_an_unknown_error_occurred: 'An unknown error occurred.'
    };

}

// Export the words
export default words;