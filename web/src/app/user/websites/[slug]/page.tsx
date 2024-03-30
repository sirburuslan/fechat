/*
 * @page Website
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This file contains the page for a single website
 */

'use client'

// Import the react hooks
import { useState, useContext, useEffect, useRef } from 'react';

// Import the Next JS Link component
import Link from 'next/link';

// Import axios module
import axios, { AxiosError, AxiosResponse } from 'axios';

// Import the UseQuery hook
import { useQuery } from 'react-query';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the incs
import { getIcon, getWord, getToken, getField, showNotification, calculateTime } from '@/core/inc/incIndex';

// Import types
import { typeToken, typePostHeader } from '@/core/types/typesIndex';

// Import the options for website and member
import {MemberOptionsContext} from '@/core/contexts/OptionsContext';

// Import the Uis
import { UiDropdown, UiNavigation } from '@/core/components/general/ui/UiIndex';

// Import the Confirmation component
import Confirmation from '@/core/components/general/Confirmation';

// Create the page content
const Page = ({params}: {params: {slug: string}}): React.JSX.Element => {

    // Member options
    let {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);

    // Set a hook for fields value
    let [fields, setFields] = useState({
        Name: '',
        Url: '',
        Enabled: 0,
        Header: '',
        Embed: '',
        Default: 1
    });

    // Set a hook for error message if website can't be reached
    let [websiteError, setWebsiteError] = useState<string>();

    // Set a hook for search parameters
    let [search, setSearch] = useState({
        Search: '',
        Page: 1
    });   
    
    // Set a hook for navigation
    let [navigation, setNavigation] = useState({
        scope: 'threads',
        page: 1,
        total: 0,
        limit: 10
    }); 

    // Threads list holder
    let [threads, setThreads] = useState<React.ReactNode | null>(null);

    // Hook to fetch data with useQuery
    let [fetchedData, setFetchedData] = useState(false);

    // Search pause container
    let searchPause = useRef<NodeJS.Timeout>();

    // Get the website's information
    let websiteInfo = async (): Promise<any> => {

        // Generate a new csrf token
        let csrfToken: typeToken = await getToken();

        // Check if csrf token is missing
        if ( !csrfToken.success ) {

            // Return error response
            return {
                success: false,
                message: getWord('errors', 'error_csrf_token_not_generated', memberOptions['Language'])
            };

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
        let response = await axios.get(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/websites/' + params.slug, headers);

        // Process the response
        return response.data;

    };

    /*
     * Schedule a search
     * 
     * @param funcion fun contains the function
     * @param integer interval contains time
     */
    let scheduleSearch = ($fun: () => void, interval: number): void => {

        // Verify if an event was already scheduled
        if (searchPause.current) {

            // Clear the previous timeout
            clearTimeout(searchPause.current);

        }

        // Add to queue
        searchPause.current = setTimeout($fun, interval);
        
    };

    /**
     * Search for threads
     * 
     * @param React.ChangeEvent e 
     */
    let searchThreads = (e: React.ChangeEvent<HTMLInputElement>): void => {

        // Add fc-search-active to show the animation
        e.target.closest('.fc-search-box')!.classList.add('fc-search-active');

        // Set search page
        search.Page = 1;

        // Set search value
        search.Search = e.target.value;

        // Search for threads
        setSearch(search);

        // Schedule a search
        scheduleSearch((): void => { 
            
            // Request the threads list
            threadsListRequest();

        }, 1000);

    };

    // Create the request for threads
    let threadsListRequest = async (): Promise<void> => {

        // Hide the navigation box
        document.getElementsByClassName('fc-navigation')[0].classList.remove('block');

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

            // Request the threads list
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/threads/list/' + params.slug, search, headers)
            
            // Process the response
            .then((response: AxiosResponse): void => {

                // Check if search is not empty
                if ( document.querySelector('.fc-search-box.fc-search-active') ) {

                    // Remove fc-search-active class from search input
                    document.getElementsByClassName('fc-search-box')[0]!.classList.remove('fc-search-active');

                    // Remove fc-search-complete class to search input
                    document.getElementsByClassName('fc-search-box')[0]!.classList.add('fc-search-complete');                    

                }

                // Verify if the response is successfully
                if ( response.data.success ) {

                    // Update threads
                    setThreads(response.data.result.elements.map((thread: any, index: number) => {

                        // Dropdown items
                        let dropdownItems: Array<{text: string, url: string, id: string}> = [{
                            text: getWord('default', 'default_delete', memberOptions['Language']),
                            url: '#',
                            id: 'delete-thread'
                        }];

                        return (
                            <div className="fc-thread flex justify-between" key={index} data-thread={thread.threadId}>
                                <div>
                                    <h4>
                                        <Link href={'/user/threads/' + thread.threadId}>
                                            {thread.guestName}
                                        </Link>
                                    </h4>
                                    <p>{calculateTime(parseInt(thread.created), parseInt(response.data.time), memberOptions['Language'])}</p>
                                </div>
                                <div className="text-center">
                                </div>                        
                                <div className="text-right">
                                    <button type="button" className="fc-manage-thread" onClick={ threadDropdown }>
                                        { getIcon('IconMoreHoriz') }
                                    </button>
                                    <UiDropdown button="" options={ dropdownItems } id={'fc-thread-menu-' + thread.threadId} />
                                </div>
                            </div>
                        );

                    }));

                    // Set limit
                    let limit: number = ((response.data.result.page * 10) < response.data.result.total)?(response.data.result.page * 10):response.data.result.total;

                    // Set text
                    document.querySelector('#fc-navigation-threads h3')!.innerHTML = (((response.data.result.page - 1) * 10) + 1) + '-' + limit + ' ' + getWord('default', 'default_of', memberOptions['Language']) + ' ' + response.data.result.total + ' ' + getWord('default', 'default_results', memberOptions['Language']);

                    // Set page
                    navigation.page = response.data.result.page;                    

                    // Set total
                    navigation.total = response.data.result.total;

                    // Set pagination
                    setNavigation(navigation);

                    // Show the navigation box
                    document.getElementsByClassName('fc-navigation')[0].classList.add('block');

                } else {

                    // Update threads
                    setThreads(<div className="fc-no-threads-found">{response.data.message}</div>);

                }

            })

            // Proccess the response
            .catch((error: AxiosError): void => {

                // Check if error message exists
                if ( typeof error.message !== 'undefined' ) {

                    // Show error notification
                    throw new Error(error.message);                    

                }

            });

        } catch(e: unknown) {

            // Check if search is not empty
            if ( document.querySelector('.fc-search-box.fc-search-active') ) {

                // Remove fc-search-active class from search input
                document.getElementsByClassName('fc-search-box')[0]!.classList.remove('fc-search-active');

                // Remove fc-search-complete class to search input
                document.getElementsByClassName('fc-search-box')[0]!.classList.add('fc-search-complete');                    

            }

        }

    };

    // Request the website information
    let { isLoading, error, data } = useQuery('websiteInfo-' + params.slug, websiteInfo, {
        enabled: !fetchedData
    });

    // Run some code for the client
    useEffect((): () => void => {

        // Request the threads list
        threadsListRequest();

        // Register an event for clicks tracking
        document.addEventListener('click', trackClicks);

        return (): void => {

            // Remove event for clicks tracking
            document.removeEventListener('click', trackClicks);

        }

    }, []);

    // Monitor the data change
    useEffect((): void => {

        // Check if has occurred an error
        if ( error ) {

            // Set website error message
            setWebsiteError((error as Error).message);

        } else if ( (typeof data != 'undefined') && !data.success ) {

            // Set website error message
            setWebsiteError(data.message);

        } else if (data) {

            // Received fields
            let rFields: string[] = Object.keys(data.website as object);

            // Calculate fields length
            let fieldsLength: number = rFields.length;

            // List the website infos
            for ( let o = 0; o < fieldsLength; o++ ) {

                // Check if field is website's name
                if ( rFields[o] === 'Name' ) {

                    // Set website name
                    document.title = data.website![rFields[o]];

                }

                // Update the field
                setFields((prev: { Name: string; Url: string; Enabled: number; Header: string; Embed: string; Default: number; }) => ({
                    ...prev,
                    [rFields[o]]: data.website![rFields[o]]
                }));

            }   

            // Create embed
            let embed: string = '<script src="' + process.env.NEXT_PUBLIC_SITE_URL + 'api/embed/' + params.slug + '"></script>';

            // Set embed
            setFields((prev: { Name: string; Url: string; Enabled: number; Header: string; Embed: string; Default: number; }) => ({
                ...prev,
                Embed: embed
            }));

            // Disable default
            setFields((prev: { Name: string; Url: string; Enabled: number; Header: string; Embed: string; Default: number; }) => ({
                ...prev,
                Default: 0
            }));

        } 

        // Stop to fetch data
        setFetchedData(true); 

    }, [data]);

    /**
     * Track any click
     * 
     * @param Event e
     */
    let trackClicks = async (e: Event): Promise<void> => {

        // Get the target
        let target = e.target;

        // Check if the click is inside dropdown
        if ( (target instanceof Element) && target.closest('.fc-dropdown-menu') && (target.nodeName === 'A') && (target.getAttribute('data-id') === 'delete-thread') ) {
            e.preventDefault();

            // Get the website's ID
            let websiteId: string | null = target.closest('.fc-thread')!.getAttribute('data-thread');

            // Create a link
            let newLink: HTMLAnchorElement = document.createElement('a');

            // Set the website's ID
            newLink.setAttribute('data-id', websiteId!);

            // Append link
            document.body.appendChild(newLink);

            // Set class
            newLink.classList.add('fc-confirmation-modal-button');

            // Trigger click
            newLink.dispatchEvent(new Event('click', {
                bubbles: true,
                cancelable: true
            }));

            if (newLink.parentNode) {

                // Remove the link
                newLink.parentNode!.removeChild(newLink);

            }

        } else if ( (target instanceof Element) && target.closest('#fc-navigation-threads') && (target.nodeName === 'A') ) {
            e.preventDefault();

            // Get the page
            let page: string | null = target.getAttribute('data-page');

            // Set search value
            search.Page = parseInt(page!);

            // Search for threads
            setSearch(search);

            // Request the threads list
            threadsListRequest();

        }

    };

    // Create the function which will handle the thread's menu click
    let threadDropdown = (e: React.MouseEvent<HTMLElement>): void => {
        e.preventDefault();

        // Get target
        let target = e.target as HTMLElement;

        // Select a dropdown
        let dropdown: HTMLCollectionOf<Element> = target.closest('div')!.getElementsByClassName('fc-dropdown');

        // Verify if the dropdown is open
        if ( dropdown[0].getAttribute('data-expand') === 'false' ) {

            // Set true
            dropdown[0].setAttribute('data-expand', 'true');

            // Get menu
            let menu: Element = dropdown[0]!.getElementsByClassName('fc-dropdown-menu')[0];

            // Get the height
            let height: number = menu.clientHeight;

            // Calculate the height of the button
            let button_height: number = target.offsetHeight;

            // Set transformation
            (menu as HTMLElement).style.transform = `translate3d(0, -${button_height + height}px, 0)`;
            
        } else {

            // Set false
            dropdown[0].setAttribute('data-expand', 'false');
            
        }

    }

    /**
     * Cancel search for threads
     * 
     * @param React.ChangeEvent e 
     */
    let cancelSearchThreads = (e: React.MouseEvent<HTMLAnchorElement>): void => {

        // Add fc-search-complete to hide the animation
        e.currentTarget.closest('.fc-search-box')!.classList.remove('fc-search-complete');

        // Reset search
        search.Search = '';

        // Search for threads
        setSearch(search);

        // Set a pause
        setTimeout(() => {

            // Request the threads list
            threadsListRequest();

        }, 500);

        // Empty the search value
        (document.getElementById('fc-search-for-threads') as HTMLInputElement).value = '';



    };

    /**
     * Delete thread
     * 
     * @param string threadId
     */
    const deleteThread = async (threadId: string): Promise<void> => {

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

            // Delete thread
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/threads/' + threadId, null, headers)

            // Process the response
            .then((response: AxiosResponse) => {

                // Verify if the response is successfully
                if ( response.data.success ) {
                    
                    // Show success notification
                    showNotification('success', response.data.message);

                    // Check if this was the last thread in the list
                    if ( document.querySelectorAll('.fc-threads-list > .fc-thread').length < 2 ) {

                        // Set previous page
                        search.Page = (search.Page > 1)?(search.Page - 1):search.Page;

                        // Search for threads
                        setSearch(search);

                        // Request the threads list
                        threadsListRequest();

                    } else {

                        // Request the threads list
                        threadsListRequest();

                    }

                } else if ( typeof response.data.message !== 'undefined' ) {

                    // Run error notification
                    throw new Error(response.data.message);

                }

            })

            // Catch the error message
            .catch((e: AxiosError): void => {

                // Show error notification
                throw new Error(e.message);

            });

        } catch (e: unknown) {

            // Check if error is known
            if ( e instanceof Error ) {

                // Show error notification
                showNotification('error', e.message);

            } else {

                // Display in the console the error
                console.log(e);

            }

        }

    };

    /**
     * Update the website data
     * 
     * @param FormEvent e 
     */
    let updateWebsite = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
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
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/websites/' + params.slug, {
                Name: fields.Name,
                Url: fields.Url
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
     * Update the chat data
     * 
     * @param FormEvent e 
     */
    let updateChat = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
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
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/websites/' + params.slug + '/chat', {
                Enabled: fields.Enabled,
                Header: fields.Header
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

    return (
        <>
            {(typeof websiteError === 'undefined')?(
                <>
                    <Confirmation confirmAction={deleteThread}><>{getWord('user', 'user_deleting_thread_permanent', memberOptions['Language'])}</></Confirmation>
                    <div className="fc-website-container">
                        <div className="grid grid-cols-1 sm:grid-cols-1 md:grid-cols-4 lg:grid-cols-6 xl:grid-cols-6 gap-1 sm:gap-1 md:gap-6 lg:gap-6 xl:gap-6 mb-3">
                            <div className="fc-website-options col-span-1 md:col-span-2 lg:col-span-3 xl:col-span-3">
                                <form onSubmit={updateWebsite}>
                                    <div className="flex">
                                        <div className="w-full">
                                            <h3 className="fc-section-title">{ getWord('user', 'user_website', memberOptions['Language']) }</h3>
                                        </div>
                                    </div> 
                                    <div className="flex">
                                        <div className="w-full">
                                            <ul className="fc-options">
                                                {getField('extra', 'FieldText', {
                                                    name: 'Name',
                                                    label: getWord('user', 'user_website_name', memberOptions['Language']),
                                                    data: {
                                                        placeholder: getWord('user', 'user_enter_website_name', memberOptions['Language']),
                                                        value: fields.Name
                                                    },
                                                    hook: {
                                                        fields: fields,
                                                        setFields: setFields
                                                    }
                                                })}
                                                {getField('extra', 'FieldText', {
                                                    name: 'Url',
                                                    label: getWord('user', 'user_website_url', memberOptions['Language']),
                                                    data: {
                                                        placeholder: getWord('user', 'user_enter_website_url', memberOptions['Language']),
                                                        value: fields.Url
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
                                <form onSubmit={updateChat}>
                                    <div className="flex">
                                        <div className="w-full">
                                            <h3 className="fc-section-title">{ getWord('user', 'user_chat', memberOptions['Language']) }</h3>
                                        </div>
                                    </div> 
                                    <div className="flex">
                                        <div className="w-full">
                                            <ul className="fc-options">
                                                {getField('extra', 'FieldCheckbox', {
                                                    name: 'Enabled',
                                                    label: getWord('default', 'default_enabled', memberOptions['Language']),
                                                    data: {
                                                        checked: getWord('default', 'default_enabled', memberOptions['Language']),
                                                        unchecked: getWord('default', 'default_enable', memberOptions['Language']),
                                                        value: fields.Enabled
                                                    },
                                                    hook: {
                                                        fields: fields,
                                                        setFields: setFields
                                                    }
                                                })}
                                                {getField('extra', 'FieldText', {
                                                    name: 'Header',
                                                    label: getWord('user', 'user_chat_header', memberOptions['Language']),
                                                    data: {
                                                        placeholder: getWord('user', 'user_enter_chat_header', memberOptions['Language']),
                                                        value: fields.Header
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
                                <form>
                                    <div className="flex">
                                        <div className="w-full">
                                            <h3 className="fc-section-title">{ getWord('user', 'user_embed', memberOptions['Language']) }</h3>
                                        </div>
                                    </div> 
                                    <div className="flex">
                                        <div className="w-full">
                                            <ul className="fc-options">
                                                {getField('extra', 'FieldTextarea', {
                                                    name: 'Embed',
                                                    label: getWord('user', 'user_embed_code', memberOptions['Language']),
                                                    data: {
                                                        placeholder: '',
                                                        value: fields.Embed
                                                    },
                                                    hook: {
                                                        fields: fields,
                                                        setFields: setFields
                                                    }
                                                })}
                                            </ul>
                                        </div>
                                    </div>
                                </form>
                            </div>
                            <div className="fc-threads-container col-span-1 md:col-span-2 lg:col-span-3 xl:col-span-3">
                                <div className="flex">
                                    <div className="w-full">
                                        <h3 className="fc-section-title">
                                            { getWord('user', 'user_threads', memberOptions['Language']) }
                                        </h3>
                                    </div>
                                </div>
                                <div className="flex mb-3">
                                    <div className="w-full flex">
                                        <div className="flex fc-search-box fc-transparent-color">
                                            <span className="fc-search-icon">
                                                { getIcon('IconSearch') }
                                            </span>
                                            <input type="text" className="form-control fc-search-input" placeholder={ getWord('user', 'user_search_for_threads', memberOptions['Language']) } id="fc-search-for-threads" onInput={searchThreads} />
                                            <Link href="#" onClick={cancelSearchThreads}>
                                                { getIcon('IconAutorenew', {className: 'fc-load-more-icon'}) }
                                                { getIcon('IconCancel', {className: 'fc-cancel-icon'}) }
                                            </Link>
                                        </div>             
                                    </div>
                                </div>
                                <div className="flex mb-3">
                                    <div className="w-full gap-4 fc-threads-list">{threads}</div>
                                </div> 
                                <div className="fc-navigation flex justify-between items-center justify-center pl-3 pr-3 fc-transparent-color" id="fc-navigation-threads">
                                    <h3></h3>
                                    <UiNavigation scope={navigation.scope} page={navigation.page} total={navigation.total} limit={navigation.limit} />
                                </div>  
                            </div>
                        </div>          
                    </div>                 
                </>
            ): (
                <div className="fc-website-container">
                    <div className="fc-website-not-found">
                        <p>{websiteError}</p>
                    </div>
                </div>
            )}
        </>

    );

}

// Export the page
export default Page;