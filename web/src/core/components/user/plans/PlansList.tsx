/*
 * @component Plans List
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This component returns the list with plans
 */

'use client'

// Import the react hooks
import { useState, useContext, useEffect } from 'react';

// Import Link from next
import Link from 'next/link';

// Import axios module
import axios from 'axios';

// Import the UseQuery hook
import { useQuery } from 'react-query';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the incs
import { getIcon, getWord, unescapeRegexString } from '@/core/inc/incIndex';

// Import the types
import { typePostHeader } from '@/core/types/typesIndex';

// Import the options for website and member
import {MemberOptionsContext} from '@/core/contexts/OptionsContext';

/**
 * Plan List Component
 * 
 * @param props with title
 */
const PlansList = (props: {title: string}): React.JSX.Element => {

    // Member options
    const {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);

    // Plans list holder
    const [plans, setPlans] = useState<React.ReactNode | null>(null);

    // Hook to fetch data with useQuery
    const [fetchedData, setFetchedData] = useState(false);

    // Create the request for plans
    const plansList = async (): Promise<any> => {

        // Set the bearer token
        const token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');

        // Set the headers
        const headers: typePostHeader = {
            headers: {
                Authorization: `Bearer ${token}`
            }
        };

        // Request the plans list
        const response = await axios.get(process.env.NEXT_PUBLIC_API_URL + 'api/v1/plans/list', headers);
        
        // Process the response
        return response.data;

    };

    // Request the plans list
    const { isLoading, error, data } = useQuery('plansList', plansList, {
        enabled: !fetchedData
    });

    // Monitor the data change
    useEffect((): void => {

        // Check if has occurred an error
        if ( error ) {

            // Update plans
            setPlans(<div className="fc-plans-not-found">{(error as Error).message}</div>);

        } else if ( (typeof data != 'undefined') && !data.success ) {

            // Update plans
            setPlans(<div className="fc-plans-not-found">{data.message}</div>);

        } else if (data) {

            // Plan subscription expired mark
            let subscriptionExpired = 1;

            // Verify if the user has selected a plan
            if (typeof memberOptions.SubscriptionExpiration !== 'undefined') {

                // Get expiration time
                const expirationTime = memberOptions.SubscriptionExpiration.split('/');

                // Verify if the subscription is expired
                if ( Date.now() < (new Date(expirationTime[2].padStart(2, '0') + '/' + expirationTime[1].padStart(2, '0') + '/' + expirationTime[0]).getTime() + 86400000) ) {

                    // Mark subscrition as valid
                    subscriptionExpired = 0;

                }

            }

            // Update plans
            setPlans(data.plans.map((plan: {planId: string, name: string, price: string, currency: string, features: Array<{featureText: string}>}, planIndex: number) => {

                return (
                    <div className="flex-grow" key={plan.planId}>
                        <div className={((typeof memberOptions.SubscriptionPlanId !== 'undefined') && (memberOptions.SubscriptionPlanId == plan.planId) && (subscriptionExpired < 1) )?("fc-plan fc-current-plan"):("fc-plan")}>
                            <h2>{unescapeRegexString(plan.name)}</h2>
                            {(plan.price != '0.00' && plan.price)?(
                                <h1>{unescapeRegexString(plan.currency)} {unescapeRegexString(plan.price)} <span>/ { getWord('user', 'user_month', memberOptions['Language']) }</span></h1>
                            ):(
                                <h1>0.00 <span>/ { getWord('user', 'user_month', memberOptions['Language']) }</span></h1>
                            )}
                            {((typeof memberOptions.SubscriptionPlanId !== 'undefined') && (memberOptions.SubscriptionPlanId == plan.planId) && (subscriptionExpired < 1) )?(
                                <Link href={"/user/gateways/" + plan.planId}>{ getWord('user', 'user_current_plan', memberOptions['Language']) }</Link>
                            ):(
                                <Link href={"/user/gateways/" + plan.planId}>{ getWord('user', 'user_choose_this_plan', memberOptions['Language']) }</Link>
                            )}
                            <hr />
                            <ul>
                                {plan.features.map((feature: {featureText: string}, featureIndex: number) => (
                                    <li key={featureIndex}>{getIcon('IconCheck')}{unescapeRegexString(feature.featureText)}</li>
                                ))}
                            </ul>
                        </div>
                    </div>
                );

            }));

        } 

        // Stop to fetch data
        setFetchedData(true); 
    
    }, [data]);

    // Run code for client
    useEffect((): void => {

        // Request the plans list
        setFetchedData(false); 

    }, [memberOptions]);

    return (
        <div className="fc-plans-container w-full">
            <div className="w-full fc-plans-header">
                <h2>{ props.title }</h2>
            </div>
            <div className="w-full fc-plans-list">
                <div className="flex">
                    {plans}
                </div>
            </div>                
        </div>    
    );

};

// Export the plans list component
export default PlansList;