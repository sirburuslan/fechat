/*
 * @component Profile Notifications
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-09
 *
 * This file contains the profile notifications in the user panel
 */

'use client'

// Import the React hooks
import { useContext, Dispatch, SetStateAction } from 'react';

// Import axios module
import axios, { AxiosError, AxiosResponse } from 'axios';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the incs
import { getIcon, getWord, getToken, getField, getOptions, updateOptions, showNotification } from '@/core/inc/incIndex';

// Import types
import { typeToken, typePostHeader, typeOptions } from '@/core/types/typesIndex';

// Import the options for website and member
import { WebsiteOptionsContext, MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Create the Profile Notifications component
const ProfileNotifications: React.FC<{fields: {[key: string]: string | number}, setFields: Dispatch<SetStateAction<any>>}> = ({fields, setFields}): React.JSX.Element => {

    // Website options
    let {websiteOptions, setWebsiteOptions} = useContext(WebsiteOptionsContext); 

    // Member options
    let {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);   

    /**
     * Update the member's notifications options
     * 
     * @param FormEvent e 
     */
    let notificationsUpdate = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
        e.preventDefault();

        // Get the target
        let target: Element = e.currentTarget;

        // Add active class
        target.classList.add('fc-option-active-btn');

        try {

            // Generate a new csrf token
            let csrfToken: typeToken = await getToken();

            // Check if csrf token is missing
            if ( !csrfToken.success ) {

                // Show error notification
                throw new Error(getWord('errors', 'error_csrf_token_not_generated', memberOptions['Language']));

            }

            // Set the bearer token
            let token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');

            // Prepare the headers
            let headers: typePostHeader = {
                headers: {
                    Authorization: `Bearer ${token}`,
                    CsrfToken: csrfToken.token
                }
            };

            // Update the fields
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/notifications', {
                MemberId: fields.MemberId,
                NotificationsEnabled: fields.NotificationsEnabled,
                NotificationsEmail: fields.NotificationsEmail
            }, headers)

            // Process the response
            .then(async (response: AxiosResponse) => {

                // Verify if the response is successfully
                if ( response.data.success ) {
                    
                    // Show success notification
                    showNotification('success', response.data.message);

                    // Request the options
                    let optionsList: {success: boolean, options?: typeOptions} = await getOptions();

                    // Update memberOptions
                    updateOptions(optionsList, setWebsiteOptions, setMemberOptions);

                } else if ( typeof response.data.message !== 'undefined' ) {

                    // Run error notification
                    throw new Error(response.data.message);

                } else {

                    // Keys container
                    let keys: string[] = Object.keys(response.data);

                    // Run error notification
                    throw new Error(response.data[keys[0]][0]);                    

                }

            })

            // Catch the error message
            .catch((error: AxiosError): void => {

                // Check if error message exists
                if ( typeof error.message !== 'undefined' ) {

                    // Show error notification
                    throw new Error(error.message);                    

                }
                

            })

            // Run a code finally
            .then ((): void => {

                // Remove active class
                target.classList.remove('fc-option-active-btn');

            });

        } catch (error: unknown) {

            // Remove active class
            target.classList.remove('fc-option-active-btn');            

            // Check if error is known
            if ( error instanceof Error ) {

                // Show error notification
                showNotification('error', error.message);

            } else {

                // Display in the console the error
                console.log(error);

            }

        }

    };

    return (
        <>
        <form onSubmit={notificationsUpdate} autoComplete="off">
            <div className="flex">
                <div className="w-full">
                    <h3 className="fc-section-title">
                        { getWord('user', 'user_notifications', memberOptions['Language']) }
                    </h3>
                </div>
            </div>
            <div className="flex">
                <div className="w-full">
                    <ul className="fc-options">
                        {getField('extra', 'FieldCheckbox', {
                            name: 'NotificationsEnabled',
                            label: getWord('admin', 'admin_smtp', memberOptions['Language']),
                            description: getWord('admin', 'admin_smtp_description', memberOptions['Language']),
                            data: {
                                checked: getWord('default', 'default_enabled', memberOptions['Language']),
                                unchecked: getWord('default', 'default_enable', memberOptions['Language']),
                                value: fields.NotificationsEnabled
                            },
                            hook: {
                                fields: fields,
                                setFields: setFields
                            }
                        })}
                        {getField('extra', 'FieldEmail', {
                            name: 'NotificationsEmail',
                            label: getWord('default', 'default_email', memberOptions['Language']),
                            data: {
                                placeholder: getWord('default', 'default_enter_email_address', memberOptions['Language']),
                                value: fields.NotificationsEmail
                            },
                            hook: {
                                fields: fields,
                                setFields: setFields
                            }
                        })}
                    </ul>
                </div>
            </div>
            <div className="flex mt-5 mb-10">
                <div className="w-full text-right">
                    <button type="submit" className="mb-3 fc-option-btn fc-option-green-btn">
                        { getIcon('IconSave', {className: 'fc-load-icon'}) }
                        { getIcon('IconAutorenew', {className: 'fc-load-more-icon'}) }
                        { getWord('admin', 'admin_save_changes', memberOptions['Language']) }
                    </button> 
                </div>
            </div>     
        </form>
        </>
    );

}

// Export the Profile Notifications component
export default ProfileNotifications;