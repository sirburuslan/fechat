/*
 * @page Change Password
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains the page change password in the auth section
 */

'use client'

// Import some React's hooks
import { useState, useEffect, useRef, MouseEventHandler } from 'react';

// Import axios
import axios, { AxiosError, AxiosResponse } from 'axios';

// Import the Link component
import Link from 'next/link';

// Import the router function
import { useRouter } from 'next/navigation';

// Import the incs
import { getIcon, getWord, getToken } from '@/core/inc/incIndex';

// Import the types
import { typeToken, typePostHeader } from '@/core/types/typesIndex';

// Import the useFormChange hook
import useFormChange from '@/core/hooks/auth/reset/useFormChange';
 
// Create the Change Password page
const Page = ({params}: {params: { slug: string }}): React.JSX.Element => {

    // Get the router
    let router = useRouter()

    // Define the fields values
    let [values, setValues] = useState({
        password: '',
        repeatPassword: ''
    });

    // Define the state of the password input
    let [passwordInputType, setPasswordInputType] = useState('password');

    // Define the state of the repeat password input
    let [repeatPasswordInputType, setRepeatPasswordInputType] = useState('password');    

    // Error for code
    let [error, setCodeError] = useState('');    

    // Message state
    let message = useRef<HTMLDivElement>(null);

    // Monitor the values
    let {password, repeatPassword} = useFormChange(values);

    // Check if document is defined
    if ( typeof document !== 'undefined' ) {

        // Update the page title dynamically
        document.title = getWord('auth', 'auth_change_password');

    };

    // Validate a reset code
    let validateResetCode = async (): Promise<void> => {

        try {

            // Submit the request to validate the reset code
            await axios.get(process.env.NEXT_PUBLIC_API_URL + 'api/v1/auth/reset/validate/' + params.slug)

            // Process the response
            .then((response: AxiosResponse): void => {

                // Get data
                let data: {success?: boolean, message?: string, Email?: string, Password?: string} = response.data;

                // Verify if the user has been logged in successfully
                if ( data.success ) {

                    // Remove the error message
                    setCodeError('');

                } else {

                    // Throw error message
                    throw new Error(data.message);

                }

            })

            // Process the error
            .catch((error: AxiosError): void => {

                // Throw error message
                throw new Error(error.message);

            });

        } catch (error: unknown) {

            // Check if error is known
            if ( error instanceof Error ) {

                // Create error message
                let messageAlert: string = `<div class="flex items-center px-4 py-3 fc-auth-main-form-alert-error fc-auth-main-form-alert-error-static" role="alert">
                    ${ getIcon('Iconexclamation', {className: 'fc-auth-main-form-alert-error-icon'}) }
                    <p>${error.message}</p>
                </div>`;
                
                // Display the error message
                setCodeError(messageAlert);

            } else {

                // Display in the console the error
                console.log(error);

            }

        }

    };

    // Handle form submit for password change
    let submitPasswordChange = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
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
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/auth/reset/change-password', {
                ResetCode: params.slug,
                Password: values.password,
                RepeatPassword: values.repeatPassword
            }, headers)
            .then((response: AxiosResponse): void => {

                // Get data
                let data: {success?: boolean, message?: string, Password?: string, RepeatPassword?: string} = response.data;

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
                        password: '',
                        repeatPassword: ''
                    });

                    // Set a pause
                    setTimeout((): void => {

                        // Redirect the user to the login page
                        router.push(process.env.NEXT_PUBLIC_SITE_URL + 'auth/signin');

                    }, 2000);

                } else if ( (!data.success && data.message) || data.Password || data.RepeatPassword ) {

                    // Set message
                    let messageError: string | undefined = data.message || data.Password || data.RepeatPassword;

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

    // Run code for client
    useEffect((): void => {

        // Check if the reset code is valid
        validateResetCode();

    }, []);

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

    // Detect when the password input type should be changed
    let handleChangeRepeatPasswordInput: MouseEventHandler<HTMLButtonElement> = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>): void => {

        // Get target
        let target = e.target as Element;

        // Check if the password input has the text type
        if (repeatPasswordInputType === 'text') {

            // Set type as password
            setRepeatPasswordInputType('password');

            // Remove class fc-auth-main-form-show-password-active-btn
            target.classList.remove('fc-auth-main-form-show-password-active-btn');

        } else {

            // Set type as text
            setRepeatPasswordInputType('text');

            // Add class fc-auth-main-form-show-password-active-btn
            target.classList.add('fc-auth-main-form-show-password-active-btn');

        }        

    };

    return (
        <>
        {(error)?(
            <div className="px-8 pt-6 pb-6 mb-4 fc-auth-main-form">
                <div className="fc-auth-main-form-alerts" dangerouslySetInnerHTML={{__html: error}} />  
                <div className="fc-auth-additional-link">
                    <p>
                        { getWord('auth', 'auth_do_you_remember_password') }
                        <Link href="/auth/signin" className="inline-block fc-auth-main-form-reset-link">{ getWord('auth', 'auth_sign_in') }</Link>                  
                    </p>
                </div>
            </div>          
        ):(
            <form className="px-8 pt-6 pb-6 mb-4 fc-auth-main-form" onSubmit={submitPasswordChange}>
                <div className="mt-4 mb-14">
                    <h3 className="text-center">{ getWord('auth', 'auth_new_password') }</h3>
                </div>
                <div className="mb-4 relative">
                    <input type={passwordInputType} value={values.password} placeholder=" " autoComplete="new-password" id="fc-auth-main-form-password" className="block px-2.5 pb-2.5 pt-4 w-full text-sm text-gray-900 bg-transparent rounded-lg border-1 border-gray-300 appearance-none dark:text-white dark:border-gray-600 dark:focus:border-blue-500 focus:outline-none focus:ring-0 focus:border-blue-600 peer fc-auth-main-form-input" onChange={(e: React.ChangeEvent<HTMLInputElement>): void => setValues({ ...values, password: e.target.value })} required />
                    <label htmlFor="fc-auth-main-form-password" className="absolute text-sm text-gray-500 dark:text-gray-400 duration-300 transform -translate-y-4 scale-75 top-2 z-10 origin-[0] bg-white dark:bg-gray-900 px-2 peer-focus:px-2 peer-focus:text-blue-600 peer-focus:dark:text-blue-500 peer-placeholder-shown:scale-100 peer-placeholder-shown:-translate-y-1/2 peer-placeholder-shown:top-1/2 peer-focus:top-2 peer-focus:scale-75 peer-focus:-translate-y-4 left-1">
                        { getWord('default', 'default_new_password') }
                    </label>
                    <button type="button" className="fc-auth-main-form-show-password-btn" onClick={handleChangePasswordInput}>
                        { getIcon('IconEyeSlash', {className: 'fc-auth-main-form-eye-hide-icon'}) }
                        { getIcon('IconEye', {className: 'fc-auth-main-form-eye-icon'}) }
                    </button>
                    <div className={`fc-auth-main-form-input-error-message ${password?'fc-auth-main-form-input-error-message-show':''}`}>{password?password:''}</div>
                </div>
                <div className="mb-4 relative">
                    <input type={repeatPasswordInputType} value={values.repeatPassword} placeholder=" " autoComplete="repeat-password" id="fc-auth-main-form-repeat-password" className="block px-2.5 pb-2.5 pt-4 w-full text-sm text-gray-900 bg-transparent rounded-lg border-1 border-gray-300 appearance-none dark:text-white dark:border-gray-600 dark:focus:border-blue-500 focus:outline-none focus:ring-0 focus:border-blue-600 peer fc-auth-main-form-input" onChange={(e: React.ChangeEvent<HTMLInputElement>): void => setValues({ ...values, repeatPassword: e.target.value })} required />
                    <label htmlFor="fc-auth-main-form-repeat-password" className="absolute text-sm text-gray-500 dark:text-gray-400 duration-300 transform -translate-y-4 scale-75 top-2 z-10 origin-[0] bg-white dark:bg-gray-900 px-2 peer-focus:px-2 peer-focus:text-blue-600 peer-focus:dark:text-blue-500 peer-placeholder-shown:scale-100 peer-placeholder-shown:-translate-y-1/2 peer-placeholder-shown:top-1/2 peer-focus:top-2 peer-focus:scale-75 peer-focus:-translate-y-4 left-1">
                        { getWord('default', 'default_enter_password_again') }
                    </label>
                    <button type="button" className="fc-auth-main-form-show-password-btn" onClick={handleChangeRepeatPasswordInput}>
                        { getIcon('IconEyeSlash', {className: 'fc-auth-main-form-eye-hide-icon'}) }
                        { getIcon('IconEye', {className: 'fc-auth-main-form-eye-icon'}) }
                    </button>
                    <div className={`fc-auth-main-form-input-error-message ${repeatPassword?'fc-auth-main-form-input-error-message-show':''}`}>{repeatPassword?repeatPassword:''}</div>
                </div>       
                <div className="mt-4 mb-3">
                    <button type="submit" className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 focus:outline-none focus:shadow-outline fc-auth-main-form-submit-btn">
                        { getWord('default', 'default_save') }
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
        )}
        </>
    );
    
} 

// Export the Change Password page
export default Page;