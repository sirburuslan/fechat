/*
 * @file Language Errors
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-18
 *
 * This file contains the words for errors messages
 */

// Create a list with words for the errors pages
const words = (): object => {

    return {
        error_errors_session_expired: 'Sesiune a expirat',
        error_your_session_expired: 'Sesiunea dvs. a expirat.',
        error_go_home: 'Mergi acasă',
        error_confirmation_button_not_configured: 'Butonul de confirmare nu este configurat corect.',
        errors_only_3_images_allowed: 'Un mesaj poate avea până la 3 imagini atașate.',
        error_an_unknown_error_occurred: 'A apărut o eroare necunoscută'
    };

}

// Export the words
export default words;