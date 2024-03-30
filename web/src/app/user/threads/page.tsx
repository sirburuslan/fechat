/*
 * @page Threads
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This file contains the page threads in the user panel
 */

'use client'

// Import the react hooks
import { useState, useRef, useContext, useEffect } from 'react';

// Import the Next JS Link component
import Link from 'next/link';

// Import axios module
import axios, { AxiosError, AxiosResponse } from 'axios';

// Import the UseQuery hook
import { useQuery } from 'react-query';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the incs
import { getIcon, getWord, getToken, showNotification, calculateTime } from '@/core/inc/incIndex';

// Import the types
import { typeToken, typePostHeader } from '@/core/types/typesIndex';

// Import the options for website and member
import {MemberOptionsContext} from '@/core/contexts/OptionsContext';

// Import the Uis
import { UiDropdown, UiNavigation } from '@/core/components/general/ui/UiIndex';

// Import the Confirmation component
import Confirmation from '@/core/components/general/Confirmation';

// Create the page content
const Page = (): React.JSX.Element => {

    // Member options
    let {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);

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

    // Check if document is defined
    if ( typeof document !== 'undefined' ) {

        // Update the page title dynamically
        document.title = getWord('user', 'user_threads', memberOptions['Language']);

    };

    // Create the request for threads
    let threadsList = async (): Promise<any> => {

        // Hide the navigation box
        document.getElementsByClassName('fc-navigation')[0].classList.remove('block');

        // Generate a new csrf token
        let csrfToken: typeToken = await getToken();

        // Check if csrf token is missing
        if ( !csrfToken.success ) {

            // Return error
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

        // Request the threads list
        let response = await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/threads/list', search, headers);
        
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
            setFetchedData(false);

        }, 1000);

    };

    // Request the threads list
    let { isLoading, error, data } = useQuery('threadsList', threadsList, {
        enabled: !fetchedData
    });

    // Run code after component load
    useEffect((): () => void => {

        // Set the bearer token
        let token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');

        // Create a Web Socket connection
        let socket = new WebSocket(process.env.NEXT_PUBLIC_API_URL?.replace('http', 'ws') + 'api/v1/user/websocket');

        // Register an event when the connection opens
        socket.onopen = (event: Event): void => {

            // Prepare the data to send
            let fields = {
                AccessToken: token
            };

            // Send fields as string
            socket.send(JSON.stringify(fields));

        };

        // Catch the messages
        socket.onmessage = (event: MessageEvent<any>): void => {

            // Decode the event data
            let eventData = JSON.parse(event.data);

            // Check if unseen messages exists
            if ( typeof eventData.unseen !== 'undefined' ) {

                // Check if page is 1
                if ( search.Page === 1 ) {

                    // Request the threads list
                    setFetchedData(false);            

                }

            }

        };

        // Register an event for clicks tracking
        document.addEventListener('click', trackClicks);

        return (): void => {

            // Web Socket connection
            socket.close();

            // Remove event for clicks tracking
            document.removeEventListener('click', trackClicks);

        }

    }, []);

    // Monitor the data change
    useEffect((): void => {

        // Check if search is not empty
        if ( document.querySelector('.fc-search-box.fc-search-active') ) {

            // Remove fc-search-active class from search input
            document.getElementsByClassName('fc-search-box')[0]!.classList.remove('fc-search-active');

            // Remove fc-search-complete class to search input
            document.getElementsByClassName('fc-search-box')[0]!.classList.add('fc-search-complete');                    

        }

        // Check if has occurred an error
        if ( error ) {

            // Update threads
            setThreads(<div className="fc-no-threads-found">{(error as Error).message}</div>);

        } else if ( (typeof data != 'undefined') && !data.success ) {

            // Update threads
            setThreads(<div className="fc-no-threads-found">{data.message}</div>);

        } else if (data) {

            // Update threads
            setThreads(data.result.elements.map((thread: any, index: number) => {

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
                            <p>{calculateTime(parseInt(thread.created), parseInt(data.time))}</p>
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
            let limit: number = ((data.result.page * 10) < data.result.total)?(data.result.page * 10):data.result.total;

            // Set text
            document.querySelector('#fc-navigation-threads h3')!.innerHTML = (((data.result.page - 1) * 10) + 1) + '-' + limit + ' ' + getWord('default', 'default_of', memberOptions['Language']) + ' ' + data.result.total + ' ' + getWord('default', 'default_results', memberOptions['Language']);

            // Set page
            navigation.page = data.result.page;                    

            // Set total
            navigation.total = data.result.total;

            // Set pagination
            setNavigation(navigation);

            // Show the navigation box
            document.getElementsByClassName('fc-navigation')[0].classList.add('block');

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
            setFetchedData(false);

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
            setFetchedData(false);

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
            await axios.delete(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/threads/' + threadId, headers)

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
                        setFetchedData(false);

                    } else {

                        // Request the threads list
                        setFetchedData(false);

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

    return (
        <>
            <Confirmation confirmAction={deleteThread}><>{getWord('user', 'user_deleting_thread_permanent', memberOptions['Language'])}</></Confirmation>
            <div className="fc-threads-container">
                <div className="flex mb-3">
                    <div className="w-full">
                        <h2 className="fc-page-title">
                            { getWord('user', 'user_threads', memberOptions['Language'])  }
                        </h2>
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
        </>

    );

}

// Export the page
export default Page;