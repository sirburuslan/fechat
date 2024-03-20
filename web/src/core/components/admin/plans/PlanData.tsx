/*
 * @component Plan Data
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-16
 *
 * This file contains the plan data component in the administrator panel
 */

// Import the React hooks
import { useEffect, useContext, Dispatch, SetStateAction } from 'react';

// Import axios module
import axios, { AxiosError, AxiosResponse } from 'axios';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the incs
import { getField, getCurrencies, getWord, getIcon, getToken, showNotification } from '@/core/inc/incIndex';

// Import types
import { typeToken, typePostHeader } from '@/core/types/typesIndex';

// Import the options for website and member
import { MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Create the Plan Data component
const PlanData: React.FC<{planId: string, fields: {[key: string]: string | number}, setFields: Dispatch<SetStateAction<any>>, features: string[]}> = ({planId, fields, setFields, features}) => {

    // Member options
    let {memberOptions, setMemberOptions} = useContext(MemberOptionsContext); 
    
    // Run some code for the client
    useEffect((): () => void => {

        // Register an event for all clicks
        document.addEventListener('click', trackClicks);

        return (): void => {

            // Remove the event listener for clicks
            document.removeEventListener('click', trackClicks);

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
        if ( (target instanceof Element) && target.closest('.fc-option-dropdown') && (target.nodeName === 'A') ) {
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

        }

    };

    /**
     * Update the plan basic data
     * 
     * @param FormEvent e 
     */
    let planUpdateBasic = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
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
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/plans/' + planId, {
                Name: fields.Name,
                Price: fields.Price,
                Currency: fields.Currency
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
     * Update the plan restrictions
     * 
     * @param FormEvent e 
     */
    let planUpdateRestrictions = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
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
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`,
                    CsrfToken: csrfToken.token
                }
            };

            // Update the fields
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/plans/restrictions/' + planId, {
                PlanId: planId,
                Websites: fields.Websites?fields.Websites:0
            }, headers)

            // Process the response
            .then(async (response: AxiosResponse) => {
                console.log(response);

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
     * Update the plan features
     * 
     * @param FormEvent e 
     */
    let planUpdateFeatures = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
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
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`,
                    CsrfToken: csrfToken.token
                }
            };

            // Update the fields
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/plans/features/' + planId, features, headers)

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

    return (
        <>
            <form onSubmit={planUpdateBasic}>
                <div className="flex">
                    <div className="w-full">
                        <h3 className="fc-section-title">{ getWord('admin', 'admin_plan_information', memberOptions['Language']) }</h3>
                    </div>
                </div> 
                <div className="flex">
                    <div className="w-full">
                        <ul className="fc-options">
                            {getField('extra', 'FieldText', {
                                name: 'Name',
                                label: getWord('admin', 'admin_plan_name', memberOptions['Language']),
                                data: {
                                    placeholder: getWord('admin', 'admin_enter_plan_name', memberOptions['Language']),
                                    value: fields.Name
                                },
                                hook: {
                                    fields: fields,
                                    setFields: setFields
                                }
                            })}
                            {getField('extra', 'FieldText', {
                                name: 'Price',
                                label: getWord('admin', 'admin_plan_price', memberOptions['Language']),
                                data: {
                                    placeholder: getWord('admin', 'admin_enter_plan_price', memberOptions['Language']),
                                    value: fields.Price
                                },
                                hook: {
                                    fields: fields,
                                    setFields: setFields
                                }
                            })}
                            {getField('extra', 'FieldStaticList', {
                                name: 'Currency',
                                label: getWord('admin', 'admin_currencies', memberOptions['Language']),
                                data: {
                                    list: getCurrencies()
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
            <form onSubmit={planUpdateRestrictions}>
                <div className="flex">
                    <div className="w-full">
                        <h3 className="fc-section-title">{ getWord('admin', 'admin_plan_restrictions', memberOptions['Language']) }</h3>
                    </div>
                </div> 
                <div className="flex">
                    <div className="w-full">
                        <ul className="fc-options">
                            {getField('extra', 'FieldText', {
                                name: 'Websites',
                                label: getWord('default', 'default_websites', memberOptions['Language']),
                                data: {
                                    placeholder: getWord('admin', 'admin_enter_number_of_allowed_websites', memberOptions['Language']),
                                    value: fields.Websites
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
            <form onSubmit={planUpdateFeatures}>
                <div className="flex">
                    <div className="w-full">
                        <h3 className="fc-section-title">{ getWord('admin', 'admin_extra', memberOptions['Language']) }</h3>
                    </div>
                </div> 
                <div className="flex">
                    <div className="w-full">
                        <ul className="fc-options">
                            {getField('extra', 'FieldListManager', {
                                name: 'Features',
                                label: getWord('admin', 'admin_plan_features', memberOptions['Language']),
                                data: {},
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
    )

};

// Export the Plan Data component
export default PlanData;