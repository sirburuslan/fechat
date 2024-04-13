/*
 * @page Websites
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This file contains the page websites in the user panel
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
import { getIcon, getWord, showNotification } from '@/core/inc/incIndex';

// Import the types
import { typePostHeader } from '@/core/types/typesIndex';

// Import the options for website and member
import {MemberOptionsContext} from '@/core/contexts/OptionsContext';

// Import the Uis
import { UiDropdown, UiModal, UiNavigation } from '@/core/components/general/ui/UiIndex';

// Import the Confirmation component
import Confirmation from '@/core/components/general/Confirmation';

// Create the page content
const Page = (): React.JSX.Element => {

    // Member options
    const {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);

    // Modal status
    const [modalStatus, setModalStatus] = useState('');

    // New website fields
    const [website, setWebsite] = useState({
        name: '',
        url: ''
    });

    // Set a hook for errors value
    const [errors, setErrors] = useState({});

    // Set a hook for search parameters
    const [search, setSearch] = useState({
        Search: '',
        Page: 1
    });   
    
    // Set a hook for navigation
    const [navigation, setNavigation] = useState({
        scope: 'websites',
        page: 1,
        total: 0,
        limit: 10
    });  

    // Websites list holder
    const [websites, setWebsites] = useState<React.ReactNode | null>(null);

    // Hook to fetch data with useQuery
    const [fetchedData, setFetchedData] = useState(false);

    // Search pause container
    const searchPause = useRef<NodeJS.Timeout>();

    // Check if document is defined
    if ( typeof document !== 'undefined' ) {

        // Update the page title dynamically
        document.title = getWord('default', 'default_websites', memberOptions['Language']);

    };

    // Create the request for websites
    const websitesList = async (): Promise<any> => {

        // Hide the navigation box
        document.getElementsByClassName('fc-navigation')[0].classList.remove('block');

        // Set the bearer token
        const token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');

        // Set the headers
        const headers: typePostHeader = {
            headers: {
                Authorization: `Bearer ${token}`
            }
        };

        // Request the websites list
        const response = await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/websites/list', search, headers)
        
        // Process the response
        return response.data;

    };


    // Content for the New Website modal
    const newWebsite = (): React.JSX.Element => {

        return (
            <form id="fc-create-member-form" onSubmit={saveWebsite}>            
                <div className="col-span-full fc-modal-text-input">
                    <input
                        type="text"
                        placeholder={getWord('user', 'user_enter_website_name', memberOptions['Language'])}
                        value={website.name}
                        name="fc-modal-text-input-website-name"
                        id="fc-modal-text-input-website-name"
                        className="block px-2.5 pb-2.5 pt-4 w-full fc-modal-form-input"
                        onChange={(e: React.ChangeEvent<HTMLInputElement>): void => setWebsite({...website, name: e.target.value})}
                        required
                    />
                    <label
                        htmlFor="fc-modal-text-input-website-name"
                        className="absolute"
                    >
                        { getIcon('IconDriveFileRenameOutline') }
                    </label>
                    <div className={(typeof (errors as {[key: string]: string}).WebsiteName !== 'undefined')?'fc-modal-form-input-error-message fc-modal-form-input-error-message-show':'fc-modal-form-input-error-message'}>
                        {(typeof (errors as {[key: string]: string}).WebsiteName !== 'undefined')?(errors as {[key: string]: string}).WebsiteName:''}
                    </div>
                </div>
                <div className="col-span-full fc-modal-text-input">
                    <input
                        type="text"
                        placeholder={getWord('user', 'user_enter_website_url', memberOptions['Language'])}
                        value={website.url}
                        name="fc-modal-text-input-website-url"
                        id="fc-modal-text-input-website-url"
                        className="block px-2.5 pb-2.5 pt-4 w-full fc-modal-form-input"
                        onChange={(e: React.ChangeEvent<HTMLInputElement>): void => setWebsite({...website, url: e.target.value})}
                        required
                    />
                    <label
                        htmlFor="fc-modal-text-input-website-name"
                        className="absolute"
                    >
                        { getIcon('IconLink') }
                    </label>
                    <div className={(typeof (errors as {[key: string]: string}).Url !== 'undefined')?'fc-modal-form-input-error-message fc-modal-form-input-error-message-show':'fc-modal-form-input-error-message'}>
                        {(typeof (errors as {[key: string]: string}).Url !== 'undefined')?(errors as {[key: string]: string}).Url:''}
                    </div>
                </div>            
                <div className="col-span-full fc-modal-button">
                    <div className="text-right">
                        <button type="submit" className="mb-3 flex justify-between fc-option-violet-btn fc-submit-button">
                            { getWord('user', 'user_save_website', memberOptions['Language']) }
                            { getIcon('IconAutorenew', {className: 'fc-load-more-icon ml-3'}) }
                            { getIcon('IconArrowForward', {className: 'fc-next-icon ml-3'}) }
                        </button>
                    </div>
                </div> 
            </form>
        );

    };

    /*
     * Schedule a search
     * 
     * @param funcion fun contains the function
     * @param integer interval contains time
     */
    const scheduleSearch = ($fun: () => void, interval: number): void => {

        // Verify if an event was already scheduled
        if (searchPause.current) {

            // Clear the previous timeout
            clearTimeout(searchPause.current);

        }

        // Add to queue
        searchPause.current = setTimeout($fun, interval);
        
    };

    // Request the websites list
    const { isLoading, error, data } = useQuery('websitesList', websitesList, {
        enabled: !fetchedData
    });

    // Run code after component load
    useEffect((): () => void => {

        // Register an event for clicks tracking
        document.addEventListener('click', trackClicks);

        return (): void => {

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

            // Update websites
            setWebsites(<div className="fc-no-websites-found">{(error as Error).message}</div>);

        } else if ( (typeof data != 'undefined') && !data.success ) {

            // Update websites
            setWebsites(<div className="fc-no-websites-found">{data.message}</div>);

        } else if (data) {

            // Update websites
            setWebsites(data.result.elements.map((website: any, index: number) => {

                // Dropdown items
                const dropdownItems: Array<{text: string, url: string, id: string}> = [{
                    text: getWord('default', 'default_edit', memberOptions['Language']),
                    url: '/user/websites/' + website.websiteId,
                    id: 'edit-website'
                }, {
                    text: getWord('default', 'default_delete', memberOptions['Language']),
                    url: '#',
                    id: 'delete-website'
                }];

                return (
                    <div className="fc-website flex justify-between" key={index} data-website={website.websiteId}>
                        <div>
                            <h4>
                                <Link href={'/user/websites/' + website.websiteId}>
                                    {website.name}
                                </Link>
                            </h4>
                            {(website.enabled > 0)?(
                                <p className="fc-chat-enabled">{getWord('default', 'default_enabled', memberOptions['Language'])}</p>
                            ):(
                                <p className="fc-chat-disabled">{getWord('default', 'default_disabled', memberOptions['Language'])}</p>
                            )}
                        </div>
                        <div className="text-center">
                        </div>                        
                        <div className="text-right">
                            <button type="button" className="fc-manage-website" onClick={ websiteDropdown }>
                                { getIcon('IconMoreHoriz') }
                            </button>
                            <UiDropdown button="" options={ dropdownItems } id={'fc-website-menu-' + website.websiteId} />
                        </div>
                    </div>
                );

            }));

            // Set limit
            const limit: number = ((data.result.page * 10) < data.result.total)?(data.result.page * 10):data.result.total;

            // Set text
            document.querySelector('#fc-navigation-websites h3')!.innerHTML = (((data.result.page - 1) * 10) + 1) + '-' + limit + ' ' + getWord('default', 'default_of', memberOptions['Language']) + ' ' + data.result.total + ' ' + getWord('default', 'default_results', memberOptions['Language']);

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
    const trackClicks = async (e: Event): Promise<void> => {

        // Get the target
        const target = e.target;

        // Check if the click is inside dropdown
        if ( (target instanceof Element) && target.closest('.fc-dropdown-menu') && (target.nodeName === 'A') && (target.getAttribute('data-id') === 'delete-website') ) {
            e.preventDefault();

            // Get the website's ID
            const websiteId: string | null = target.closest('.fc-website')!.getAttribute('data-website');

            // Create a link
            const newLink: HTMLAnchorElement = document.createElement('a');

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

        } else if ( (target instanceof Element) && target.closest('#fc-navigation-websites') && (target.nodeName === 'A') ) {
            e.preventDefault();

            // Get the page
            const page: string | null = target.getAttribute('data-page');

            // Set search value
            search.Page = parseInt(page!);

            // Search for websites
            setSearch(search);

            // Request the websites list
            setFetchedData(false); 

        }

    };

    /**
     * Search for websites
     * 
     * @param React.ChangeEvent e 
     */
    const searchWebsites = (e: React.ChangeEvent<HTMLInputElement>): void => {

        // Add fc-search-active to show the animation
        e.target.closest('.fc-search-box')!.classList.add('fc-search-active');

        // Set search page
        search.Page = 1;

        // Set search value
        search.Search = e.target.value;

        // Search for websites
        setSearch(search);

        // Schedule a search
        scheduleSearch((): void => { 
            
            // Request the websites list
            setFetchedData(false); 

        }, 1000);

    };

    /**
     * Cancel search for websites
     * 
     * @param React.ChangeEvent e 
     */
    const cancelSearchWebsites = (e: React.MouseEvent<HTMLAnchorElement>): void => {

        // Add fc-search-complete to hide the animation
        e.currentTarget.closest('.fc-search-box')!.classList.remove('fc-search-complete');

        // Reset search
        search.Search = '';

        // Search for websites
        setSearch(search);

        // Set a pause
        setTimeout(() => {

            // Request the websites list
            setFetchedData(false); 

        }, 500);

        // Empty the search value
        (document.getElementById('fc-search-for-websites') as HTMLInputElement).value = '';



    };

    /**
     * Delete website
     * 
     * @param string websiteId
     */
    const deleteWebsite = async (id: string): Promise<void> => {

        try {

            // Set the bearer token
            const token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');

            // Set the headers
            const headers: typePostHeader = {
                headers: {
                    Authorization: `Bearer ${token}`
                }
            }; 

            // Delete website
            await axios.delete(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/websites/' + id, headers)

            // Process the response
            .then((response: AxiosResponse) => {

                // Verify if the response is successfully
                if ( response.data.success ) {
                    
                    // Show success notification
                    showNotification('success', response.data.message);

                    // Check if this was the last website in the list
                    if ( document.querySelectorAll('.fc-websites-list > .fc-website').length < 2 ) {

                        // Set previous page
                        search.Page = (search.Page > 1)?(search.Page - 1):search.Page;

                        // Search for websites
                        setSearch(search);

                        // Request the websites list
                        setFetchedData(false); 

                    } else {

                        // Request the websites list
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

    /**
     * Create the function which will handle the website's menu click
     * 
     * @param Event e 
     */
    const websiteDropdown = (e: React.MouseEvent<HTMLElement>): void => {
        e.preventDefault();

        // Get target
        const target = e.target as HTMLElement;

        // Select a dropdown
        const dropdown: HTMLCollectionOf<Element> = target.closest('div')!.getElementsByClassName('fc-dropdown');

        // Verify if the dropdown is open
        if ( dropdown[0].getAttribute('data-expand') === 'false' ) {

            // Set true
            dropdown[0].setAttribute('data-expand', 'true');

            // Get menu
            const menu: Element = dropdown[0]!.getElementsByClassName('fc-dropdown-menu')[0];

            // Get the height
            const height: number = menu.clientHeight;

            // Calculate the height of the button
            const button_height: number = target.offsetHeight;

            // Set transformation
            (menu as HTMLElement).style.transform = `translate3d(0, -${button_height + height}px, 0)`;
            
        } else {

            // Set false
            dropdown[0].setAttribute('data-expand', 'false');
            
        }

    }

    /** 
     * Save website form handler
     * 
     * @param FormEvent e 
     */
    const saveWebsite = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
        e.preventDefault();

        // Reset the errors messages
        setErrors({});

        try {

            // Set the bearer token
            const token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');

            // Set the headers
            const headers: typePostHeader = {
                headers: {
                    Authorization: `Bearer ${token}`
                }
            };            

            // Save the website request
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/websites', {
                Name: website.name,
                Url: website.url
            }, headers)

            // Process the response
            .then((response: AxiosResponse) => {

                // Verify if the response is successfully
                if ( response.data.success ) {
                    
                    // Show success notification
                    showNotification('success', response.data.message);

                    // Reset the form
                    setWebsite({
                        name: '',
                        url: ''
                    });

                    // Request the websites list
                    setFetchedData(false); 

                } else if ( typeof response.data.message !== 'undefined' ) {

                    // Run error notification
                    throw new Error(response.data.message);

                } else {

                    // Keys container
                    const keys: string[] = Object.keys(response.data);

                    // Count the keys
                    const keysTotal: number = keys.length;

                    // Check if keys exists
                    if ( keysTotal > 0 ) {

                        // Errors container
                        const errorsHolder: {[key: string]: string} = {};

                        // List the keys
                        for ( let e = 0; e < keysTotal; e++ ) {

                            // Save error in the container
                            errorsHolder[keys[e]] = response.data[keys[e]][0];

                        }

                        // Update the errors
                        setErrors(errorsHolder);

                    }

                    // Run error notification
                    throw new Error(getWord('user', 'user_website_not_saved', memberOptions['Language']));                    

                }

            })

            // Catch the error message
            .catch((e: AxiosError): void => {

                // Show error notification
                throw new Error(e.message);

            });

        } catch ( e: unknown ) {

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
            <UiModal size="fc-modal-md" title={getWord('user', 'user_new_website', memberOptions['Language'])} status={modalStatus} updateStatus={setModalStatus}>{newWebsite()}</UiModal>
            <Confirmation confirmAction={deleteWebsite}><>{getWord('user', 'user_deleting_website_permanent', memberOptions['Language'])}</></Confirmation>
            <div className="fc-websites-container">
                <div className="flex mb-3">
                    <div className="w-full">
                        <h2 className="fc-page-title">
                            { getWord('default', 'default_websites', memberOptions['Language'])  }
                        </h2>
                    </div>
                </div>
                <div className="flex mb-3">
                    <div className="w-full flex">
                        <div className="flex fc-search-box fc-transparent-color">
                            <span className="fc-search-icon">
                                { getIcon('IconSearch') }
                            </span>
                            <input type="text" className="form-control fc-search-input" placeholder={ getWord('user', 'user_search_for_websites', memberOptions['Language']) } id="fc-search-for-websites" onInput={searchWebsites} />
                            <Link href="#" onClick={cancelSearchWebsites}>
                                { getIcon('IconAutorenew', {className: 'fc-load-more-icon'}) }
                                { getIcon('IconCancel', {className: 'fc-cancel-icon'}) }
                            </Link>
                        </div>
                        <button className="ec-search-btn fc-new-website-button" onClick={(): void => setModalStatus('fc-modal-show')}>
                            { getIcon('IconAddToQueue') }
                            { getWord('user', 'user_new_website', memberOptions['Language'])  }
                        </button>                
                    </div>
                </div>
                <div className="flex mb-3">
                    <div className="w-full gap-4 fc-websites-list">{websites}</div>
                </div> 
                <div className="fc-navigation flex justify-between items-center justify-center pl-3 pr-3 fc-transparent-color" id="fc-navigation-websites">
                    <h3></h3>
                    <UiNavigation scope={navigation.scope} page={navigation.page} total={navigation.total} limit={navigation.limit} />
                </div>
            </div>
        </>

    );

}

// Export the page
export default Page;