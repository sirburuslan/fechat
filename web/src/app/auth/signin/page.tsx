/*
 * @page Sign In
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains the page sign in the auth section
 */

'use client'

// Import some React's hooks
import { useState, useRef, useContext, MouseEventHandler, FormEvent } from 'react';

// Import axios
import axios, { AxiosError, AxiosResponse } from 'axios';

// Import the Link component
import Link from 'next/link';

// Import the Image component
import Image from 'next/image';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the incs
import { getIcon, getWord, getOptions, getToken, updateOptions } from '@/core/inc/incIndex';

// Import the types
import { typeOptions, typeToken, typePostHeader } from '@/core/types/typesIndex';

// Import the options for website and member
import {WebsiteOptionsContext, MemberOptionsContext} from '@/core/contexts/OptionsContext';

// Import the useFormSignIn hook
import useFormSignIn from '@/core/hooks/auth/signin/useFormSignIn';
 
// Create the Page component
const Page = (): React.JSX.Element => {

    // Website options
    let {websiteOptions, setWebsiteOptions} = useContext(WebsiteOptionsContext); 

    // Member options
    let {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);

    // Define the fields values
    let [values, setValues] = useState({
        email: '',
        password: ''
    });

    // Message state
    let message = useRef<HTMLDivElement>(null);

    // Monitor the values
    let {email, password} = useFormSignIn(values);

    // Define the state of the password input
    let [passwordInputType, setPasswordInputType] = useState('password');

    // Check if document is defined
    if ( typeof document !== 'undefined' ) {

        // Update the page title dynamically
        document.title = getWord('auth', 'auth_sign_in');

    };

    // Get all options
    let getOptionsAll = async (): Promise<void> => {

        // Request the options
        let optionsList: {success: boolean, options?: typeOptions} = await getOptions();

        // Update memberOptions
        updateOptions(optionsList, setWebsiteOptions, setMemberOptions);

    }

    // Detect when the password input type should be changed
    let handleChangePasswordInput: MouseEventHandler<HTMLButtonElement> = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>): void => {

        // Get target
        let target = e.target as Element;

        // Check if the password input has the text type
        if (passwordInputType === 'text') {

            // Set type as password
            setPasswordInputType('password');

            // Remove class fc-auth-main-form-show-password-active-btn
            target.classList.remove('fc-auth-main-form-show-password-active-btn');

        } else {

            // Set type as text
            setPasswordInputType('text');

            // Add class fc-auth-main-form-show-password-active-btn
            target.classList.add('fc-auth-main-form-show-password-active-btn');

        }        

    };
 
    // Handle form submit
    let submitAuth = async (e: FormEvent<HTMLFormElement>): Promise<void> => {
        e.preventDefault();

        // Reset alert
        message.current!.innerHTML = '';

        // Get target
        let target = e.target as Element;

        // Add the fc-auth-main-form-submit-active-btn class
        target.getElementsByClassName('fc-auth-main-form-submit-btn')[0].classList.add('fc-auth-main-form-submit-active-btn');

        try {

            // Generate a new csrf token
            let csrfToken: typeToken = await getToken();

            // Check if csrf token is missing
            if ( !csrfToken.success ) {

                // Throw error message
                throw new Error(getWord('errors', 'error_csrf_token_not_generated'));

            }

            // Set the headers
            let headers: typePostHeader = {
                headers: {
                    CsrfToken: csrfToken.token
                }
            };

            // Submit the request
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/auth/signin', {
                Email: values.email,
                Password: values.password
            }, headers)
            .then((response: AxiosResponse): void => {

                // Get data
                let data: {success?: boolean, message?: string, Email?: string, Password?: string} = response.data;

                // Verify if the user has been logged in successfully
                if ( data.success ) {

                    // Save the jwt token
                    SecureStorage.setItem('fc_jwt', response.data.member.token);

                    // Save the member role
                    SecureStorage.setItem('fc_role', response.data.member.role);

                    // Reload the options
                    getOptionsAll();

                    // Create success message
                    let messageAlert: string = `<div class="flex items-center px-4 py-3 mb-4 fc-auth-main-form-alert-success" role="alert">
                        ${ getIcon('Iconinformation', {className: 'fc-auth-main-form-alert-success-icon'}) }
                        <p>${data.message}</p>
                    </div>`;

                    // Change alert
                    message.current!.innerHTML = messageAlert;

                    // Set a pause
                    setTimeout((): void => {

                        // Check if member is administrator
                        if ( response.data.member.role === 0 ) {

                            // Redirect the administrator to the dashboard page
                            document.location.href = process.env.NEXT_PUBLIC_SITE_URL + 'admin/dashboard';

                        } else {

                            // Redirect the user to the dashboard page
                            document.location.href = process.env.NEXT_PUBLIC_SITE_URL + 'user/dashboard';

                        }

                    }, 2000);

                } else if ( (!data.success && data.message) || data.Email || data.Password ) {

                    // Set message
                    let messageError: string | undefined = data.message || data.Email || data.Password;

                    // Verify if message is not undefined
                    if ( typeof messageError !== 'undefined' ) {

                        // Throw error message
                        throw new Error(messageError);

                    }

                }

                // Remove the fc-auth-main-form-submit-active-btn class
                target.getElementsByClassName('fc-auth-main-form-submit-btn')[0].classList.remove('fc-auth-main-form-submit-active-btn');

            })
            .catch((error: AxiosError): void => {

                // Throw error message
                throw new Error(error.message);

            });

        } catch (error: unknown) {

            // Check if error is known
            if ( error instanceof Error ) {

                // Create error message
                let messageAlert: string = `<div class="flex items-center px-4 py-3 fc-auth-main-form-alert-error" role="alert">
                    ${ getIcon('Iconexclamation', {className: 'fc-auth-main-form-alert-error-icon'}) }
                    <p>${error.message}</p>
                </div>`;

                // Change alert
                message.current!.innerHTML = messageAlert;   

            } else {

                // Display in the console the error
                console.log(error);

            }

            // Remove the fc-auth-main-form-submit-active-btn class
            target.getElementsByClassName('fc-auth-main-form-submit-btn')[0].classList.remove('fc-auth-main-form-submit-active-btn');

        }
        
    };

    return (
        <form className="px-8 pt-6 pb-6 mb-4 fc-auth-main-form" onSubmit={submitAuth}>
            <div className="mt-4 mb-14">
                <h3 className="text-center">{ (websiteOptions.WebsiteName !== '')?getWord('auth', 'auth_sign_in_to').replace('[website]', websiteOptions.WebsiteName):getWord('auth', 'auth_sign_in_to').replace('[website]', getWord('default', 'default_website_name')) }</h3>
            </div>
            {((websiteOptions.GoogleAuthEnabled !== "") && (parseInt(websiteOptions.GoogleAuthEnabled) > 0))?(
                <>
                    <div>
                        <Link href="/auth/google/access" className="fc-auth-main-form-continue-google-link">
                            <Image src="/assets/img/google.png" width={18} height={18} alt="Member Access" />
                            { getWord('auth', 'auth_continue_with_google') }
                        </Link>
                    </div>            
                    <div className="fc-auth-main-form-separator" data-text={ getWord('auth', 'auth_or') }></div>
                </>
            ):''}
            <div className="mb-6 relative">
                <input type="text" value={values.email} placeholder=" " autoComplete="current-email" id="fc-auth-main-form-email" className="block px-2.5 pb-2.5 pt-4 w-full text-sm text-gray-900 bg-transparent rounded-lg border-1 border-gray-300 appearance-none dark:text-white dark:border-gray-600 dark:focus:border-blue-500 focus:outline-none focus:ring-0 focus:border-blue-600 peer fc-auth-main-form-input" onChange={(e: React.ChangeEvent<HTMLInputElement>): void => setValues({ ...values, email: e.target.value })} required />
                <label htmlFor="fc-auth-main-form-email" className="absolute text-sm text-gray-500 dark:text-gray-400 duration-300 transform -translate-y-4 scale-75 top-2 z-10 origin-[0] bg-white dark:bg-gray-900 px-2 peer-focus:px-2 peer-focus:text-blue-600 peer-focus:dark:text-blue-500 peer-placeholder-shown:scale-100 peer-placeholder-shown:-translate-y-1/2 peer-placeholder-shown:top-1/2 peer-focus:top-2 peer-focus:scale-75 peer-focus:-translate-y-4 left-1">
                    { getWord('auth', 'auth_enter_your_email') }
                </label>
                <div className={`fc-auth-main-form-input-error-message ${email?'fc-auth-main-form-input-error-message-show':''}`}>{email?email:''}</div>
            </div>
            <div className="mb-4 relative">
                <input type={passwordInputType} value={values.password} placeholder=" " autoComplete="current-password" id="fc-auth-main-form-password" className="block px-2.5 pb-2.5 pt-4 w-full text-sm text-gray-900 bg-transparent rounded-lg border-1 border-gray-300 appearance-none dark:text-white dark:border-gray-600 dark:focus:border-blue-500 focus:outline-none focus:ring-0 focus:border-blue-600 peer fc-auth-main-form-input" onChange={(e: React.ChangeEvent<HTMLInputElement>): void => setValues({ ...values, password: e.target.value })} required />
                <label htmlFor="fc-auth-main-form-password" className="absolute text-sm text-gray-500 dark:text-gray-400 duration-300 transform -translate-y-4 scale-75 top-2 z-10 origin-[0] bg-white dark:bg-gray-900 px-2 peer-focus:px-2 peer-focus:text-blue-600 peer-focus:dark:text-blue-500 peer-placeholder-shown:scale-100 peer-placeholder-shown:-translate-y-1/2 peer-placeholder-shown:top-1/2 peer-focus:top-2 peer-focus:scale-75 peer-focus:-translate-y-4 left-1">
                    { getWord('auth', 'auth_enter_your_password') }
                </label>
                <button type="button" className="fc-auth-main-form-show-password-btn" onClick={handleChangePasswordInput}>
                    { getIcon('IconEyeSlash', {className: 'fc-auth-main-form-eye-hide-icon'}) }
                    { getIcon('IconEye', {className: 'fc-auth-main-form-eye-icon'}) }
                </button>
                <div className={`fc-auth-main-form-input-error-message ${password?'fc-auth-main-form-input-error-message-show':''}`}>{password?password:''}</div>
            </div>
            <div className="mb-6">
                <Link href="/auth/reset" className="inline-block align-baseline font-bold text-sm text-blue-500 hover:text-blue-800 fc-auth-main-form-reset-link">
                    { getWord('auth', 'auth_forgot_password') }
                </Link>
            </div>            
            <div className="mt-4 mb-3">
                <button type="submit" className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 focus:outline-none focus:shadow-outline fc-auth-main-form-submit-btn">
                    { getWord('auth', 'auth_sign_in') }
                    { getIcon('IconArrowRight', {className: 'fc-auth-main-form-submit-icon'}) }
                    { getIcon('IconArrowPath', {className: 'fc-rotate-animation fc-auth-main-form-submitting-icon'}) }
                </button>
            </div>
            <div className="fc-auth-main-form-alerts" ref={message} />
            {(websiteOptions.RegistrationEnabled === '1')?(
                <div className="fc-auth-additional-link">
                    <p>
                        { getWord('auth', 'auth_dont_have_account') }
                        <Link href="/auth/registration" className="inline-block fc-auth-main-form-reset-link">{ getWord('auth', 'auth_register_an_account') }</Link>                  
                    </p>
                </div>
            ):''}
        </form>
    );
    
} 

// Export the Page component
export default Page;