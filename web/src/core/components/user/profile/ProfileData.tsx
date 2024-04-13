/*
 * @component Profile Data
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-09
 *
 * This file contains the profile data component in the user panel
 */

'use client'

// Import the React hooks
import { useContext, Dispatch, SetStateAction } from 'react';

// Import axios module
import axios, { AxiosError, AxiosResponse } from 'axios';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the incs
import { getIcon, getWord, getLanguages, getField, getOptions, updateOptions, showNotification } from '@/core/inc/incIndex';

// Import types
import { typePostHeader, typeOptions } from '@/core/types/typesIndex';

// Import the options for website and member
import { WebsiteOptionsContext, MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Create the Profile Data component
const ProfileData: React.FC<{fields: {[key: string]: string | number}, setFields: Dispatch<SetStateAction<any>>}> = ({fields, setFields}): React.JSX.Element => {

    // Website options
    const {websiteOptions, setWebsiteOptions} = useContext(WebsiteOptionsContext); 

    // Member options
    const {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);   

    /**
     * Update the member's data
     * 
     * @param FormEvent e 
     */
    const memberUpdate = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
        e.preventDefault();

        // Get the target
        const target: Element = e.currentTarget;

        // Add active class
        target.classList.add('fc-option-active-btn');

        try {

            // Set the bearer token
            const token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');

            // Prepare the headers
            const headers: typePostHeader = {
                headers: {
                    Authorization: `Bearer ${token}`
                }
            };

            // Update the fields
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/profile', {
                MemberId: fields.MemberId,
                FirstName: fields.FirstName,
                LastName: fields.LastName,
                Email: fields.Email,
                Phone: fields.Phone,
                Language: fields.Language
            }, headers)

            // Process the response
            .then(async (response: AxiosResponse) => {

                // Verify if the response is successfully
                if ( response.data.success ) {
                    
                    // Show success notification
                    showNotification('success', response.data.message);

                    // Request the options
                    const optionsList: {success: boolean, options?: typeOptions} = await getOptions();

                    // Update memberOptions
                    updateOptions(optionsList, setWebsiteOptions, setMemberOptions);

                } else if ( typeof response.data.message !== 'undefined' ) {

                    // Run error notification
                    throw new Error(response.data.message);

                } else {

                    // Keys container
                    const keys: string[] = Object.keys(response.data);

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

    /**
     * Update the member's password
     * 
     * @param FormEvent e 
     */
    const passwordUpdate = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
        e.preventDefault();

        // Get the target
        const target: Element = e.currentTarget;

        // Add active class
        target.classList.add('fc-option-active-btn');

        try {

            // Set the bearer token
            const token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');

            // Prepare the headers
            const headers: typePostHeader = {
                headers: {
                    Authorization: `Bearer ${token}`
                }
            };

            // Update the fields
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/profile/security', {
                Password: fields.Password,
                RepeatPassword: fields.RepeatPassword
            }, headers)

            // Process the response
            .then(async (response: AxiosResponse) => {

                // Verify if the response is successfully
                if ( response.data.success ) {
                    
                    // Show success notification
                    showNotification('success', response.data.message);

                } else if ( typeof response.data.message !== 'undefined' ) {

                    // Run error notification
                    throw new Error(response.data.message);

                } else {

                    // Keys container
                    const keys: string[] = Object.keys(response.data);

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
        <form onSubmit={memberUpdate} autoComplete="off">
            <div className="flex">
                <div className="w-full">
                    <h3 className="fc-section-title">
                        {getWord('default', 'default_basic_information', memberOptions['Language'])}
                    </h3>
                </div>
            </div>
            <div className="flex">
                <div className="w-full">
                    <ul className="fc-options">
                        {getField('extra', 'FieldText', {
                            name: 'FirstName',
                            label: getWord('default', 'default_first_name', memberOptions['Language']),
                            data: {
                                placeholder: getWord('default', 'default_enter_first_name', memberOptions['Language']),
                                value: fields.FirstName
                            },
                            hook: {
                                fields: fields,
                                setFields: setFields
                            }
                        })}
                        {getField('extra', 'FieldText', {
                            name: 'LastName',
                            label: getWord('default', 'default_last_name', memberOptions['Language']),
                            data: {
                                placeholder: getWord('default', 'default_enter_last_name', memberOptions['Language']),
                                value: fields.LastName
                            },
                            hook: {
                                fields: fields,
                                setFields: setFields
                            }
                        })}
                        {getField('extra', 'FieldEmail', {
                            name: 'Email',
                            label: getWord('default', 'default_email', memberOptions['Language']),
                            data: {
                                placeholder: getWord('default', 'default_enter_email_address', memberOptions['Language']),
                                value: fields.Email
                            },
                            hook: {
                                fields: fields,
                                setFields: setFields
                            }
                        })}
                        {getField('extra', 'FieldPhone', {
                            name: 'Phone',
                            label: getWord('default', 'default_phone_number', memberOptions['Language']),
                            data: {
                                placeholder: getWord('default', 'default_enter_the_phone_number', memberOptions['Language']),
                                value: fields.Phone
                            },
                            hook: {
                                fields: fields,
                                setFields: setFields
                            }
                        })}
                        {getField('extra', 'FieldStaticList', {
                            name: 'Language',
                            label: getWord('default', 'default_languages', memberOptions['Language']),
                            data: {
                                list: getLanguages(memberOptions['Language'])
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
        <form onSubmit={passwordUpdate}>       
            <div className="flex mt-10">
                <div className="w-full">
                    <h3 className="fc-section-title">{getWord('default', 'default_security', memberOptions['Language'])}</h3>
                </div>
            </div>
            <div className="flex">
                <div className="w-full">
                    <ul className="fc-options">
                        {getField('extra', 'FieldPassword', {
                            name: 'Password',
                            label: getWord('default', 'default_password', memberOptions['Language']),
                            data: {
                                placeholder: getWord('default', 'default_enter_password', memberOptions['Language']),
                                value: fields.Password
                            },
                            hook: {
                                fields: fields,
                                setFields: setFields
                            }
                        })}
                        {getField('extra', 'FieldPassword', {
                            name: 'RepeatPassword',
                            label: getWord('default', 'default_repeat_password', memberOptions['Language']),
                            data: {
                                placeholder: getWord('default', 'default_enter_password_again', memberOptions['Language']),
                                value: fields.RepeatPassword
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

// Export the Profile Data component
export default ProfileData;