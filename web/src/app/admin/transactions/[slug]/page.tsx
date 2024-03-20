/*
 * @page Transactions Page
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-10
 *
 * This file contains the transaction's details in the transactions page
 */

'use client'

// Import the react's hooks
import React, { useEffect, useState, useContext } from 'react';

// Import axios module
import axios, { AxiosError, AxiosResponse } from 'axios';

// Import the UseQuery hook
import { useQuery } from 'react-query';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the incs
import { getIcon, getWord, getToken, calculateTime, unescapeRegexString } from '@/core/inc/incIndex';

// Import the types
import { typeToken, typePostHeader } from '@/core/types/typesIndex';

// Import the options for website and member
import { MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Create the Transaction page
const Page = ({params}: {params: { slug: string }}) => {

    // Member options
    let {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);  

    // Hook for transaction's details
    let [transactionDetails, setTransactionDetails] = useState<React.JSX.Element[]>([]);

    // Set a hook for error message if transaction can't be reached
    let [transactionError, setTransactionError] = useState('');

    // Hook to fetch data with useQuery
    let [fetchedData, setFetchedData] = useState(false);

    // Check if document is defined
    if ( typeof document !== 'undefined' ) {

        // Update the page title dynamically
        document.title = getWord('admin', 'admin_transaction', memberOptions['Language']) + ' - #' + params.slug;

    }

    // Get the transaction's information
    let transactionInfo = async (): Promise<any> => {

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

        // Request the fields value
        let response = await axios.get(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/transactions/' + params.slug, headers);

        // Process the response
        return response.data;

    };

    // Request the transaction's information
    let { isLoading, error, data } = useQuery('transactionInfo-' + params.slug, transactionInfo, {
        enabled: !fetchedData
    });

    // Monitor the data change
    useEffect((): void => {

        // Check if has occurred an error
        if ( error ) {

            // Set transaction error
            setTransactionError((error as Error).message);

        } else if ( (typeof data != 'undefined') && !data.success ) { 
            
            // Set transaction error
            setTransactionError(data.message);

        } else if (data) {

            // Empty transactions
            setTransactionDetails([]);

            // Set transaction id
            setTransactionDetails(prev => [...prev, (<div className="fc-detail" key="transaction-id">
            <div className="fc-icon fc-icon-info">
                { getIcon('IconTaskAlt') }
            </div>
            <div className="fc-text">
                <div>
                    <h6>
                        { getWord('admin', 'admin_transaction_id', memberOptions['Language']) }
                    </h6>
                    <h5>
                        { "#" + data.transaction.result.transactionId }
                    </h5> 
                </div>                               
            </div>                            
            </div>)]);

            // Set order id
            setTransactionDetails(prev => [...prev, (<div className="fc-detail" key="order-id">
            <div className="fc-icon fc-icon-info">
                { getIcon('IconLowPriority') }
            </div>
            <div className="fc-text">
                <div>
                    <h6>
                        { getWord('admin', 'admin_order_id', memberOptions['Language']) }
                    </h6>
                    <h5>
                        { data.transaction.result.orderId }
                    </h5> 
                </div>                               
            </div>                            
            </div>)]);                     

            // Set plan name
            setTransactionDetails(prev => [...prev, (<div className="fc-detail" key="plan-name">
            <div className="fc-icon fc-icon-info">
                { getIcon('IconPlans') }
            </div>
            <div className="fc-text">
                <div>
                    <h6>
                        { getWord('admin', 'admin_plan_name2', memberOptions['Language']) }
                    </h6>
                    <h5>
                        { "#" + unescapeRegexString(data.transaction.result.planName) }
                    </h5> 
                </div>                               
            </div>                            
            </div>)]);       

            // Set user name
            setTransactionDetails(prev => [...prev, (<div className="fc-detail" key="user-name">
            <div className="fc-icon fc-icon-promo">
                { getIcon('IconPerson') }
            </div>
            <div className="fc-text">
                <div>
                    <h6>
                        { getWord('admin', 'admin_user', memberOptions['Language']) }
                    </h6>
                    <h5>
                        { (data.transaction.result.firstName !== '')?unescapeRegexString(data.transaction.result.firstName + ' ' + data.transaction.result.lastName):'#' + data.transaction.result.memberId }
                    </h5> 
                </div>                               
            </div>                            
            </div>)]);

            // Set amount
            setTransactionDetails(prev => [...prev, (<div className="fc-detail" key="amount">
            <div className="fc-icon fc-icon-info">
                { getIcon('IconAccountBalanceWallet') }
            </div>
            <div className="fc-text">
                <div>
                    <h6>
                        { getWord('admin', 'admin_amount', memberOptions['Language']) }
                    </h6>
                    <h5>
                        { data.transaction.result.planCurrency + ' ' + data.transaction.result.planPrice }
                    </h5> 
                </div>                               
            </div>                            
            </div>)]);

            // Set created time
            setTransactionDetails(prev => [...prev, (<div className="fc-detail" key="created">
            <div className="fc-icon fc-icon-info">
                { getIcon('IconQueryBuilder') }
            </div>
            <div className="fc-text">
                <div>
                    <h6>
                        { getWord('admin', 'admin_created', memberOptions['Language']) }
                    </h6>
                    <h5>
                        {calculateTime(parseInt(data.transaction.result.created), data.time, memberOptions['Language'])}
                    </h5> 
                </div>                               
            </div>                            
            </div>)]);   

        } 

        // Stop to fetch data
        setFetchedData(true); 
    
    }, [data]);

    return (
        <>
            {(transactionError == '')?(
                <div className="fc-transaction-container">
                    <div className="grid grid-cols-3 gap-6 mb-3">
                        <div className="fc-transaction-basic-information col-span-3 md:col-span-2 mb-5">
                            <div className="flex">
                                <div className="w-full">
                                    <h3 className="fc-section-title">
                                        { getWord('admin', 'admin_transaction_details', memberOptions['Language']) }
                                    </h3>
                                </div>
                            </div>
                            <div className="flex">
                                <div className="w-full">
                                    <div className="fc-details">
                                        {transactionDetails.map((detail, index) => detail)}
                                    </div>
                                </div>
                            </div>
                        </div>          
                    </div>
                </div>
            ): (
                <div className="fc-transaction-container">
                    <div className="fc-transaction-not-found">
                        <p>{transactionError}</p>
                    </div>
                </div>
            )}
        </>

    );

};

// Export the Transaction page
export default Page;