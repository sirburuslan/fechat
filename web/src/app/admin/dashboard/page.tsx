/*
 * @page Dashboard
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-11
 *
 * This file contains the page dashboard in the administrator panel
 */

'use client'

// Import the react hooks
import { useEffect, useState, useRef, useContext, SyntheticEvent } from 'react';

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

// Import the Chart Line Widget
import ChartLineWidget from '@/core/components/admin/dashboard/ChartLineWidget';

// Import the incs
import { getIcon, getWord, getToken, getMonth, showNotification, calculateTime, unescapeRegexString } from '@/core/inc/incIndex';

// Import the types
import { typeToken, typePostHeader } from '@/core/types/typesIndex';

// Import the options for website and member
import {MemberOptionsContext} from '@/core/contexts/OptionsContext';

// Import the ui components
import {UiCalendar} from '@/core/components/general/ui/UiIndex';

// Create the page content
const Page = (): React.JSX.Element => {

    // Member options
    let {memberOptions} = useContext(MemberOptionsContext);

    // Set a hook for error message if dashboard can't be reached
    let [dashboardError, setDashboardError] = useState('');

    // Set a hook for dashboard content
    let [dashboardContent, setDashboardContent] = useState<{events: Array<{[key: string]: string}>, members: Array<{[key: string]: string}>, transactions: Array<{[key: string]: string}>, time: number}>({
        events: [],
        members: [],
        transactions: [],
        time: 0
    });

    // Register default value for year
    let [iYear, setIYear] = useState(new Date().getFullYear());

    // Register default value for month
    let [iMonth, setIMonth] = useState(new Date().getMonth()); 

    // Register default value for date
    let [iDate, setIDate] = useState(new Date().getDate()); 

    // Hook to fetch data with useQuery
    let [fetchedData, setFetchedData] = useState(false);

    // Init the reference for the calendar
    let calendarBtn = useRef<HTMLDivElement>(null);

    // Check if document is defined
    if ( typeof document !== 'undefined' ) {

        // Update the page title dynamically
        document.title = getWord('admin', 'admin_dashboard', memberOptions['Language']);

    };

    // Verify if members chart time exists
    if ( (typeof document !== 'undefined') && (typeof memberOptions.MembersChartTime === 'string') ) {

        // Get the dropown link
        let dropdownLink: Node | null = document.querySelector('#fc-member-members-chart-menu a[data-id="' + memberOptions.MembersChartTime + '"]');

        // Check if dropdown link exists
        if ( dropdownLink ) {

            // Set the dropown link
            document.getElementsByClassName('fc-dashboard-widget-dropdown-text')[0].textContent = dropdownLink.textContent;

        }
        
    };

    // Create the data sets for chart
    let datasets: string[] = [getWord('admin', 'admin_members', memberOptions['Language'])];

    // Get content for dashboard
    let dashboardData = async (): Promise<any> => {

        // Generate a new csrf token
        let csrfToken: typeToken = await getToken();

        // Check if csrf token is missing
        if ( !csrfToken.success ) {

            // Show error notification
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

        // Request the plans list
        let response = await axios.get(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/dashboard', headers);
        
        // Process the response
        return response.data;

    };

    // Load events from the database
    let eventsList = async (): Promise<void> => {

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
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/events/list', {
                Year: iYear.toString(),
                Month: iMonth.toString(),
                Date: iDate.toString()
            }, headers)

            // Process the response
            .then(async (response: AxiosResponse) => {

                // Verify if the response is successfully
                if ( response.data.success ) {
                    
                    // Update the dashboard content
                    setDashboardContent(prev => ({
                        ...prev,
                        events: response.data.events
                    }));

                } else {

                    // Update the dashboard content
                    setDashboardContent(prev => ({
                        ...prev,
                        events: []
                    }));

                }

            })

            // Catch the error message
            .catch((error: AxiosError): void => {

                // Check if error message exists
                if ( typeof error.message !== 'undefined' ) {

                    // Show error notification
                    throw new Error(error.message);                    

                }
                

            });

        } catch (error: unknown) {

            // Check if error is known
            if ( error instanceof Error ) {

                // Display in the console the error
                console.log(error.message);

            } else {

                // Display in the console the error
                console.log(error);

            }

        }

    };

    // Get members when the administrator changes the time preferences
    let dashboardMembers = async (): Promise<void> => {

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
            await axios.get(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/dashboard/members', headers)
            
            // Process the response
            .then((response: AxiosResponse): void => {
                
                // Verify if the response is successfully
                if ( response.data.success ) {
                    
                    // Check if members exists
                    if ( typeof response.data.members !== 'undefined' ) {

                        // Update the dashboard content
                        setDashboardContent(prev => ({
                            ...prev,
                            members: response.data.members
                        }));
                        
                    }

                } else {

                    // Set error notification
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

                // Set dashboard error message
                setDashboardError(e.message);

            } else {

                // Display the error in the browser's console
                console.log(e);
                
            }

        }

    };

    // Request the dashboard content
    let { isLoading, error, data } = useQuery('dashboardData', dashboardData, {
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

    // Run code for client
    useEffect((): void => {

        // Check if has occurred an error
        if ( error ) {

            // Set dashboard error message
            setDashboardError((error as Error).message);

        } else if ( (typeof data != 'undefined') && !data.success ) { 
            
            // Set dashboard error message
            setDashboardError(data.message);

        } else if (data) {

            // Save server time
            setDashboardContent(prev => ({
                ...prev,
                time: data.time
            }));                    

            // Check if events exists
            if ( typeof data.dashboard.events !== 'undefined' ) {

                // Update the dashboard content
                setDashboardContent(prev => ({
                    ...prev,
                    events: data.dashboard.events
                }));
                
            }

            // Check if members exists
            if ( typeof data.dashboard.members !== 'undefined' ) {

                // Update the dashboard content
                setDashboardContent(prev => ({
                    ...prev,
                    members: data.dashboard.members
                }));
                
            }

            // Check if transactions exists
            if ( typeof data.dashboard.transactions !== 'undefined' ) {

                // Update the dashboard content
                setDashboardContent(prev => ({
                    ...prev,
                    transactions: data.dashboard.transactions
                }));
                
            }

        } 

        // Stop to fetch data
        setFetchedData(true); 
    
    }, [data]);

    // Monitor when the date is changed
    useEffect((): void => {

        // Get the events
        eventsList();

    }, [iDate, iMonth, iYear]);

    /**
     * Track any click
     * 
     * @param Event e
     */
    let trackClicks = async (e: Event): Promise<void> => {

        // Get the target
        let target = e.target;

        // Check if the click is inside dropdown
        if ( (target instanceof Element) && target.closest('#fc-member-members-chart-menu') && (target.nodeName === 'A') ) {
            e.preventDefault();

            // Get menu item text
            let menuText = target.textContent;

            try {

                // Set the bearer token
                let token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');
    
                // Generate a new csrf token
                let csrfToken: typeToken = await getToken();
    
                // Check if csrf token is missing
                if ( !csrfToken.success ) {
    
                    // Show error notification
                    throw new Error(getWord('errors', 'error_csrf_token_not_generated', memberOptions['Language']));
    
                }
    
                // Set the post's fields
                let fields: {
                    [key: string]: string | number | null
                } = {
                    MembersChartTime: target.getAttribute('data-id')
                };
    
                // Set the headers
                let headers: typePostHeader = {
                    headers: {
                        Authorization: `Bearer ${token}`,
                        CsrfToken: csrfToken.token
                    }
                };
    
                // Run the request
                await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/members/options', fields, headers)
    
                // Process the response
                .then((response: AxiosResponse) => {

                    // Verify if the response is successfully
                    if ( response.data.success ) {

                        // Request the dashboard members
                        dashboardMembers();

                        // Set a pause
                        setTimeout((): void => {

                            // Set the dropown link
                            document.getElementsByClassName('fc-dashboard-widget-dropdown-text')[0].textContent = menuText;

                        }, 300);

                    }

                    // Verify if an error has been occurred
                    if ( response.data.MembersChartTime ) {
    
                        // Throw error
                        throw new Error(response.data.MembersChartTime);
                        
                    }
    
                })
    
                // Catch the error
                .catch((error: AxiosError) => {
    
                    // Show error notification
                    throw new Error(error.message);
    
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

        } else if ( (target instanceof Element) && !target.closest('.fc-events-calendar') ) {

            // Change the calendar status
            calendarBtn.current!.closest('.fc-events-calendar')!.setAttribute('data-expand', 'false');

        } else if ( (target instanceof Element) && target.closest('.fc-calendar-selected-date') && (target.nodeName === 'A') ) {

            // Get active date
            let activeDate = new Date(target.getAttribute('data-date')!);
            
            // Set date
            setIDate(activeDate.getDate());

            // Set month
            setIMonth(activeDate.getMonth());    
            
            // Set year
            setIYear(activeDate.getFullYear());
            
        }

    };

    /**
     * Catch the calendar button click
     * 
     * @param Event e
     */
    let calendarButtonClick = (e: React.MouseEvent<Element>): void => {
        e.preventDefault();

        // Get the target
        let target = e.target as HTMLElement;

        // Verify if the calendar is open
        if ( target.closest('.fc-events-calendar')!.getAttribute('data-expand') === 'false' ) {

            // Set expand true
            target.closest('.fc-events-calendar')!.setAttribute('data-expand', 'true');            

        } else {

            // Set expand false
            target.closest('.fc-events-calendar')!.setAttribute('data-expand', 'false');

        }

    };

    // Detect previous date click
    let prevDate = (): void => {
        
        // Get next date
        let nextDate = new Date(new Date((iMonth + 1) + '/' + iDate + '/' + iYear).getTime() - (86400 * 1000));
        
        // Set date
        setIDate(nextDate.getDate());

        // Set month
        setIMonth(nextDate.getMonth());    
        
        // Set year
        setIYear(nextDate.getFullYear());

    };

    // Detect next date click
    let nextDate = (): void => {

        // Get next date
        let nextDate = new Date(new Date((iMonth + 1) + '/' + iDate + '/' + iYear).getTime() + (86400 * 1000));
        
        // Set date
        setIDate(nextDate.getDate());

        // Set month
        setIMonth(nextDate.getMonth());    
        
        // Set year
        setIYear(nextDate.getFullYear());  

    };  

    return (
        <>
            {(dashboardError == '')?(
            <div className="grid md:grid-cols-1 lg:grid-cols-3 gap-4 fc-dashboard-container">
                <div className="fc-events-section">
                    <div className="flex mb-3">
                        <div className="w-full">
                            <h2 className="fc-page-title">
                                { getWord('admin', 'admin_events', memberOptions['Language'])  }
                            </h2>
                        </div>
                    </div>
                    <div className="flex">
                        <div className="w-full fc-events">
                            <div className="fc-events-header flex">
                                <div className="fc-events-date-navigator fc-transparent-color">
                                    <div className="flex">
                                        <div className="flex-none">
                                            <button type="button" className="fc-events-date-previous-date" onClick={prevDate}>
                                                { getIcon('IconChevronLeft') }
                                            </button>
                                        </div>
                                        <div className="grow h-14 text-center">
                                            <h4>
                                                { iDate.toString().padStart(2, '0') } { getMonth((iMonth + 1).toString().padStart(2, '0')) } { iYear }
                                            </h4>
                                        </div>
                                        <div className="flex-none">
                                            <button type="button" className="fc-events-date-next-date" onClick={nextDate}>
                                                { getIcon('IconChevronRight') }
                                            </button>
                                        </div>
                                    </div>
                                </div>
                                <div className="fc-events-calendar flex justify-between fc-transparent-color" data-expand="false">
                                    { getIcon('IconCalendarMonth') }
                                    <button type="button" className="fc-events-calendar-open" onClick={calendarButtonClick}>
                                        { getIcon('IconExpandMore', {className: 'fc-dropdown-arrow-down-icon'}) }
                                    </button>
                                    <div className="fc-events-calendar-container" ref={calendarBtn}>
                                        <UiCalendar date={iDate.toString().padStart(2, '0')} month={(iMonth + 1).toString().padStart(2, '0')} year={iYear.toString()} />
                                    </div>
                                </div>                            
                            </div>
                            <div className="fc-events-body flex">
                                <div className="fc-events-list">
                                    {(dashboardContent.events.length > 0)?(dashboardContent.events.map((event: {[key: string]: string | number}, index: Number): React.JSX.Element => {

                                        // Verify if typeID is 1
                                        if ( event.typeId === 1 ) {

                                            return (<div className="justify-start items-start p-3 fc-event fc-event-new-member flex" key={event.eventId}>
                                                {(typeof event.profilePhoto === 'string')?(
                                                    <Image className="h-10 w-10 rounded-full" width={40} height={40} src={event.profilePhoto as string} alt="Member Photo"  onError={(e: SyntheticEvent<HTMLImageElement, Event>): void => {e.currentTarget.src = '/assets/img/cover.png'}} />
                                                ): (
                                                    <div className="flex fc-event-icon items-center justify-center">
                                                        { getIcon('IconPerson') }
                                                    </div>
                                                )}
                                                <div className="fc-event-text">
                                                    <h4>
                                                        <Link href={'/admin/members/' + event.memberId}>{(event.firstName !== '')?event.firstName + ' ' + event.lastName:'#' + event.memberId}</Link> {getWord('admin', 'admin_has_joined', memberOptions['Language'])} {getWord('default', 'default_website_name', memberOptions['Language'])}.
                                                    </h4>
                                                    <h6>{calculateTime(dashboardContent.time, event.created as number, memberOptions['Language'])}</h6>
                                                </div>                                    
                                            </div>);

                                        }

                                        return (<div className="justify-start items-start p-3 fc-event fc-event-new-transaction flex" key={event.eventId}>
                                            {(typeof event.profilePhoto === 'string')?(
                                                <Image className="h-10 w-10 rounded-full" src={event.profilePhoto as string} width={40} height={40} alt="Member Photo" onError={(e: SyntheticEvent<HTMLImageElement, Event>): void => {e.currentTarget.src = '/assets/img/cover.png'}} />
                                            ): (
                                                <div className="flex fc-event-icon items-center justify-center">
                                                    { getIcon('IconPerson') }
                                                </div>
                                            )}
                                            <div className="fc-event-text">
                                                <h4>
                                                    <Link href={'/admin/members/' + event.memberId}>{(event.firstName !== '')?event.firstName + ' ' + event.lastName:'#' + event.memberId}</Link> {getWord('admin', 'admin_has_made_a_purchase', memberOptions['Language'])}
                                                </h4>
                                                <h6>{calculateTime(dashboardContent.time, event.created as number, memberOptions['Language'])}</h6>
                                            </div>                                    
                                        </div>);

                                    })):(
                                        <div className="fc-no-events-found">
                                            <p>{ getWord('admin', 'admin_no_events_were_found', memberOptions['Language'])  }</p>
                                        </div>
                                    )}                                                           
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div className="col-span-2">
                    <ChartLineWidget members={dashboardContent.members} datasets={datasets} />
                    <div className="fc-dashboard-widget">
                        <div className="fc-dashboard-widget-head flex justify-between">
                            <h3>
                                { getIcon('IconAddShoppingCart', {className: 'fc-dashboard-widget-icon'}) }
                                <span className="fc-dashboard-widget-text">
                                    { getWord('admin', 'admin_transactions', memberOptions['Language'])  }
                                </span>
                            </h3>
                        </div>
                        <div className="fc-dashboard-widget-body">
                            <ul className="fc-transactions-list">
                                {(dashboardContent.transactions.length > 0)?dashboardContent.transactions.map((transaction: {[key: string]: string}, index: number): React.JSX.Element => {
                                    return (<li key={transaction.transactionId}>
                                        <Link href={'/admin/transactions/' + transaction.transactionId} className="flex justify-between">
                                            <div>
                                                <h2>{unescapeRegexString(transaction.planName)}</h2>
                                                <p>{calculateTime(parseInt(transaction.created), dashboardContent.time, memberOptions['Language'])}</p>
                                            </div>
                                            <div>
                                                <h3>{transaction.planCurrency + " " + transaction.planPrice}</h3>
                                            </div>
                                            <div className="flex justify-end">
                                                {(typeof transaction.profilePhoto === 'string')?(
                                                    <Image className="h-10 w-10 rounded-full" width={40} height={40} src={transaction.profilePhoto as string} alt="Member Photo" onError={(e: SyntheticEvent<HTMLImageElement, Event>): void => {e.currentTarget.src = '/assets/img/cover.png'}} />
                                                ): (
                                                    <span className="fc-transaction-user-icon">
                                                        {getIcon('IconPerson')}
                                                    </span>
                                                )}
                                            </div>                                   
                                        </Link>
                                    </li>);

                                }):(
                                    <li className="fc-no-results-found">{ getWord('admin', 'admin_no_transactions_were_found', memberOptions['Language'])  }</li>
                                )}                                                                       
                            </ul>
                        </div>                    
                    </div>
                </div>
            </div>):(
                <div className="fc-dashboard-container">
                    <div className="fc-dashboard-not-found">
                        <p>{dashboardError}</p>
                    </div>
                </div>
            )}
        </>
    );

}

// Export the page
export default Page;