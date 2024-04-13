/*
 * @page Transactions List
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This file contains the transactions list displayed in the transactions page
 */

// Import the react's hooks
import { useContext, useState, useEffect, SyntheticEvent } from 'react';

// Import the Next/Link component
import Link from 'next/link';

// Import the Next/Image component
import Image from 'next/image';

// Import axios module
import axios from 'axios';

// Import the UseQuery hook
import { useQuery } from 'react-query';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Imoport the incs
import { getIcon, getWord, calculateTime, unescapeRegexString } from '@/core/inc/incIndex';

// Import the types
import { typePostHeader } from '@/core/types/typesIndex';

// Import the options for website and member
import { MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Import the Uis
import { UiNavigation } from '@/core/components/general/ui/UiIndex';

// Create the TransactionsList component
const TransactionsList = (props: {plan?: string}) => {

    // Member options
    const {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);  

    // Set a hook for search parameters
    const [search, setSearch] = useState({
        Search: '',
        Page: 1
    });   
    
    // Set a hook for navigation
    const [navigation, setNavigation] = useState({
        scope: 'transactions',
        page: 0,
        total: 0,
        limit: 10
    });

    // Transactions list holder
    const [transactions, setTransactions] = useState<React.ReactNode | null>(null);

    // Hook to fetch data with useQuery
    const [fetchedData, setFetchedData] = useState(false);

    // Create the request for transactions
    const transactionsListRequest = async (): Promise<any> => {

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

        // Request the transactions list
        const response = await axios.post((typeof props.plan !== 'undefined')?process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/transactions/list/' + props.plan as string:process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/transactions/list/', search, headers);
        
        // Process the response
        return response.data;

    };

    // Request the transactions list
    const { isLoading, error, data } = useQuery('transactionsList-' + props.plan, transactionsListRequest, {
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

        // Check if has occurred an error
        if ( error ) {

            // Update transactions
            setTransactions(<div className="fc-no-transactions-found">{(error as Error).message}</div>);

        } else if ( (typeof data != 'undefined') && !data.success ) { 
            
            // Update transactions
            setTransactions(<div className="fc-no-transactions-found">{data.message}</div>);

        } else if (data) {

            // Update transactions
            setTransactions(data.result.elements.map((transaction: {[key: string]: string}, index: number): React.JSX.Element => {

                return (<li className="fc-transaction flex justify-between" key={transaction.transactionId}>                                
                        <div>
                            <h2>
                                <Link href={'/admin/transactions/' + transaction.transactionId}>
                                    {unescapeRegexString(transaction.planName)}
                                </Link>
                            </h2>
                            <p>{calculateTime(parseInt(transaction.created), data.time, memberOptions['Language'])}</p>
                        </div>
                        <div>
                            <h3>{transaction.planCurrency + " " + transaction.planPrice}</h3>
                        </div>
                        <div className="flex justify-end">
                            {(typeof transaction.profilePhoto === 'string')?(
                                <Image src={transaction.profilePhoto as string} width={30} height={30} alt="Member Photo" className="h-10 w-10 rounded-full" onError={(e: SyntheticEvent<HTMLImageElement, Event>): void => {e.currentTarget.src = '/assets/img/cover.png'}} />
                            ): (
                                <span className="fc-transaction-user-icon">
                                    {getIcon('IconPerson')}
                                </span>
                            )}
                        </div>
                    </li>);

            }));

            // Set limit
            const limit: number = ((data.result.page * 10) < data.result.total)?(data.result.page * 10):data.result.total;

            // Set text
            document.querySelector('#fc-navigation-transactions h3')!.innerHTML = (((data.result.page - 1) * 10) + 1) + '-' + limit + ' ' + getWord('default', 'default_of', memberOptions['Language']) + ' ' + data.result.total + ' ' + getWord('default', 'default_results', memberOptions['Language']);

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

        // Check if the click is on a page link
        if ( (target instanceof Element) && target.closest('#fc-navigation-transactions') && (target.nodeName === 'A') ) {
            e.preventDefault();

            // Get the page
            const page: string | null = target.getAttribute('data-page');

            // Set search value
            search.Page = parseInt(page!);

            // Search for transactions
            setSearch(search);

            // Request the transactions list
            setFetchedData(false);

        }

    };

    return (
        <>
            <div className="flex mb-3">
                <div className="w-full gap-4 fc-transactions-list">{transactions}</div>
            </div> 
            <div className="fc-navigation flex justify-between items-center justify-center pl-3 pr-3 fc-transparent-color" id="fc-navigation-transactions">
                <h3></h3>
                <UiNavigation scope={navigation.scope} page={navigation.page} total={navigation.total} limit={navigation.limit} />
            </div>
        </>
    );

};

// Export the TransactionsList component
export default TransactionsList;