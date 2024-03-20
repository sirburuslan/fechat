/*
 * @file Language Auth
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-18
 *
 * This file contains the words for the auth section
 */

// Create a list with words
const words = (): object => {

    return {
        auth_sign_in: 'Conectează-te',
        auth_register_an_account: 'Înregistrați un cont',
        auth_dont_have_account: 'Nu aveți un cont?',
        auth_sign_up_to: 'Înscrieți-vă la [site web]',
        auth_sign_in_to: 'Conectați-vă la [site web]',
        auth_continue_with_google: 'Continuați cu Google',
        auth_continue_with_sso: 'Continuați cu SSO',
        auth_enter_your_email: 'Introduceți adresa dvs. de e-mail',
        auth_enter_your_password: 'Introduceți parola',
        auth_forgot_password: 'Ați uitat parola?',
        auth_email_not_valid: 'Adresa de e-mail nu este validă.',
        auth_or: 'SAU',
        auth_registration: 'Înregistrare',
        auth_do_you_have_account: 'Ai un cont?',
        auth_sign_up: 'Înscrieți-vă',
        auth_reset_password: 'Resetează parola',
        auth_do_you_remember_password: 'Îți amintești parola?',
        auth_reset: 'Resetare',
        auth_change_password: 'Schimbați parola',
        auth_new_password: 'Parolă nouă',
        auth_redirecting: 'Redirecționează...',
        auth_google_is_not_configured: 'Google nu este configurat.',
        auth_validating: 'Validare...',
    };

}

// Export the list with words
export default words;