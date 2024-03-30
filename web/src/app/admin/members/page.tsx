/*
 * @page Members
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-16
 *
 * This file contains the page members in the administrator panel
 */

'use client'

// Import the react hooks
import { useEffect, useContext, useState, useRef, SyntheticEvent  } from 'react';

// Import the Next JS Link component
import Link from 'next/link';

// Import the Next JS Image component
import Image from 'next/image';

// Import axios module
import axios, { AxiosError, AxiosResponse } from 'axios';

// Import the UseQuery hook
import { useQuery } from 'react-query';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the incs
import { getIcon, getWord, getToken, calculateTime, showNotification, unescapeRegexString } from '@/core/inc/incIndex';

// Import types
import { typePostHeader, typeToken } from '@/core/types/typesIndex';

// Import the options for website and member
import { MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Import the Uis
import { UiModal, UiNavigation, UiDropdown } from '@/core/components/general/ui/UiIndex';

// Import the Confirmation component
import Confirmation from '@/core/components/general/Confirmation';

// Create the page content
const Page = (): React.JSX.Element => {

    // Member options
    let {memberOptions} = useContext(MemberOptionsContext);   

    // Modal status
    let [modalStatus, setModalStatus] = useState('');

    // New Member fields
    let [member, setMember] = useState({
        firstName: '',
        lastName: '',
        email: '',
        password: ''
    });

    // Set a hook for errors value
    let [errors, setErrors] = useState({});  

    // Set a hook for search parameters
    let [search, setSearch] = useState({
        Search: '',
        Page: 1
    });   
    
    // Set a hook for navigation
    let [navigation, setNavigation] = useState({
        scope: 'members',
        page: 0,
        total: 0,
        limit: 24
    });  

    // Members list holder
    let [members, setMembers] = useState<React.ReactNode | null>(null);

    // Hook to fetch data with useQuery
    let [fetchedData, setFetchedData] = useState(false);

    // Check if document is defined
    if ( typeof document !== 'undefined' ) {

        // Update the page title dynamically
        document.title = getWord('admin', 'admin_members', memberOptions['Language']);

    }

    // Search pause container
    let searchPause = useRef<NodeJS.Timeout>();

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

    // Create the request for members
    let membersList = async (): Promise<any> => {

        // Hide the navigation box
        document.getElementsByClassName('fc-navigation')[0].classList.remove('block');

        // Generate a new csrf token
        let csrfToken: typeToken = await getToken();

        // Check if csrf token is missing
        if ( !csrfToken.success ) {

            // Return error message
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

        // Request the members list
        let response = await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/members/list', search, headers);

        // Process the response
        return response.data;

    };

    // Request the members list
    let { isLoading, error, data } = useQuery('membersList', membersList, {
        enabled: !fetchedData
    });

    // Run code for client
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

            // Update members
            setMembers(<div className="fc-no-members-found">{(error as Error).message}</div>);

        } else if ( (typeof data != 'undefined') && !data.success ) { 
            
            // Update members
            setMembers(<div className="fc-no-members-found">{data.message}</div>);

        } else if (data) {

            // Update members
            setMembers(data.result.elements.map((member: any, index: number) => {

                // Dropdown items
                let dropdownItems: Array<{text: string, url: string, id: string}> = [{
                    text: getWord('default', 'default_edit', memberOptions['Language']),
                    url: '/admin/members/' + member.memberId,
                    id: 'edit-member'
                }, {
                    text: getWord('default', 'default_delete', memberOptions['Language']),
                    url: '#',
                    id: 'delete-member'
                }];

                return <div className="fc-member" key={'member-' + member.memberId} data-member={member.memberId}>
                    <div className="fc-dashboard-sidebar-body flex justify-between">
                        <div className="flex">
                            <div className="grid grid-cols-1 content-around fc-member-photo">
                                {(member.profilePhoto as string)?(
                                    <Image src={member.profilePhoto} width={40} height={40} alt="Member Photo" onError={(e: SyntheticEvent<HTMLImageElement, Event>): void => {e.currentTarget.src = '/assets/img/cover.png'}} />
                                ): (
                                    <div className="fc-profile-photo-cover">
                                        {getIcon('IconPerson')}
                                    </div>
                                )}
                            </div>
                            <div className="grid grid-cols-1 content-around fc-member-info">
                                <h3>
                                    <Link href={'/admin/members/' + member.memberId}>
                                        {(member.firstName && member.lastName)?(unescapeRegexString(member.firstName + ' ' + member.lastName)):(member.email)}
                                    </Link>
                                </h3>
                                <p>
                                    {(parseInt(member.role) === 0)?(getWord('default', 'default_administrator', memberOptions['Language'])):(getWord('default', 'default_user', memberOptions['Language']))}
                                </p>
                            </div>
                        </div>
                        <div className="grid grid-cols-1 content-around">
                            <button type="button" className="fc-manage-member" onClick={openMemberDropdown}>
                                { getIcon('IconManageAccounts') }
                            </button>
                            <UiDropdown button="" options={ dropdownItems } id={'fc-member-menu-' + member.memberId} menuPosition="fc-dropdown-menu-right" /> 
                        </div>                            
                    </div>
                    <div className="grid grid-cols-1 content-around fc-member-footer">
                        <div className="flex justify-between">
                            <div>
                                { getIcon('IconPersonAddAlt', {className: 'fc-member-last-access-icon'}) }
                                <span>
                                    {calculateTime(parseInt(member.created), parseInt(data.time), memberOptions['Language'])}
                                </span>
                            </div>
                        </div>
                    </div>
                </div>;

            }));

            // Set limit
            let limit: number = ((data.result.page * 24) < data.result.total)?(data.result.page * 24):data.result.total;

            // Set text
            document.querySelector('#fc-navigation-members h3')!.innerHTML = (((data.result.page - 1) * 24) + 1) + '-' + limit + ' ' + getWord('default', 'default_of', memberOptions['Language']) + ' ' + data.result.total + ' ' + getWord('default', 'default_results', memberOptions['Language']);

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
        if ( (target instanceof Element) && target.closest('.fc-dropdown-menu') && (target.nodeName === 'A') && (target.getAttribute('data-id') === 'delete-member') ) {
            e.preventDefault();

            // Get the member's ID
            let memberId: string | null = target.closest('.fc-member')!.getAttribute('data-member');

            // Create a link
            let newLink: HTMLAnchorElement = document.createElement('a');

            // Set the member's ID
            newLink.setAttribute('data-id', memberId!);

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

        } else if ( (target instanceof Element) && target.closest('#fc-navigation-members') && (target.nodeName === 'A') ) {
            e.preventDefault();

            // Get the page
            let page: string | null = target.getAttribute('data-page');

            // Set search value
            search.Page = parseInt(page!);

            // Search for members
            setSearch(search);
                
            // Request the members list
            setFetchedData(false); 

        } else if ( (target instanceof Element) && target.classList && target.classList.contains('fc-export-members-button') ) {
            e.preventDefault();
            
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
    
                // Request the members list for export
                await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/members/export', null, headers)
                
                // Process the response
                .then((response: AxiosResponse): void => {
    
                    // Verify if the response is successfully
                    if ( response.data.success ) {
    
                        // Set the list
                        let list: [string[]] | null = null;

                        // Append to list
                        list = [['"' + getWord('admin', 'admin_member_id', memberOptions['Language']) + '"', '"' + getWord('admin', 'admin_first_name', memberOptions['Language']) + '"', '"' + getWord('admin', 'admin_last_name', memberOptions['Language']) + '"', '"' + getWord('admin', 'admin_email', memberOptions['Language']) + '"', '"' + getWord('admin', 'admin_phone', memberOptions['Language']) + '"', '"' + getWord('admin', 'admin_role', memberOptions['Language']) + '"']];

                        // Total number of members
                        let membersTotal: number = response.data.members.length;

                        // List all numbers
                        for ( let m = 0; m < membersTotal; m++ ) {

                            // Append to list
                            list.push(['"' + response.data.members[m].memberId + '"', '"' + unescapeRegexString(response.data.members[m].firstName) + '"', '"' + unescapeRegexString(response.data.members[m].lastName) + '"', '"' + response.data.members[m].email + '"', '"' + response.data.members[m].phone + '"', '"' + response.data.members[m].role + '"']);

                        }
                        
                        // CSV variable
                        let csv: string = '';

                        // Prepare the csv
                        list!.forEach(function(row) {
                            csv += row.join(',');
                            csv += "\n";
                        });

                        // Create the CSV link and download the file
                        let csv_link: HTMLAnchorElement = document.createElement('a');

                        // Set charset
                        csv_link.href = 'data:text/csv;charset=utf-8,' + encodeURI(csv);

                        // Open in new tab the file
                        csv_link.target = '_blank';

                        // Set the name of the file
                        csv_link.download = 'members.csv';

                        // Download the CSV
                        csv_link.click();
    
                    } else {

                        // Show error notification
                        throw new Error(response.data.message);
    
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
    
                // Check if error is known
                if ( e instanceof Error ) {

                    // Show error notification
                    showNotification('error', e.message);

                } else {

                    // Display in the console the error
                    console.log(e);

                }
    
            }

        }

    };

    /** 
     * Create member form handler
     * 
     * @param FormEvent e 
     */
    let createMember = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
        e.preventDefault();

        // Reset the errors messages
        setErrors({});

        try {

            // Generate a new csrf token
            let csrfToken: typeToken = await getToken();

            // Check if csrf token is missing
            if ( !csrfToken.success ) {

                // Show error notification
                throw new Error(getWord('errors', 'error_csrf_token_not_generated', memberOptions['Language']));

            }

            // Enable the submit button animation
            document.getElementsByClassName('fc-submit-button')[0].classList.add('fc-active-button');

            // Set the bearer token
            let token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');

            // Set the headers
            let headers: typePostHeader = {
                headers: {
                    Authorization: `Bearer ${token}`,
                    CsrfToken: csrfToken.token
                }
            };            

            // Create a new member
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/members', {
                FirstName: member.firstName,
                LastName: member.lastName,
                Email: member.email,
                Password: member.password
            }, headers)

            // Process the response
            .then((response: AxiosResponse) => {

                // Disable the submit button animation
                document.getElementsByClassName('fc-submit-button')[0].classList.remove('fc-active-button');

                // Verify if the response is successfully
                if ( response.data.success ) {
                    
                    // Show success notification
                    showNotification('success', response.data.message);

                    // Reset the form
                    setMember({
                        firstName: '',
                        lastName: '',
                        email: '',
                        password: ''
                    });

                    // Request the members list
                    setFetchedData(false); 

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
            .catch((e: AxiosError): void => {

                // Show error notification
                throw new Error(e.message);

            });          

        } catch (e: unknown) {

            // Disable the submit button animation
            document.getElementsByClassName('fc-submit-button')[0].classList.remove('fc-active-button');            

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
     * Create the function which will open or close dropdown
     * 
     * @param MouseEvent e 
     */
    let openMemberDropdown = (e: React.MouseEvent<HTMLElement>): void => {
        e.preventDefault();

        // Get the current target
        let target: EventTarget & HTMLElement = e.currentTarget;

        // Verify if the dropdown is showed
        if ( target.closest('.content-around')!.getElementsByClassName('fc-dropdown')[0].getAttribute('data-expand') === 'false' ) {

            // Change the dropdown status
            target.closest('.content-around')!.getElementsByClassName('fc-dropdown')[0].setAttribute('data-expand', 'true');

        } else {

            // Change the dropdown status
            target.closest('.content-around')!.getElementsByClassName('fc-dropdown')[0].setAttribute('data-expand', 'false');

        }

    }

    /**
     * Search for members
     * 
     * @param React.ChangeEvent e 
     */
    let searchMembers = (e: React.ChangeEvent<HTMLInputElement>): void => {

        // Add fc-search-active to show the animation
        e.target.closest('.fc-search-box')!.classList.add('fc-search-active');

        // Set search page
        search.Page = 1;

        // Set search value
        search.Search = e.target.value;

        // Search for members
        setSearch(search);

        // Schedule a search
        scheduleSearch((): void => { 
            
            // Request the members list
            setFetchedData(false); 

        }, 1000);

    };

    /**
     * Cancel search for members
     * 
     * @param React.ChangeEvent e 
     */
    let cancelSearchMembers = (e: React.MouseEvent<HTMLAnchorElement>): void => {

        // Add fc-search-complete to hide the animation
        e.currentTarget.closest('.fc-search-box')!.classList.remove('fc-search-complete');

        // Reset search
        search.Search = '';

        // Search for members
        setSearch(search);

        // Set a pause
        setTimeout(() => {

            // Request the members list
            setFetchedData(false); 

        }, 500);

        // Empty the search value
        (document.getElementById('fc-search-for-members') as HTMLInputElement).value = '';



    };

    /**
     * Delete member
     * 
     * @param string memberId
     */
    const deleteMember = async (id: string): Promise<void> => {

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

            // Delete member
            await axios.delete(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/members/' + id, headers)

            // Process the response
            .then((response: AxiosResponse) => {

                // Verify if the response is successfully
                if ( response.data.success ) {
                    
                    // Show success notification
                    showNotification('success', response.data.message);

                    // Check if this was the last member in the list
                    if ( document.querySelectorAll('.fc-members-list > .fc-member').length < 2 ) {

                        // Set previous page
                        search.Page = (search.Page > 1)?(search.Page - 1):search.Page;

                        // Search for members
                        setSearch(search);

                        // Request the members list
                        setFetchedData(false); 

                    } else {

                        // Request the members list
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

    let newMember = (): React.JSX.Element => {

        return (
            <form onSubmit={createMember} id="fc-create-member-form">            
                <div className="col-span-full fc-modal-text-input">
                    <input
                        type="text"
                        placeholder={getWord('default', 'default_enter_first_name', memberOptions['Language'])}
                        value={member.firstName}
                        name="fc-modal-text-input-first-name"
                        id="fc-modal-text-input-first-name"
                        className="block px-2.5 pb-2.5 pt-4 w-full fc-modal-form-input"
                        onChange={(e: React.ChangeEvent<HTMLInputElement>): void => setMember({...member, firstName: e.target.value})}
                        required
                    />
                    <label
                        htmlFor="fc-modal-text-input-first-name"
                        className="absolute"
                    >
                        { getIcon('IconPersonCheckFill') }
                    </label>
                    <div className={(typeof (errors as {[key: string]: string}).FirstName !== 'undefined')?'fc-modal-form-input-error-message fc-modal-form-input-error-message-show':'fc-modal-form-input-error-message'}>
                        {(typeof (errors as {[key: string]: string}).FirstName !== 'undefined')?(errors as {[key: string]: string}).FirstName:''}
                    </div>
                </div>
                <div className="col-span-full fc-modal-text-input">
                    <input
                        type="text"
                        placeholder={getWord('default', 'default_enter_last_name', memberOptions['Language'])}
                        value={member.lastName}
                        name="fc-modal-text-input-last-name"
                        id="fc-modal-text-input-last-name"
                        className="block px-2.5 pb-2.5 pt-4 w-full fc-modal-form-input"
                        onChange={(e: React.ChangeEvent<HTMLInputElement>): void => setMember({...member, lastName: e.target.value})}
                        required
                    />
                    <label
                        htmlFor="fc-modal-text-input-last-name"
                        className="absolute"
                    >
                        { getIcon('IconPersonCheckFill') }
                    </label>
                    <div className={(typeof (errors as {[key: string]: string}).LastName !== 'undefined')?'fc-modal-form-input-error-message fc-modal-form-input-error-message-show':'fc-modal-form-input-error-message'}>
                        {(typeof (errors as {[key: string]: string}).LastName !== 'undefined')?(errors as {[key: string]: string}).LastName:''}
                    </div>
                </div>
                <div className="col-span-full fc-modal-text-input">
                    <input
                        type="email"
                        placeholder={getWord('default', 'default_enter_email_address', memberOptions['Language'])}
                        value={member.email}
                        name="fc-modal-text-input-email"
                        id="fc-modal-text-input-email"
                        className="block px-2.5 pb-2.5 pt-4 w-full fc-modal-form-input"
                        onChange={(e: React.ChangeEvent<HTMLInputElement>): void => setMember({...member, email: e.target.value})}
                        autoComplete="email"
                        required
                    />
                    <label
                        htmlFor="fc-modal-text-input-email"
                        className="absolute"
                    >
                        { getIcon('IconAt') }
                    </label>
                    <div className={(typeof (errors as {[key: string]: string}).Email !== 'undefined')?'fc-modal-form-input-error-message fc-modal-form-input-error-message-show':'fc-modal-form-input-error-message'}>
                        {(typeof (errors as {[key: string]: string}).Email !== 'undefined')?(errors as {[key: string]: string}).Email:''}
                    </div>
                </div>
                <div className="col-span-full fc-modal-text-input">
                    <input
                        type="password"
                        placeholder={getWord('default', 'default_enter_password', memberOptions['Language'])}
                        value={member.password}
                        name="fc-modal-text-input-password"
                        id="fc-modal-text-input-password"
                        className="block px-2.5 pb-2.5 pt-4 w-full fc-modal-form-input"
                        onChange={(e: React.ChangeEvent<HTMLInputElement>): void => setMember({...member, password: e.target.value})}
                        autoComplete="current-password"
                        required
                    />
                    <label
                        htmlFor="fc-modal-text-input-password"
                        className="absolute"
                    >
                        { getIcon('IconKeyFill') }
                    </label>
                    <div className={(typeof (errors as {[key: string]: string}).Password !== 'undefined')?'fc-modal-form-input-error-message fc-modal-form-input-error-message-show':'fc-modal-form-input-error-message'}>
                        {(typeof (errors as {[key: string]: string}).Password !== 'undefined')?(errors as {[key: string]: string}).Password:''}
                    </div>
                </div>                
                <div className="col-span-full fc-modal-button">
                    <div className="text-right">
                        <button type="submit" className="mb-3 flex justify-between fc-option-violet-btn fc-submit-button">
                            { getWord('admin', 'admin_save_plan', memberOptions['Language']) }
                            { getIcon('IconAutorenew', {className: 'fc-load-more-icon'}) }
                            { getIcon('IconArrowForward', {className: 'fc-next-icon'}) }
                        </button>
                    </div>
                </div> 
            </form>
        );

    };

    return (
        <>
            <UiModal size="fc-modal-md" title={getWord('admin', 'admin_new_member', memberOptions['Language'])} status={modalStatus} updateStatus={setModalStatus}>{newMember()}</UiModal>
            <Confirmation confirmAction={deleteMember}><>{getWord('admin', 'admin_deleting_member_permanent', memberOptions['Language'])}</></Confirmation>
            <div className="fc-members-container">
                <div className="flex mb-3">
                    <div className="w-full">
                        <h2 className="fc-page-title">
                            { getWord('admin', 'admin_members', memberOptions['Language'])  }
                        </h2>
                    </div>
                </div>
                <div className="flex mb-3">
                    <div className="w-full flex">
                        <div className="flex fc-search-box fc-transparent-color">
                            <span className="fc-search-icon">
                                { getIcon('IconSearch') }
                            </span>
                            <input type="text" placeholder={ getWord('admin', 'admin_search_for_members', memberOptions['Language']) } className="form-control fc-search-input" id="fc-search-for-members" onInput={searchMembers} />
                            <Link href="#" onClick={cancelSearchMembers}>
                                { getIcon('IconAutorenew', {className: 'fc-load-more-icon'}) }
                                { getIcon('IconCancel', {className: 'fc-cancel-icon'}) }
                            </Link>
                        </div>
                        <button className="ec-search-btn fc-new-member-button" onClick={(): void => setModalStatus('fc-modal-show')}>
                            { getIcon('IconPersonAddAlt') }
                            { getWord('admin', 'admin_new_member', memberOptions['Language'])  }
                        </button>
                        <button className="ec-search-btn fc-export-members-button">
                            { getIcon('IconCloudDownload') }
                            { getWord('admin', 'admin_export', memberOptions['Language'])  }
                        </button>                    
                    </div>
                </div>  
                <div className="flex mb-3">
                    <div className="w-full grid md:grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-4 fc-members-list">{members}</div>
                </div> 
                <div className="fc-navigation flex justify-between items-center justify-center pl-3 pr-3 fc-transparent-color" id="fc-navigation-members">
                    <h3></h3>
                    <UiNavigation scope={navigation.scope} page={navigation.page} total={navigation.total} limit={navigation.limit} />
                </div>          
            </div>
        </>
    );

};

// Export the page content
export default Page;