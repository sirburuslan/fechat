/*
 * @component Plans
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-14
 *
 * This file contains the Plans component for home page
 */

'use client'

// Use the React hooks
import { useEffect, useState, useContext } from 'react';

// Import Link from next
import Link from 'next/link';

// Import axios module
import axios, { AxiosError, AxiosResponse } from 'axios';

// Import the incs
import { getWord, getIcon, unescapeRegexString } from '@/core/inc/incIndex';

// Import the options for website and member
import { WebsiteOptionsContext, MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Create the Plans component
const Plans = (): React.JSX.Element => {

    // Website options
    const {websiteOptions} = useContext(WebsiteOptionsContext);

    // Member options
    const {memberOptions} = useContext(MemberOptionsContext);

    // Plans list holder
    const [plans, setPlans] = useState<React.ReactNode | null>(null);

    // Create the request for plans
    const plansListRequest = async (): Promise<void> => {

        try {

            // Request the plans list
            await axios.get(process.env.NEXT_PUBLIC_API_URL + 'api/v1/plans/list')
            
            // Process the response
            .then((response: AxiosResponse): void => {

                // Verify if the response is successfully
                if ( response.data.success ) {

                    // Update plans
                    setPlans(response.data.plans.map((plan: {planId: string, name: string, price: string, currency: string, features: Array<{featureText: string}>}, planIndex: number) => {

                        return (
                            <div className="flex-grow" key={plan.planId}>
                                <div className="fc-plan">
                                    <h2>{unescapeRegexString(plan.name)}</h2>
                                    <h1>{unescapeRegexString(plan.currency)} {unescapeRegexString(plan.price)} <span>/ { getWord('user', 'user_month', memberOptions['Language']) }</span></h1>
                                    {(websiteOptions.RegistrationEnabled === '1')?(
                                        <Link href="/auth/registration" scroll={false}>{ getWord('public', 'public_get_started', memberOptions['Language']) }</Link>
                                    ):''}
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

                } else {

                    // Update plans
                    setPlans(<div className="fc-plans-not-found">{response.data.message}</div>);

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

            

        }

    };

    // Run code for client
    useEffect((): void => {

        // Request the plans list
        plansListRequest();

    }, [memberOptions]);

    return (
        <div className="fc-plans">
            <div className="fc-plans-container">
                <div className="w-full">
                    <h2>{ getWord('public', 'public_lifetime_plans', memberOptions['Language']) }</h2>
                </div>
                <div className="w-full fc-plans-list">
                    <div className="flex">
                        {plans}
                    </div>
                </div>                              
            </div>
        </div>
    );

};

// Export the Plans component
export default Plans;