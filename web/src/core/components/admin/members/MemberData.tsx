/*
 * @component Member Data
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-14
 *
 * This file contains the member data component in the administrator panel
 */

'use client'

// Import the React hooks
import { useEffect, useContext, Dispatch, SetStateAction } from 'react';

// Import axios module
import axios, { AxiosError, AxiosResponse } from 'axios';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the incs
import { getIcon, getWord, getLanguages, getToken, getField, getOptions, updateOptions, showNotification } from '@/core/inc/incIndex';

// Import types
import { typeToken, typePostHeader, typeOptions } from '@/core/types/typesIndex';

// Import the options for website and member
import { WebsiteOptionsContext, MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Create the Member Data component
const MemberData: React.FC<{memberId: string, fields: {[key: string]: string | number}, setFields: Dispatch<SetStateAction<any>>}> = ({memberId, fields, setFields}): React.JSX.Element => {

    // Website options
    let {websiteOptions, setWebsiteOptions} = useContext(WebsiteOptionsContext); 

    // Member options
    let {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);   

    // Get the plans
    let plansList = async (): Promise<void> => {

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

            // Set the headers
            let headers: typePostHeader = {
                headers: {
                    Authorization: `Bearer ${token}`,
                    CsrfToken: csrfToken.token
                }
            };

            // Request the plans list
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/plans/list', {
                Page: 1,
                Search: document.querySelector<HTMLInputElement>('#fc-option-dropdown-plan .fc-dropdown-search')!.value
            }, headers)
            
            // Process the response
            .then((response: AxiosResponse): void => {
                console.log(response);
                // Check if search is not empty
                if ( document.querySelector('.fc-search-box.fc-search-active') ) {

                    // Remove fc-search-active class from search input
                    document.getElementsByClassName('fc-search-box')[0]!.classList.remove('fc-search-active');

                    // Remove fc-search-complete class to search input
                    document.getElementsByClassName('fc-search-box')[0]!.classList.add('fc-search-complete');                    

                }

                // Verify if the response is successfully
                if ( response.data.success ) {

                    // Plans list container
                    let plans: string = '';

                    // List the plans
                    for ( let plan of response.data.result.elements ) {

                        // Add plan to the list
                        plans += '<li>'
                            + '<a href="#" class="#" data-id="' + plan.planId + '">'
                                + plan.name
                            + '</a>'
                        + '</li>';

                    }
                    
                    // Display the plans
                    document.querySelector('#fc-option-dropdown-plan ul')!.innerHTML = plans;

                } else {

                    // Set error message
                    throw new Error(response.data.message);                      

                }

            })

            // Proccess the response
            .catch((error: AxiosError): void => {
                console.log(error);
                // Check if error message exists
                if ( typeof error.message !== 'undefined' ) {

                    // Show error notification
                    throw new Error(error.message);                    

                }

            });

        } catch(e: unknown) {
            console.log(e);
            // Check if no plans
            if ( e instanceof Error ) {
                
                // Display the no plans found message
                document.querySelector('#fc-option-dropdown-plan ul')!.innerHTML = '<li class="fc-no-results-found-message">'
                    + e.message
                + '</li>';                  

            } else {

                // Display the exception in the console
                console.log(e);

            }

        }

    }    

    // Run some code for the client
    useEffect((): () => void => {

        // Register an event for all clicks
        document.addEventListener('click', trackClicks);

        // Register an event for all inputs
        document.addEventListener('input', trackInputs);        

        return (): void => {

            // Remove the event listener for clicks
            document.removeEventListener('click', trackClicks);

            // Remove the event listener for inputs
            document.removeEventListener('input', trackInputs);            

        };

    }, []);

    /**
     * Track any click
     * 
     * @param Event e
     */
    let trackClicks = (e: Event): void => {

        // Get the target
        let target = e.target;
        
        // Check if the click is inside dropdown
        if ( (target instanceof Element) && target.closest('.fc-option-dropdown') && !target.closest('#fc-option-dropdown-plan') && (target.nodeName === 'A') ) {
            e.preventDefault();
            
            // Get the element's ID
            let elementId: string | null = target.getAttribute('data-id');

            // Get the option
            let option: string | null = target.closest('.fc-extra-option')!.getAttribute('data-option');

            // Verify if option is not null
            if ( option !== null ) {
        
                // Update the field
                setFields((prev: any) => ({
                    ...prev,
                    [option!]: elementId as string
                }));

            }

        } else if ( (target instanceof Element) && target.closest('#fc-option-dropdown-plan') && (target.nodeName === 'BUTTON') ) {
            e.preventDefault();

            // Empty the search
            document.querySelector<HTMLInputElement>('#fc-option-dropdown-plan .fc-dropdown-search')!.value = '';

            // Get the plans
            plansList();
            
        } else if ( (target instanceof Element) && target.closest('#fc-option-dropdown-plan') && (target.nodeName === 'A') ) {
            e.preventDefault();

            // Get the element's ID
            let elementId: string | null = target.getAttribute('data-id');

            // Get the element's text
            let elementText: string | null = target.textContent;            

            // Get button
            let button: HTMLButtonElement | null = document.querySelector('#fc-option-dropdown-plan button');

            // Set button id
            button!.setAttribute('data-id', elementId!);

            // Set button text
            button!.getElementsByTagName('span')[0].textContent = elementText;            
            
        }

    };

    /**
     * Track any input
     * 
     * @param Event e
     */
    let trackInputs = (e: Event): void => {

        // Get the target
        let target = e.target;
        
        // Check if the input is inside dropdown
        if ( (target instanceof Element) && target.closest('#fc-option-dropdown-plan') ) {
            e.preventDefault();

            // Get the plans
            plansList();
            
        }

    };

    /**
     * Update the member's password
     * 
     * @param FormEvent e 
     */
    let passwordUpdate = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
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
            await axios.put(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/members/' + memberId, {
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

    /**
     * Update the member's data
     * 
     * @param FormEvent e 
     */
    let memberUpdate = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
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
console.log(headers);
            // Update the fields
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/members/' + memberId, {
                MemberId: fields.MemberId,
                FirstName: fields.FirstName,
                LastName: fields.LastName,
                Email: fields.Email,
                Phone: fields.Phone,
                Role: fields.Role,
                Language: fields.Language,
                PlanId: document.querySelector('#fc-option-dropdown-plan button')?.getAttribute('data-id')?document.querySelector('#fc-option-dropdown-plan button')?.getAttribute('data-id'):0
            }, headers)

            // Process the response
            .then(async (response: AxiosResponse) => {

                // Verify if the response is successfully
                if ( response.data.success ) {
                    
                    // Show success notification
                    showNotification('success', response.data.message);

                    // Verify if was updated data for the logged administrator
                    if ( memberId === memberOptions.MemberId ) {

                        // Request the options
                        let optionsList: {success: boolean, options?: typeOptions} = await getOptions();

                        // Update memberOptions
                        updateOptions(optionsList, setWebsiteOptions, setMemberOptions);

                    }

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
        <form onSubmit={memberUpdate}>
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
            <div className="flex mt-10">
                <div className="w-full">
                    <h3 className="fc-section-title">{getWord('default', 'default_advanced', memberOptions['Language'])}</h3>
                </div>
            </div>
            <div className="flex">
                <div className="w-full">
                    <ul className="fc-options">
                        {getField('extra', 'FieldStaticList', {
                            name: 'Role',
                            label: getWord('admin', 'admin_role', memberOptions['Language']),
                            data: {
                                list: [{
                                    itemId: '0',
                                    itemName: 'Administrator'
                                }, {
                                    itemId: '1',
                                    itemName: 'User'
                                }]
                            },
                            hook: {
                                fields: fields,
                                setFields: setFields
                            }
                        })}
                        {(fields.Role !== '0')?getField('extra', 'FieldDynamicList', {
                            name: 'Plan',
                            label: getWord('admin', 'admin_plans', memberOptions['Language']),
                            placeholder: getWord('admin', 'admin_search_for_plans', memberOptions['Language']),
                            data: {},
                            hook: {
                                fields: fields,
                                setFields: setFields
                            }
                        }):''}
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

// Export the Member Data component
export default MemberData;