/*
 * @component Gateways List
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This file shows the gateways list in the user's panel
 */

// Import the react hooks
import { useState, useContext, useEffect } from 'react';

// Import axios module
import axios, { AxiosError, AxiosResponse } from 'axios';

// Import the UseQuery hook
import { useQuery } from 'react-query';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import PayPal components for button
import { PayPalScriptProvider, PayPalButtons, usePayPalScriptReducer } from "@paypal/react-paypal-js";

// Import the incs
import { getWord, showNotification, getOptions, updateOptions } from '@/core/inc/incIndex';

// Import the types
import { typePostHeader, typeOptions } from '@/core/types/typesIndex';

// Import the options for website and member
import { WebsiteOptionsContext, MemberOptionsContext } from '@/core/contexts/OptionsContext';

/**
 * Configure PayPal button
 * 
 * @param string planId
 * @param string payPalPlanId
 */
const PayPalButton = ({planId, payPalPlanId}: {planId: string, payPalPlanId: string}) => {

    // Website options
    const {websiteOptions, setWebsiteOptions} = useContext(WebsiteOptionsContext); 

    // Member options
    const {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);  

    // Get access to the Script context and dispatch actions
	const [{ options }, dispatch] = usePayPalScriptReducer();

    // Wait for content load
	useEffect(() => {

        // Set options
        dispatch({
            type: "resetOptions",
            value: {
                ...options,
                intent: "subscription",
            },
        });
        
    }, []);

    /**
     * Catch approve payment click
     * 
     * @param data 
     */
    const onApprove = async (data: any): Promise<void> => {

        try {

            // Set the bearer token
            const token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');

            // Create a subscription
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/subscriptions', {
                PlanId: parseInt(planId),
                Source: "PayPal",
                OrderId: data.orderID,
                SubscriptionId: data.subscriptionID
            }, {
                headers: {
                    Authorization: `Bearer ${token}`
                }
            })

            // Process the response
            .then(async (response: AxiosResponse): Promise<void> => {

                // Check if the file was uploaded
                if ( response.data.success ) {

                    // Show success notification
                    showNotification('success', response.data.message);

                    // Request the options
                    const optionsList: {success: boolean, options?: typeOptions} = await getOptions();

                    // Update memberOptions
                    updateOptions(optionsList, setWebsiteOptions, setMemberOptions);

                    // Wait 2 seonds
                    setTimeout((): void => {

                        // Redirect the user to the dashboard page
                        document.location.href = process.env.NEXT_PUBLIC_SITE_URL + 'user/dashboard';

                    }, 2000);

                } else {

                    // Show error notification
                    throw new Error(response.data.message);

                }

            })
            
            // Catch the error message
            .catch((error: AxiosError): void => {

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

	return (<PayPalButtons createSubscription={async (data: Record<string, unknown>, actions: any) => {

            // Get the order ID
			const orderId = await actions.subscription.create({plan_id: payPalPlanId});

            return orderId;

		}}

		style={{
			label: "subscribe",
		}}

        onApprove={onApprove}

	/>);

}

const GatewaysList = (props: {plan: string}): React.JSX.Element => {

    // Website options
    const {websiteOptions, setWebsiteOptions} = useContext(WebsiteOptionsContext); 

    // Member options
    const {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);

    // Set a hook for gateways list
    const [gatewaysList, setGatewaysList] = useState<React.ReactNode>([]);    

    // Set a hook for error messages
    const [gatewaysError, setGatewaysError] = useState('');

    // Hook to fetch data with useQuery
    const [fetchedData, setFetchedData] = useState(false);

    // Check if document is defined
    if ( typeof document !== 'undefined' ) {

        // Update the page title dynamically
        document.title = getWord('user', 'user_payments_page', memberOptions['Language']);

    }

    // Get the gateways list
    const gateways = async (): Promise<any> => {

        // Set the bearer token
        const token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');

        // Set the headers
        const headers: typePostHeader = {
            headers: {
                Authorization: `Bearer ${token}`
            }
        };            

        // Request the fields value
        const response = await axios.get(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/gateways/' + props.plan, headers)

        // Process the response
        return response.data;

    };

    // Request the gateways list
    const { isLoading, error, data } = useQuery('gatewaysList', gateways, {
        enabled: !fetchedData
    });

    // Monitor the data change
    useEffect((): void => {

        // Check if has occurred an error
        if ( error ) {

            // Set gateways error
            setGatewaysError((error as Error).message);

        } else if ( (typeof data != 'undefined') && !data.success ) { 
            
            // Set gateways error
            setGatewaysError(data.message);

        } else if (data) {

            // Check if free parameter exists
            if ( typeof data.free !== 'undefined' ) {

                // Show success notification
                setGatewaysError(data.message);

                // Wait a moment
                setTimeout(async (): Promise<void> => {

                    // Request the options
                    const optionsList: {success: boolean, options?: typeOptions} = await getOptions();

                    // Update memberOptions
                    updateOptions(optionsList, setWebsiteOptions, setMemberOptions);

                }, 10);

                // Wait 2 seonds
                setTimeout((): void => {

                    // Redirect the user to the dashboard page
                    document.location.href = process.env.NEXT_PUBLIC_SITE_URL + 'user/dashboard';

                }, 3000);

            } else {

                // List the gateways
                setGatewaysList(data.gateways.map((gateway: {network: string, planId?: string, clientId?: string}, indexGateway: number): React.JSX.Element => {

                    // Check if gateway is PayPal
                    if ( gateway.network === 'PayPal' ) {

                        return (
                            <li key={gateway.network}>
                                <PayPalScriptProvider options={{clientId: gateway.clientId as string, components: "buttons", intent: "subscription", vault: true}}>
                                    <PayPalButton planId={props.plan} payPalPlanId={gateway.planId as string} />
                                </PayPalScriptProvider>
                            </li>
                        )

                    }

                    return (<></>);

                }));

            }

        } 

        // Stop to fetch data
        setFetchedData(true); 
    
    }, [data]);

    return (
        <>
            {(isLoading)?(
                <></>
            ):(
                (gatewaysError == '')?(
                    <div className="fc-gateways-container w-full">
                        <div className="w-full fc-gateways-header">
                            <h2>{ getWord('default', 'default_choose_payment_method', memberOptions['Language']) }</h2>
                        </div>
                        <div className="w-full fc-gateways-list">
                            <ul>
                                {gatewaysList}
                            </ul>
                        </div>                          
                    </div>
                ): (
                    <div className="fc-gateways-container w-full pr-3 pl-3">
                        <div className="fc-gateways-not-found w-full">
                            <p>{gatewaysError}</p>
                        </div>
                    </div>
                )          
            )}
        </>
    );

};

// Export the gateways list
export default GatewaysList;