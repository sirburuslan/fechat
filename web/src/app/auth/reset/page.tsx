/*
 * @page Reset
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains the page reset in the auth section
 */

'use client'

// Import some React's hooks
import { useState, useContext, useRef } from 'react';

// Import axios
import axios, { AxiosError, AxiosResponse } from 'axios';

// Import the Link component
import Link from 'next/link';

// Import the incs
import { getIcon, getWord, getToken } from '@/core/inc/incIndex';

// Import the types
import { typeToken, typePostHeader } from '@/core/types/typesIndex';

// Import the options for website and member
import {WebsiteOptionsContext} from '@/core/contexts/OptionsContext';

// Import the useFormReset hook
import useFormReset from '@/core/hooks/auth/reset/useFormReset';
 
// Create the Page component
const Page = (): React.JSX.Element => {

    // Website options
    let {websiteOptions} = useContext(WebsiteOptionsContext);

    // Define the fields values
    let [values, setValues] = useState({
        email: ''
    });

    // Message state
    let message = useRef<HTMLDivElement>(null);

    // Monitor the values
    let {email} = useFormReset(values);

    // Check if document is defined
    if ( typeof document !== 'undefined' ) {

        // Update the page title dynamically
        document.title = getWord('auth', 'auth_reset_password');

    };
 
    // Handle form submit for password reset
    let submitReset = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
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
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/auth/reset', {
                email: values.email
            }, headers)
            .then((response: AxiosResponse): void => {

                // Get data
                let data: {success?: boolean, message?: string, Email?: string} = response.data;

                // Verify if the user has been logged in successfully
                if ( data.success ) {

                    // Create success message
                    let messageAlert: string = `<div class="flex items-center px-4 py-3 mb-4 fc-auth-main-form-alert-success" role="alert">
                        ${ getIcon('Iconinformation', {className: 'fc-auth-main-form-alert-success-icon'}) }
                        <p>${data.message}</p>
                    </div>`;

                    // Change alert
                    message.current!.innerHTML = messageAlert;

                    // Empty the email field
                    setValues({
                        email: ''
                    });

                } else if ( (!data.success && data.message) || data.Email ) {

                    // Set message
                    let messageError: string | undefined = data.message || data.Email;

                    // Verify if message is not undefined
                    if ( typeof messageError !== 'undefined' ) {

                        // Throw the error message
                        throw new Error(messageError);

                    }

                }

                // Remove the fc-auth-main-form-submit-active-btn class
                target.getElementsByClassName('fc-auth-main-form-submit-btn')[0].classList.remove('fc-auth-main-form-submit-active-btn');

            })
            .catch((error: AxiosError): void => {
                
                // Throw the error message
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
        <form className="px-8 pt-6 pb-6 mb-4 fc-auth-main-form" onSubmit={submitReset}>
            <div className="mt-4 mb-14">
                <h3 className="text-center">{ (websiteOptions.WebsiteName !== '')?getWord('auth', 'auth_sign_in_to').replace('[website]', websiteOptions.WebsiteName):getWord('auth', 'auth_sign_in_to').replace('[website]', getWord('default', 'default_website_name')) }</h3>
            </div>
            <div className="mb-6 relative">
                <input type="text" value={values.email} placeholder=" " autoComplete="current-email" id="fc-auth-main-form-email" className="block px-2.5 pb-2.5 pt-4 w-full text-sm text-gray-900 bg-transparent rounded-lg border-1 border-gray-300 appearance-none dark:text-white dark:border-gray-600 dark:focus:border-blue-500 focus:outline-none focus:ring-0 focus:border-blue-600 peer fc-auth-main-form-input" onChange={(e: React.ChangeEvent<HTMLInputElement>): void => setValues({ ...values, email: e.target.value })} required />
                <label htmlFor="fc-auth-main-form-email" className="absolute text-sm text-gray-500 dark:text-gray-400 duration-300 transform -translate-y-4 scale-75 top-2 z-10 origin-[0] bg-white dark:bg-gray-900 px-2 peer-focus:px-2 peer-focus:text-blue-600 peer-focus:dark:text-blue-500 peer-placeholder-shown:scale-100 peer-placeholder-shown:-translate-y-1/2 peer-placeholder-shown:top-1/2 peer-focus:top-2 peer-focus:scale-75 peer-focus:-translate-y-4 left-1">
                    { getWord('auth', 'auth_enter_your_email') }
                </label>
                <div className={`fc-auth-main-form-input-error-message ${email?'fc-auth-main-form-input-error-message-show':''}`}>{email?email:''}</div>
            </div>       
            <div className="mt-4 mb-3">
                <button type="submit" className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 focus:outline-none focus:shadow-outline fc-auth-main-form-submit-btn">
                    { getWord('auth', 'auth_reset') }
                    { getIcon('IconArrowRight', {className: 'fc-auth-main-form-submit-icon'}) }
                    { getIcon('IconArrowPath', {className: 'fc-rotate-animation fc-auth-main-form-submitting-icon'}) }
                </button>
            </div>
            <div className="fc-auth-main-form-alerts" ref={message} />
            <div className="fc-auth-additional-link">
                <p>
                    { getWord('auth', 'auth_do_you_remember_password') }
                    <Link href="/auth/signin" className="inline-block fc-auth-main-form-reset-link">{ getWord('auth', 'auth_sign_in') }</Link>                  
                </p>
            </div>
        </form>
    );
    
} 

// Export the Page component
export default Page;