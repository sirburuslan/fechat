/*
 * @file Language Auth
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains the words for the auth section
 */

// Create a list with words
const words = (): object => {

    return {
        auth_sign_in: 'Sign In',
        auth_register_an_account: 'Register an account',
        auth_dont_have_account: 'Don\'t have an account?',
        auth_sign_up_to: 'Sign Up to [website]',
        auth_sign_in_to: 'Sign In to [website]',
        auth_continue_with_google: 'Continue with Google',
        auth_continue_with_sso: 'Continue with SSO',
        auth_enter_your_email: 'Enter your email address',
        auth_enter_your_password: 'Enter your password',
        auth_forgot_password: 'Forgot Password?',
        auth_email_not_valid: 'The email address is not valid.',
        auth_or: 'OR',
        auth_registration: 'Registration',
        auth_do_you_have_account: 'Do you have an account?',
        auth_sign_up: 'Sign Up',
        auth_reset_password: 'Reset Password',
        auth_do_you_remember_password: 'Do you remember password?',
        auth_reset: 'Reset',
        auth_change_password: 'Change Password',
        auth_new_password: 'New Password',
        auth_redirecting: 'Redirecting...',
        auth_google_is_not_configured: 'Google is not configured.',
        auth_validating: 'Validating...'
    };

}

// Export the list with words
export default words;