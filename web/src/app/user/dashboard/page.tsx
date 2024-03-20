/*
 * @page Dashboard
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This file contains the page dashboard in the user panel
 */

'use client'

// Import the react hooks
import { useState, useContext, useEffect, ReactNode } from 'react';

// Import Link from next
import Link from 'next/link';

// Import axios module
import axios, { AxiosError, AxiosResponse } from 'axios';

// Import the UseQuery hook
import { useQuery } from 'react-query';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the incs
import { getIcon, getWord, getToken, showNotification, calculateTime, unescapeRegexString } from '@/core/inc/incIndex';

// Import the types
import { typeToken, typePostHeader } from '@/core/types/typesIndex';

// Import the options for website and member
import {MemberOptionsContext} from '@/core/contexts/OptionsContext';

// Import the Chart Line Widget
import ChartLineWidget from '@/core/components/user/dashboard/ChartLineWidget';

// Create the page content
const Page = (): React.JSX.Element => {

    // Member options
    let {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);

    // Set a hook for error message if dashboard can't be reached
    let [dashboardError, setDashboardError] = useState('');

    // Set a hook for dashboard content
    let [dashboardContent, setDashboardContent] = useState<{threads: Array<{[key: string]: string}>, messages: React.ReactNode | null}>({
        threads: [],
        messages: null
    });

    // Hook to fetch data with useQuery
    let [fetchedData, setFetchedData] = useState(false);

    // Check if document is defined
    if ( typeof document !== 'undefined' ) {

        // Update the page title dynamically
        document.title = getWord('admin', 'admin_dashboard', memberOptions['Language']);

    };

    // Verify if threads chart time exists
    if ( (typeof document !== 'undefined') && (typeof memberOptions.ThreadsChartTime === 'string') ) {

        // Get the dropown link
        let dropdownLink: Node | null = document.querySelector('#fc-threads-chart-menu a[data-id="' + memberOptions.ThreadsChartTime + '"]');

        // Check if dropdown link exists
        if ( dropdownLink ) {

            // Set the dropown link
            document.getElementsByClassName('fc-dashboard-widget-dropdown-text')[0].textContent = dropdownLink.textContent;

        }
        
    };

    // Get the last updated threads
    let lastUpdatedThreads = async (): Promise<any> => {

        // Generate a new CSRF token
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

        // Get the list with updated threads
        let response = await axios.get(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/threads/last', headers);

        // Process the response
        return response.data;

    };

    // Create the data sets for chart
    let datasets: string[] = [getWord('user', 'user_threads', memberOptions['Language'])];

    // Get content for dashboard
    let dashboardData = async (): Promise<void> => {

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
            await axios.get(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/dashboard', headers)
            
            // Process the response
            .then((response: AxiosResponse): void => {

                // Verify if the response is successfully
                if ( response.data.success ) {
                    
                    // Check if threads exists
                    if ( typeof response.data.threads !== 'undefined' ) {

                        // Update the dashboard content
                        setDashboardContent((prev: {threads: Array<{[key: string]: string}>, messages: React.ReactNode | null}) => ({
                            ...prev,
                            threads: response.data.threads
                        }));
                        
                    }

                } else {

                    // Set error message
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

    // Get threads when the user changes the time preferences
    let dashboardThreads = async (): Promise<void> => {

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
            await axios.get(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/dashboard/threads', headers)
            
            // Process the response
            .then((response: AxiosResponse): void => {
                
                // Verify if the response is successfully
                if ( response.data.success ) {

                    // Check if threads exists
                    if ( typeof response.data.threads !== 'undefined' ) {

                        // Update the dashboard content
                        setDashboardContent((prev: {threads: Array<{[key: string]: string}>, messages: React.ReactNode | null}) => ({
                            ...prev,
                            threads: response.data.threads
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

    // Request the threads list
    let { isLoading, error, data } = useQuery('threadsListHot', lastUpdatedThreads, {
        enabled: !fetchedData
    });

    // Run code after content load
    useEffect((): () => void => {

        // Request the dashboard content
        dashboardData();

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

            // Update threads
            setDashboardContent((prev: {threads: Array<{[key: string]: string}>, messages: React.ReactNode | null}) => ({
                ...prev,
                messages: (
                    <li className="fc-no-threads-found">
                        {(error as Error).message}
                    </li>
                )

            }));

        } else if ( (typeof data != 'undefined') && !data.success ) { 
            
            // Update threads
            setDashboardContent((prev: {threads: Array<{[key: string]: string}>, messages: React.ReactNode | null}) => ({
                ...prev,
                messages: (
                    <li className="fc-no-threads-found">
                        {data.message}
                    </li>
                )

            }));

        } else if (data) {

            // Update threads
            setDashboardContent((prev: { threads: { [key: string]: string; }[]; messages: ReactNode; }) => ({
                ...prev,
                messages: data.result.elements.map((thread: {threadId: string, memberId: number, messageSeen: number, guestName: string, message: string, created: number, totalMessages: number}, threadIndex: number) => {

                    return (
                        <li key={thread.threadId}>
                            <Link href={'/user/threads/' + thread.threadId} className={((thread.messageSeen < 1) && (thread.memberId === 0))?"fc-unread-message":""}>
                                <div className="w-full mb-2 flex justify-between">
                                    <h4>
                                        {unescapeRegexString(thread.guestName)}
                                        {(thread.created > (data.time - 86400))?(
                                            <span className="fc-thread-new">
                                                {getWord('user', 'user_new_thread', memberOptions['Language'])}
                                            </span>                                                      
                                        ):''}

                                    </h4>
                                    <span>{calculateTime(thread.created, parseInt(data.time), memberOptions['Language'])}</span>
                                </div>
                                <div className="w-full">
                                    <p>
                                        {unescapeRegexString(thread.message.replaceAll(/\\n/g, " "))}
                                    </p>
                                </div> 
                                <div className="w-full fc-message-footer">
                                    { getIcon('IconMarkChatUnread', {className: 'fc-thread-messages-count-icon'}) }
                                    <span className="fc-thread-messages-count-text">
                                        {thread.totalMessages}
                                    </span>                                                
                                </div>                                       
                            </Link>
                        </li>
                    );

                })

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
        if ( (target instanceof Element) && target.closest('#fc-threads-chart-menu') && (target.nodeName === 'A') ) {
            e.preventDefault();

            setMemberOptions((prev: {
                memberOptions: {
                    [key: string]: string;
                };
                setMemberOptions: React.Dispatch<React.SetStateAction<any>>;
            }) => ({
                ...prev,
                ThreadsChartTime: (target as HTMLElement).getAttribute('data-id')
            }));

            // Set the dropown link
            document.getElementsByClassName('fc-dashboard-widget-dropdown-text')[0].textContent = target.textContent;

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
                    ThreadsChartTime: target.getAttribute('data-id')
                };
    
                // Set the headers
                let headers: typePostHeader = {
                    headers: {
                        Authorization: `Bearer ${token}`,
                        CsrfToken: csrfToken.token
                    }
                };
    
                // Run the request
                await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/options', fields, headers)
    
                // Process the response
                .then((response: AxiosResponse) => {

                    // Verify if the response is successfully
                    if ( response.data.success ) {

                        // Request the dashboard threads
                        dashboardThreads();

                    }

                    // Verify if an error has been occurred
                    if ( response.data.ThreadsChartTime ) {
    
                        // Throw error
                        throw new Error(response.data.ThreadsChartTime);
                        
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

        }

    };

    return (
        <>
            {(dashboardError == '')?(
            <div className="grid md:grid-cols-1 lg:grid-cols-2 gap-4 fc-dashboard-container">
                <div className="fc-messages-section">
                    <div className="flex mb-3">
                        <div className="w-full">
                            <h2 className="fc-page-title">
                                { getWord('user', 'user_messages', memberOptions['Language'])  }
                            </h2>
                        </div>
                    </div>
                    <div className="flex mb-3">
                        <div className="w-full">
                            <ul className="fc-messages-last">
                                {dashboardContent.messages}
                            </ul>
                        </div>
                    </div>                    
                </div>
                <div>
                    <ChartLineWidget threads={dashboardContent.threads} datasets={datasets} />
                    <div className="fc-dashboard-widget">
                        <div className="fc-dashboard-widget-head flex justify-between">
                            <h3>
                                { getIcon('IconAddCard', {className: 'fc-dashboard-widget-icon'}) }
                                <span className="fc-dashboard-widget-text">
                                    { getWord('user', 'user_plan_subscription', memberOptions['Language']) }
                                </span>
                            </h3>
                        </div>
                        <div className="fc-dashboard-widget-body">
                            <div className="fc-subscription-plan flex justify-between">
                                <div>
                                    <h2>{(typeof memberOptions.SubscriptionPlanName !== 'undefined')?memberOptions.SubscriptionPlanName:''}</h2>
                                    {(memberOptions.SubscriptionPlanPrice === '0.00')?(
                                        <p>{ getWord('user', 'user_expires', memberOptions['Language']) } - {memberOptions.SubscriptionExpiration}</p>
                                    ):(
                                        <p>{ getWord('user', 'user_next_billing_date', memberOptions['Language']) } - {memberOptions.SubscriptionExpiration}</p>
                                    )}
                                </div>
                                <Link href={'/user/plans'}>
                                    { getWord('user', 'user_upgrade', memberOptions['Language'])  }
                                    { getIcon('IconMoving') }                                
                                </Link>
                            </div>
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