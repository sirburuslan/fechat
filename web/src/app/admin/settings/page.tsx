/*
 * @page Settings
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This file contains the page settings in the administrator panel
 */


'use client'

// Import the React hooks
import { useState, useEffect, Dispatch, SetStateAction, useContext } from 'react';

// Import the Next Js link
import Link from 'next/link';

// Import axios module
import axios, { AxiosError, AxiosResponse } from 'axios';

// Import the UseQuery hook
import { useQuery } from 'react-query';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import incs
import { getWord, getIcon, getField, getToken, showNotification, getOptions, updateOptions } from '@/core/inc/incIndex';

// Import types
import { typeToken, typePostHeader, typeOptions } from '@/core/types/typesIndex';

// Import the options for website and member
import {WebsiteOptionsContext, MemberOptionsContext} from '@/core/contexts/OptionsContext';

// Create the Page content
const Page = () => {

    // Website options
    let {websiteOptions, setWebsiteOptions} = useContext(WebsiteOptionsContext); 

    // Member options
    let {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);

    // Set a hook for fields value
    let [fields, setFields] = useState({
        WebsiteName: '',
        HomePageLogo: '',
        SignInPageLogo: '',
        DashboardLogoSmall: '',
        DashboardLogoLarge: '',
        AnalyticsCode: '',
        PrivacyPolicy: '',
        Cookies: '',
        TermsOfService: '',
        DemoVideo: '',
        RegistrationEnabled: 0,
        RegistrationConfirmationEnabled: 0,
        Gateways: 'paypal',
        PayPalEnabled: 0,
        PayPalClientId: '',
        PayPalClientSecret: '',
        PayPalSandboxEnabled: 0,
        Ip2LocationEnabled: 0,
        Ip2LocationKey: '',
        GoogleMapsEnabled: 0,
        GoogleMapsKey: '',   
        GoogleAuthEnabled: 0,   
        GoogleClientId: '',
        GoogleClientSecret: '',  
        GoogleSignInRedirect: process.env.NEXT_PUBLIC_SITE_URL + 'auth/google/callback',
        ReCAPTCHAEnabled: 0,
        ReCAPTCHAKey: '',
        SmtpEnabled: 0,
        SmtpProtocol: "mail",
        SmtpHost: '',
        SmtpPort: "0",
        SmtpUsername: '',
        SmtpPassword: '',
        SmtpSending: 'ssl'
    });

    // Set a hook for errors value
    let [errors, setErrors] = useState({});  

    // Hook to fetch data with useQuery
    let [fetchedData, setFetchedData] = useState(false);

    // Check if document is defined
    if ( typeof document !== 'undefined' ) {

        // Update the page title dynamically
        document.title = getWord('admin', 'admin_settings', memberOptions['Language']);

    }

    // Get all settings options which are hidden in the website's options
    let getAllOptions = async (): Promise<any> => {

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

        // Request the fields value
        let response = await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/settings/list', null, headers);

        // Process the response
        return response.data;

    }

    // Request the options
    let { isLoading, error, data } = useQuery('optionsList', getAllOptions, {
        enabled: !fetchedData
    });

    useEffect(() => {

        // Register an event for all clicks
        document.addEventListener('click', trackClicks);

        return (): void => {

            // Remove the event listener for clicks
            document.removeEventListener('click', trackClicks);

        };

    }, []);

    // Monitor the data change
    useEffect((): void => {

        // Check if has occurred an error
        if ( error ) {

            // Show error notification
            showNotification('error', (error as Error).message);

        } else if ( (typeof data != 'undefined') && !data.success ) {

            // Show error notification
            showNotification('error', data.message);            

        } else if (data) {

            // Received fields
            let rFields: string[] = Object.keys(data.options as object);

            // Calculate fields length
            let fieldsLength: number = rFields.length;

            // List the options
            for ( let o = 0; o < fieldsLength; o++ ) {
                
                // Update the fields
                setFields((prev) => ({ ...prev, [rFields[o]]: data.options![rFields[o]]}));

            }

        } 

        // Stop to fetch data
        setFetchedData(true); 
    
    }, [data]);

    /**
     * Track any click
     * 
     * @param Event e
     */
    let trackClicks = (e: Event): void => {

        // Get the target
        let target = e.target;
        
        // Check if the click is inside dropdown
        if ( (target instanceof Element) && target.closest('.fc-settings-dropdown') && (target.nodeName === 'A') ) {
            e.preventDefault();
            
            // Get the element's ID
            let elementId: string | null = target.getAttribute('data-id');

            // Get the element's text
            let elementText: string | null = target.textContent;

            // Check if the click was done inside the payments gateways dropdown
            if ( target.closest('#fc-settings-uidropdown-gateways') ) {

                // Remove the fc-settings-gateways-show-tab class
                document.querySelectorAll('.fc-settings-gateways-tab.fc-settings-gateways-show-tab').forEach(link => {
                    link.classList.remove('fc-settings-gateways-show-tab');
                });
                
                // Add fc-settings-gateways-show-tab class to the tab
                document.querySelector('.fc-settings-gateways-tab[data-gateway="' + elementId + '"]')!.classList.add('fc-settings-gateways-show-tab');

            } else {

                // Get the option
                let option: string | null = target.closest('.fc-settings-option')!.getAttribute('data-option');

                // Verify if option is not null
                if ( option !== null ) {

                    // Get the fields list 
                    let fieldsList: {[key: string]: string | number} = fields;

                    // Update the field
                    fieldsList[option] = elementId as string;

                    // Prepare the setFields to support fieldsList
                    let setNewFields: Dispatch<SetStateAction<any>> = setFields;

                    // Update fields list
                    setNewFields(fieldsList);

                    // Show the save changes button
                    document.getElementsByClassName('fc-settings-actions')[0].classList.add('fc-settings-actions-show');

                }

                // Change the button parameters
                target.closest('.fc-settings-dropdown')!.getElementsByTagName('span')[0].textContent = elementText;

            }

        }

    };

    /**
     * Register the Change Tab handler
     * 
     * @param Event e 
     */
    let changeTab = (e: React.MouseEvent<Element>): void => {
        e.preventDefault();

        // Save target
        let target = e.target as Element;

        // Verify if tab exists
        if ( target.getAttribute('href') && document.querySelector(target.getAttribute('href')!) ) {

            // Remove the active class from nav links
            document.querySelectorAll('.fc-settings-sidebar ul li .fc-nav-link').forEach(link => {
                link.classList.remove('fc-active-nav-link');
            });

            // Remove the active class from tabs
            document.querySelectorAll('.fc-settings-tabs .fc-tabs .fc-tab').forEach(link => {
                link.classList.remove('fc-show-tab');
            });

            // Add active class to the nav link
            target.classList.add('fc-active-nav-link');

            // Add active class to the tab
            document.querySelector(target.getAttribute('href')!)!.classList.add('fc-show-tab');

        }

    }

    // Update the options
    let optionsUpdate = async (e: React.MouseEvent<HTMLButtonElement>): Promise<void> => {

        // Reset the errors messages
        setErrors({});

        // Get the target
        let target: HTMLButtonElement = e.currentTarget;

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
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/settings/update', fields, headers)

            // Process the response
            .then(async (response: AxiosResponse) => {

                // Remove active class
                target.classList.remove('fc-option-active-btn');

                // Verify if the response is successfully
                if ( response.data.success ) {
                    
                    // Show success notification
                    showNotification('success', response.data.message);

                    // Remove the save changes button
                    document.getElementsByClassName('fc-settings-actions')[0].classList.remove('fc-settings-actions-show');

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

                    // Count the keys
                    let keysTotal: number = keys.length;

                    // Check if keys exists
                    if ( keysTotal > 0 ) {

                        // Errors container
                        let errorsHolder: {[key: string]: string} = {};

                        // List the keys
                        for ( let e = 0; e < keysTotal; e++ ) {

                            // Save error in the container
                            errorsHolder[keys[e]] = response.data[keys[e]][0];

                        }

                        // Update the errors
                        setErrors(errorsHolder);

                    }

                    // Run error notification
                    throw new Error(getWord('admin', 'admin_changes_not_saved', memberOptions['Language']));                    

                }

            })

            // Catch the error message
            .catch((error: AxiosError): void => {

                // Remove active class
                target.classList.remove('fc-option-active-btn');

                // Check if error message exists
                if ( typeof error.message !== 'undefined' ) {

                    // Show error notification
                    throw new Error(error.message);                    

                }
                

            });

        } catch (error: unknown) {

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
            <div className="fc-settings-container">
                <div className="grid xl:grid-cols-6 auto-rows-auto">
                    <div className="fc-settings-sidebar xl:col-span-1">
                        <ul>
                            <li>
                                <Link href="#fc-settings-general-tab" className="fc-nav-link fc-active-nav-link" onClick={changeTab}>
                                    { getIcon('IconCropFree') }
                                    { getWord('admin', 'admin_general', memberOptions['Language']) }
                                </Link>
                            </li>
                            <li>
                                <Link href="#fc-settings-advanced-tab" className="fc-nav-link" onClick={changeTab}>
                                    { getIcon('IconTune') }
                                    { getWord('default', 'default_advanced', memberOptions['Language']) }
                                </Link>
                            </li>                            
                            <li>
                                <Link href="#fc-settings-members-tab" className="fc-nav-link" onClick={changeTab}>
                                    { getIcon('IconFollowTheSigns') }                 
                                    { getWord('admin', 'admin_members', memberOptions['Language']) }
                                </Link>
                            </li>
                            <li>
                                <Link href="#fc-settings-payments-tab" className="fc-nav-link" onClick={changeTab}>
                                    { getIcon('IconAddShoppingCart') }
                                    { getWord('admin', 'admin_payments', memberOptions['Language']) }
                                </Link>
                            </li>
                            <li>
                                <Link href="#fc-settings-location-tab" className="fc-nav-link" onClick={changeTab}>
                                    { getIcon('IconLocationOn') }
                                    { getWord('admin', 'admin_location', memberOptions['Language']) }
                                </Link>
                            </li>  
                            <li>
                                <Link href="#fc-settings-sign-in-tab" className="fc-nav-link" onClick={changeTab}>
                                    { getIcon('IconLogin') }
                                    { getWord('auth', 'auth_sign_in', memberOptions['Language']) }
                                </Link>
                            </li>                                                      
                            <li>
                                <Link href="#fc-settings-smtp-tab" className="fc-nav-link" onClick={changeTab}>
                                    { getIcon('IconForwardToInbox') }
                                    { getWord('admin', 'admin_smtp', memberOptions['Language']) }
                                </Link>
                            </li>                    
                        </ul>
                    </div>
                    <div className="fc-settings-tabs xl:col-span-5">   
                        <div className="w-full fc-settings-actions text-right">
                            <button type="button" className="mb-3 fc-option-btn fc-option-green-btn" onClick={optionsUpdate}>
                                { getIcon('IconSave', {className: 'fc-load-icon'}) }
                                { getIcon('IconAutorenew', {className: 'fc-load-more-icon'}) }
                                { getWord('admin', 'admin_save_changes', memberOptions['Language']) }
                            </button> 
                        </div>
                        <div className="w-full">
                            <div className="fc-tabs" id="fc-settings-tabs">
                                <div className="fc-tab fc-show-tab" id="fc-settings-general-tab">
                                    <ul className="fc-settings">
                                        {getField('general', 'FieldText', {
                                            name: 'WebsiteName',
                                            label: getWord('admin', 'admin_website_name', memberOptions['Language']),
                                            description: getWord('admin', 'admin_website_name_description', memberOptions['Language']),
                                            data: {
                                                placeholder: getWord('admin', 'admin_website_enter_name', memberOptions['Language']),
                                                value: fields.WebsiteName
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })}
                                        {getField('general', 'FieldImage', {
                                            name: 'HomePageLogo',
                                            label: getWord('admin', 'admin_home_page_logo', memberOptions['Language']),
                                            description: getWord('admin', 'admin_home_page_logo_description', memberOptions['Language']),
                                            data: {
                                                placeholder: getWord('admin', 'admin_home_page_enter_logo', memberOptions['Language']),
                                                value: fields.HomePageLogo
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })}
                                        {getField('general', 'FieldImage', {
                                            name: 'SignInPageLogo',
                                            label: getWord('admin', 'admin_sign_in_page_logo', memberOptions['Language']),
                                            description: getWord('admin', 'admin_sign_in_page_logo_description', memberOptions['Language']),
                                            data: {
                                                placeholder: getWord('admin', 'admin_sign_in_page_enter_logo', memberOptions['Language']),
                                                value: fields.SignInPageLogo
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })} 
                                        {getField('general', 'FieldImage', {
                                            name: 'DashboardLogoSmall',
                                            label: getWord('admin', 'admin_dashboard_small_logo', memberOptions['Language']),
                                            description: getWord('admin', 'admin_dashboard_small_logo_description', memberOptions['Language']),
                                            data: {
                                                placeholder: getWord('admin', 'admin_dashboard_small_enter_logo', memberOptions['Language']),
                                                value: fields.DashboardLogoSmall
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })}
                                        {getField('general', 'FieldImage', {
                                            name: 'DashboardLogoLarge',
                                            label: getWord('admin', 'admin_dashboard_large_logo', memberOptions['Language']),
                                            description: getWord('admin', 'admin_dashboard_large_logo_description', memberOptions['Language']),
                                            data: {
                                                placeholder: getWord('admin', 'admin_dashboard_large_enter_logo', memberOptions['Language']),
                                                value: fields.DashboardLogoLarge
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })}                                                                                                      
                                        {getField('general', 'FieldTextarea', {
                                            name: 'AnalyticsCode',
                                            label: getWord('admin', 'admin_analytics_code', memberOptions['Language']),
                                            description: getWord('admin', 'admin_analytics_code_description', memberOptions['Language']),
                                            data: {
                                                placeholder: getWord('admin', 'admin_analytics_enter_code', memberOptions['Language']),
                                                value: fields.AnalyticsCode
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })}
                                    </ul>
                                </div>
                                <div className="fc-tab" id="fc-settings-advanced-tab">
                                    <ul className="fc-settings">
                                        {getField('general', 'FieldText', {
                                            name: 'PrivacyPolicy',
                                            label: getWord('admin', 'admin_privacy_policy', memberOptions['Language']),
                                            description: getWord('admin', 'admin_privacy_policy_description', memberOptions['Language']),
                                            data: {
                                                placeholder: getWord('admin', 'admin_enter_url', memberOptions['Language']),
                                                value: fields.PrivacyPolicy
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })}
                                        {getField('general', 'FieldText', {
                                            name: 'Cookies',
                                            label: getWord('admin', 'admin_cookies', memberOptions['Language']),
                                            description: getWord('admin', 'admin_cookies_description', memberOptions['Language']),
                                            data: {
                                                placeholder: getWord('admin', 'admin_enter_url', memberOptions['Language']),
                                                value: fields.Cookies
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })}
                                        {getField('general', 'FieldText', {
                                            name: 'TermsOfService',
                                            label: getWord('admin', 'admin_terms_of_service', memberOptions['Language']),
                                            description: getWord('admin', 'admin_terms_of_service_description', memberOptions['Language']),
                                            data: {
                                                placeholder: getWord('admin', 'admin_enter_url', memberOptions['Language']),
                                                value: fields.TermsOfService
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })}
                                        {getField('general', 'FieldText', {
                                            name: 'DemoVideo',
                                            label: getWord('admin', 'admin_demo_video', memberOptions['Language']),
                                            description: getWord('admin', 'admin_demo_video_description', memberOptions['Language']),
                                            data: {
                                                placeholder: getWord('admin', 'admin_enter_url', memberOptions['Language']),
                                                value: fields.DemoVideo
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })}                                        
                                    </ul>
                                </div>                                
                                <div className="fc-tab" id="fc-settings-members-tab">
                                    <ul className="fc-settings">
                                        {getField('general', 'FieldCheckbox', {
                                            name: 'RegistrationEnabled',
                                            label: getWord('admin', 'admin_registration', memberOptions['Language']),
                                            description: getWord('admin', 'admin_registration_description', memberOptions['Language']),
                                            data: {
                                                checked: getWord('default', 'default_enabled', memberOptions['Language']),
                                                unchecked: getWord('default', 'default_enable', memberOptions['Language']),
                                                value: fields.RegistrationEnabled
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })} 
                                        {getField('general', 'FieldCheckbox', {
                                            name: 'RegistrationConfirmationEnabled',
                                            label: getWord('admin', 'admin_registration_confirmation', memberOptions['Language']),
                                            description: getWord('admin', 'admin_registration_confirmation_description', memberOptions['Language']),
                                            data: {
                                                checked: getWord('default', 'default_enabled', memberOptions['Language']),
                                                unchecked: getWord('default', 'default_enable', memberOptions['Language']),
                                                value: fields.RegistrationConfirmationEnabled
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })}                                                               
                                    </ul>                            
                                </div>
                                <div className="fc-tab" id="fc-settings-payments-tab">
                                    <div className="w-full">
                                        <ul className="fc-settings">
                                            {getField('general', 'FieldStaticList', {
                                                name: 'Gateways',
                                                label: getWord('admin', 'admin_gateways', memberOptions['Language']),
                                                description: getWord('admin', 'admin_gateways_description', memberOptions['Language']),
                                                data: {
                                                    list: [{
                                                        itemId: 'paypal',
                                                        itemName: 'PayPal'
                                                    }]
                                                },
                                                hook: {
                                                    fields: fields,
                                                    setFields: setFields,
                                                    errors: errors,
                                                    setErrors: setErrors
                                                }
                                            })}
                                        </ul>
                                    </div>
                                    <div className="w-full">
                                        <div className="fc-settings-gateways-tabs">
                                            <div className="fc-settings-gateways-tab fc-settings-gateways-show-tab" data-gateway="paypal">
                                                <ul className="fc-settings">
                                                    {getField('general', 'FieldCheckbox', {
                                                        name: 'PayPalEnabled',
                                                        label: getWord('admin', 'admin_paypal_status', memberOptions['Language']),
                                                        description: getWord('admin', 'admin_paypal_status_description', memberOptions['Language']),
                                                        data: {
                                                            checked: getWord('default', 'default_enabled', memberOptions['Language']),
                                                            unchecked: getWord('default', 'default_enable', memberOptions['Language']),
                                                            value: fields.PayPalEnabled
                                                        },
                                                        hook: {
                                                            fields: fields,
                                                            setFields: setFields,
                                                            errors: errors,
                                                            setErrors: setErrors
                                                        }
                                                    })}
                                                    {getField('general', 'FieldText', {
                                                        name: 'PayPalClientId',
                                                        label: getWord('admin', 'admin_paypal_client_id', memberOptions['Language']),
                                                        description: getWord('admin', 'admin_paypal_client_id_description', memberOptions['Language']),
                                                        data: {
                                                            placeholder: getWord('admin', 'admin_enter_client_id', memberOptions['Language']),
                                                            value: fields.PayPalClientId
                                                        },
                                                        hook: {
                                                            fields: fields,
                                                            setFields: setFields,
                                                            errors: errors,
                                                            setErrors: setErrors
                                                        }
                                                    })} 
                                                    {getField('general', 'FieldText', {
                                                        name: 'PayPalClientSecret',
                                                        label: getWord('admin', 'admin_paypal_client_secret', memberOptions['Language']),
                                                        description: getWord('admin', 'admin_paypal_client_secret_description', memberOptions['Language']),
                                                        data: {
                                                            placeholder: getWord('admin', 'admin_enter_client_secret', memberOptions['Language']),
                                                            value: fields.PayPalClientSecret
                                                        },
                                                        hook: {
                                                            fields: fields,
                                                            setFields: setFields,
                                                            errors: errors,
                                                            setErrors: setErrors
                                                        }
                                                    })}
                                                    {getField('general', 'FieldCheckbox', {
                                                        name: 'PayPalSandboxEnabled',
                                                        label: getWord('admin', 'admin_paypal_sandbox', memberOptions['Language']),
                                                        description: getWord('admin', 'admin_paypal_sandbox_description', memberOptions['Language']),
                                                        data: {
                                                            checked: getWord('default', 'default_enabled', memberOptions['Language']),
                                                            unchecked: getWord('default', 'default_enable', memberOptions['Language']),
                                                            value: fields.PayPalSandboxEnabled
                                                        },
                                                        hook: {
                                                            fields: fields,
                                                            setFields: setFields,
                                                            errors: errors,
                                                            setErrors: setErrors
                                                        }
                                                    })}                                         
                                                </ul>
                                            </div>                                    
                                        </div>
                                    </div>
                                </div>
                                <div className="fc-tab" id="fc-settings-location-tab">
                                    <ul className="fc-settings">
                                        {getField('general', 'FieldCheckbox', {
                                            name: 'Ip2LocationEnabled',
                                            label: getWord('admin', 'admin_ip_2_location', memberOptions['Language']),
                                            description: getWord('admin', 'admin_ip_2_location_description', memberOptions['Language']),
                                            data: {
                                                checked: getWord('default', 'default_enabled', memberOptions['Language']),
                                                unchecked: getWord('default', 'default_enable', memberOptions['Language']),
                                                value: fields.Ip2LocationEnabled
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })}
                                        {getField('general', 'FieldText', {
                                            name: 'Ip2LocationKey',
                                            label: getWord('admin', 'admin_ip_2_location_key', memberOptions['Language']),
                                            description: getWord('admin', 'admin_ip_2_location_key_description', memberOptions['Language']),
                                            data: {
                                                placeholder: getWord('admin', 'admin_enter_key', memberOptions['Language']),
                                                value: fields.Ip2LocationKey
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })} 
                                        {getField('general', 'FieldCheckbox', {
                                            name: 'GoogleMapsEnabled',
                                            label: getWord('admin', 'admin_google_maps', memberOptions['Language']),
                                            description: getWord('admin', 'admin_google_maps_description', memberOptions['Language']),
                                            data: {
                                                checked: getWord('default', 'default_enabled', memberOptions['Language']),
                                                unchecked: getWord('default', 'default_enable', memberOptions['Language']),
                                                value: fields.GoogleMapsEnabled
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })}
                                        {getField('general', 'FieldText', {
                                            name: 'GoogleMapsKey',
                                            label: getWord('admin', 'admin_google_maps_key', memberOptions['Language']),
                                            description: getWord('admin', 'admin_google_maps_key_description', memberOptions['Language']),
                                            data: {
                                                placeholder: getWord('admin', 'admin_enter_key', memberOptions['Language']),
                                                value: fields.GoogleMapsKey
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })}                                         
                                    </ul>
                                </div>
                                <div className="fc-tab" id="fc-settings-sign-in-tab">
                                    <ul className="fc-settings">
                                        {getField('general', 'FieldCheckbox', {
                                            name: 'GoogleAuthEnabled',
                                            label: getWord('admin', 'admin_google_login_registration', memberOptions['Language']),
                                            description: getWord('admin', 'admin_google_login_registration_description', memberOptions['Language']),
                                            data: {
                                                checked: getWord('default', 'default_enabled', memberOptions['Language']),
                                                unchecked: getWord('default', 'default_enable', memberOptions['Language']),
                                                value: fields.GoogleAuthEnabled
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })}
                                        {getField('general', 'FieldText', {
                                            name: 'GoogleClientId',
                                            label: getWord('admin', 'admin_google_client_id', memberOptions['Language']),
                                            description: getWord('admin', 'admin_google_client_id_description', memberOptions['Language']),
                                            data: {
                                                placeholder: getWord('admin', 'admin_enter_client_id', memberOptions['Language']),
                                                value: fields.GoogleClientId
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })}                                        
                                        {getField('general', 'FieldText', {
                                            name: 'GoogleClientSecret',
                                            label: getWord('admin', 'admin_google_client_secret', memberOptions['Language']),
                                            description: getWord('admin', 'admin_google_client_secret_description', memberOptions['Language']),
                                            data: {
                                                placeholder: getWord('admin', 'admin_enter_client_secret', memberOptions['Language']),
                                                value: fields.GoogleClientSecret
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })} 
                                        {getField('general', 'FieldText', {
                                            name: 'GoogleSignInRedirect',
                                            label: getWord('admin', 'admin_google_sign_in_redirect', memberOptions['Language']),
                                            description: getWord('admin', 'admin_google_sign_in_redirect_description', memberOptions['Language']),
                                            data: {
                                                placeholder: '',
                                                value: fields.GoogleSignInRedirect
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })}                                                                                                                                                         
                                    </ul>                  
                                </div>                                                                
                                <div className="fc-tab" id="fc-settings-smtp-tab">
                                    <ul className="fc-settings">
                                        {getField('general', 'FieldCheckbox', {
                                            name: 'SmtpEnabled',
                                            label: getWord('admin', 'admin_smtp', memberOptions['Language']),
                                            description: getWord('admin', 'admin_smtp_description', memberOptions['Language']),
                                            data: {
                                                checked: getWord('default', 'default_enabled', memberOptions['Language']),
                                                unchecked: getWord('default', 'default_enable', memberOptions['Language']),
                                                value: fields.SmtpEnabled
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })}
                                        {getField('general', 'FieldStaticList', {
                                            name: 'SmtpProtocol',
                                            label: getWord('admin', 'admin_smtp_protocol', memberOptions['Language']),
                                            description: getWord('admin', 'admin_smtp_protocol_description', memberOptions['Language']),
                                            data: {
                                                list: [{
                                                    itemId: 'mail',
                                                    itemName: 'Mail'
                                                }, {
                                                    itemId: 'sendmail',
                                                    itemName: 'SendMail'
                                                }, {
                                                    itemId: 'smtp',
                                                    itemName: 'SMTP'
                                                }]
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })} 
                                        {getField('general', 'FieldText', {
                                            name: 'SmtpHost',
                                            label: getWord('admin', 'admin_smtp_host', memberOptions['Language']),
                                            description: getWord('admin', 'admin_smtp_host_description', memberOptions['Language']),
                                            data: {
                                                placeholder: getWord('admin', 'admin_enter_smtp_host', memberOptions['Language']),
                                                value: fields.SmtpHost
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })} 
                                        {getField('general', 'FieldText', {
                                            name: 'SmtpPort',
                                            label: getWord('admin', 'admin_smtp_port', memberOptions['Language']),
                                            description: getWord('admin', 'admin_smtp_port_description', memberOptions['Language']),
                                            data: {
                                                placeholder: getWord('admin', 'admin_enter_smtp_port', memberOptions['Language']),
                                                value: fields.SmtpPort
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })} 
                                        {getField('general', 'FieldText', {
                                            name: 'SmtpUsername',
                                            label: getWord('admin', 'admin_smtp_username', memberOptions['Language']),
                                            description: getWord('admin', 'admin_smtp_username_description', memberOptions['Language']),
                                            data: {
                                                placeholder: getWord('admin', 'admin_enter_smtp_username', memberOptions['Language']),
                                                value: fields.SmtpUsername
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })}
                                        {getField('general', 'FieldText', {
                                            name: 'SmtpPassword',
                                            label: getWord('admin', 'admin_smtp_password', memberOptions['Language']),
                                            description: getWord('admin', 'admin_smtp_password_description', memberOptions['Language']),
                                            data: {
                                                placeholder: getWord('admin', 'admin_enter_smtp_password', memberOptions['Language']),
                                                value: fields.SmtpPassword
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })}
                                        {getField('general', 'FieldStaticList', {
                                            name: 'SmtpSending',
                                            label: getWord('admin', 'admin_smtp_sending', memberOptions['Language']),
                                            description: getWord('admin', 'admin_smtp_sending_description', memberOptions['Language']),
                                            data: {
                                                list: [{
                                                    itemId: 'ssl',
                                                    itemName: 'SSL'
                                                }, {
                                                    itemId: 'tsl',
                                                    itemName: 'TSL'
                                                }]
                                            },
                                            hook: {
                                                fields: fields,
                                                setFields: setFields,
                                                errors: errors,
                                                setErrors: setErrors
                                            }
                                        })}                                                                                                                            
                                    </ul>                            
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </>
    );

};

// Export the Page content
export default Page;