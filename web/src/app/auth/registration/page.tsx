/*
 * @page Registration
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains the page registration in the auth section
 */

'use client';

// Import some React's hooks
import { useContext, useState, useRef, MouseEventHandler, FormEventHandler, FormEvent } from 'react';

// Import axios
import axios, { AxiosError, AxiosResponse } from 'axios';

// Import the Image component
import Image from 'next/image';

// Import the Next Link Component
import Link from 'next/link';

// Import the router function
import { useRouter } from 'next/navigation';

// Import the incs
import { getIcon, getWord } from '@/core/inc/incIndex';

// Import the options for website and member
import {WebsiteOptionsContext} from '@/core/contexts/OptionsContext';

// Import the useFormRegistration hook
import useFormRegistration from '@/core/hooks/auth/registration/useFormRegistration';
 
// Create the Page component
const Page = (): React.JSX.Element => {

    // Get the router
    let router = useRouter()

    // Website options
    let {websiteOptions} = useContext(WebsiteOptionsContext);

    // Verify if registration is disabled
    if ( (websiteOptions.Default === '0') && (websiteOptions.RegistrationEnabled !== '1') ) {

        // Redirect the user to the login page
        router.push(process.env.NEXT_PUBLIC_SITE_URL as string);

    }

    // Default fields values
    let [values, setValues] = useState({
        email: '',
        password: ''
    });

    // Define the state of the password input
    let [passwordInputType, setPasswordInputType] = useState('password');

    // Monitor the values
    let {email, password} = useFormRegistration(values);

    // Message state
    let message = useRef<HTMLDivElement>(null);

    // Check if document is defined
    if ( typeof document !== 'undefined' ) {

        // Update the page title dynamically
        document.title = getWord('auth', 'auth_registration');

    };

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

    // Submit form handler
    let submitForm: FormEventHandler<HTMLFormElement> = async (e: FormEvent<HTMLFormElement>): Promise<void> => {
        e.preventDefault();

        // Reset alert
        message.current!.innerHTML = '';

        // Get the target
        let target = e.target as Element;

        // Add the fc-auth-main-form-submit-active-btn class
        target.getElementsByClassName('fc-auth-main-form-submit-btn')[0].classList.add('fc-auth-main-form-submit-active-btn');

        try {

            // Submit the request with the user details
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/auth/registration', {
                email: values.email,
                password: values.password
            })
            .then((response: AxiosResponse): void => {

                // Get data
                let data: {success?: boolean, message?: string, Email?: string, Password?: string} = response.data;

                // Verify if the user has been logged in successfully
                if ( data.success ) {

                    // Create success message
                    let messageAlert: string = `<div class="flex items-center px-4 py-3 mb-4 fc-auth-main-form-alert-success" role="alert">
                        ${ getIcon('Iconinformation', {className: 'fc-auth-main-form-alert-success-icon'}) }
                        <p>${data.message}</p>
                    </div>`;

                    // Change alert
                    message.current!.innerHTML = messageAlert;

                    // Set a pause
                    setTimeout((): void => {

                        // Redirect the user to the login page
                        router.push(process.env.NEXT_PUBLIC_SITE_URL + 'auth/signin');

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

    } 

    return (
        <form className="px-8 pt-6 pb-6 mb-4 fc-auth-main-form" onSubmit={submitForm}>
            <div className="mt-4 mb-14">
                <h3 className="text-center">{ (websiteOptions.WebsiteName !== '')?getWord('auth', 'auth_sign_up_to').replace('[website]', websiteOptions.WebsiteName):getWord('auth', 'auth_sign_up_to').replace('[website]', getWord('default', 'default_website_name')) }</h3>
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
                <input type="text" value={values.email} placeholder=" " autoComplete="current-email" id="fc-auth-main-form-email" className="block px-2.5 pb-2.5 pt-4 w-full text-sm text-gray-900 bg-transparent rounded-lg border-1 border-gray-300 appearance-none dark:text-white dark:border-gray-600 dark:focus:border-blue-500 focus:outline-none focus:ring-0 focus:border-blue-600 peer fc-auth-main-form-input" onChange={(e): void => setValues({...values, email: e.target.value})} required />
                <label htmlFor="fc-auth-main-form-email" className="absolute text-sm text-gray-500 dark:text-gray-400 duration-300 transform -translate-y-4 scale-75 top-2 z-10 origin-[0] bg-white dark:bg-gray-900 px-2 peer-focus:px-2 peer-focus:text-blue-600 peer-focus:dark:text-blue-500 peer-placeholder-shown:scale-100 peer-placeholder-shown:-translate-y-1/2 peer-placeholder-shown:top-1/2 peer-focus:top-2 peer-focus:scale-75 peer-focus:-translate-y-4 left-1">
                    { getWord('auth', 'auth_enter_your_email') }
                </label>
                <div className={`fc-auth-main-form-input-error-message ${email?'fc-auth-main-form-input-error-message-show':''}`}>{email?email:''}</div>
            </div>
            <div className="mb-6 relative">
                <input type={passwordInputType} value={values.password} placeholder=" " autoComplete="current-password" id="fc-auth-main-form-password" className="block px-2.5 pb-2.5 pt-4 w-full text-sm text-gray-900 bg-transparent rounded-lg border-1 border-gray-300 appearance-none dark:text-white dark:border-gray-600 dark:focus:border-blue-500 focus:outline-none focus:ring-0 focus:border-blue-600 peer fc-auth-main-form-input" onChange={(e): void => setValues({...values, password: e.target.value})} required />
                <label htmlFor="fc-auth-main-form-password" className="absolute text-sm text-gray-500 dark:text-gray-400 duration-300 transform -translate-y-4 scale-75 top-2 z-10 origin-[0] bg-white dark:bg-gray-900 px-2 peer-focus:px-2 peer-focus:text-blue-600 peer-focus:dark:text-blue-500 peer-placeholder-shown:scale-100 peer-placeholder-shown:-translate-y-1/2 peer-placeholder-shown:top-1/2 peer-focus:top-2 peer-focus:scale-75 peer-focus:-translate-y-4 left-1">
                    { getWord('auth', 'auth_enter_your_password') }
                </label>
                <button type="button" className="fc-auth-main-form-show-password-btn" onClick={handleChangePasswordInput}>
                    { getIcon('IconEyeSlash', {className: 'fc-auth-main-form-eye-hide-icon'}) }
                    { getIcon('IconEye', {className: 'fc-auth-main-form-eye-icon'}) }
                </button>
                <div className={`fc-auth-main-form-input-error-message ${password?'fc-auth-main-form-input-error-message-show':''}`}>{password?password:''}</div>
            </div>        
            <div className="mt-4 mb-3">
                <button type="submit" className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 focus:outline-none focus:shadow-outline fc-auth-main-form-submit-btn">
                    { getWord('auth', 'auth_sign_up') }
                    { getIcon('IconArrowRight', {className: 'fc-auth-main-form-submit-icon'}) }
                    { getIcon('IconArrowPath', {className: 'fc-rotate-animation fc-auth-main-form-submitting-icon'}) }
                </button>
            </div>
            <div className="fc-auth-main-form-alerts" ref={message} />
            <div className="fc-auth-additional-link">
                <p>
                    { getWord('auth', 'auth_do_you_have_account') }
                    <Link href="/auth/signin" className="inline-block fc-auth-main-form-reset-link">{ getWord('auth', 'auth_sign_in') }</Link>                  
                </p>
            </div>
        </form>
    );
    
} 

// Export the Page component
export default Page;