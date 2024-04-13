/*
 * @page Plan
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This file contains the page plan in the administrator panel
 */

'use client'

// Import the React hooks
import React, { useState, useEffect, useContext } from 'react';

// Import axios module
import axios from 'axios';

// Import the UseQuery hook
import { useQuery } from 'react-query';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the incs
import { getWord, getIcon, showNotification, unescapeRegexString } from '@/core/inc/incIndex';

// Import types
import { typePostHeader } from '@/core/types/typesIndex';

// Import the options for website and member
import { MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Import the Uis
import { UiModal } from '@/core/components/general/ui/UiIndex';

// Import the Plab Data
import PlanData from "@/core/components/admin/plans/PlanData";

// Import the Transactions List component
import TransactionsList from "@/core/components/admin/transactions/TransactionsList";

// Create the page content
const Page = ({params}: {params: { slug: string }}): React.JSX.Element => {

    // Member options
    const {memberOptions} = useContext(MemberOptionsContext); 

    // Modal status
    const [modalStatus, setModalStatus] = useState('');

    // Set a hook for fields value
    const [fields, setFields] = useState({
        Name: '',
        Price: '0.00',
        Currency: 'USD',
        Websites: 0,
        Default: 1
    });

    // Set a hook for features value
    const [features, setFeatures] = useState<string[]>([]);

    // Set a hook for error message if plan can't be reached
    const [planError, setPlanError] = useState('');

    // Hook to fetch data with useQuery
    const [fetchedData, setFetchedData] = useState(false);

    // Check if document is defined
    if ( typeof document !== 'undefined' ) {

        // Update the page title dynamically
        document.title = getWord('admin', 'admin_plans', memberOptions['Language']);

    }

    // Get the plan's information
    const planInfo = async (): Promise<any> => {

        // Set the bearer token
        const token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');

        // Set the headers
        const headers: typePostHeader = {
            headers: {
                Authorization: `Bearer ${token}`
            }
        };            

        // Request the fields value
        const response = await axios.get(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/plans/' + params.slug, headers)

        // Process the response
        return response.data;

    };

    // New feture modal information
    const newFeature = (): React.JSX.Element => {

        return (
            <form id="fc-create-member-form" onSubmit={newFeatureSave}>            
                <div className="col-span-full fc-modal-text-input">
                    <input
                        type="text"
                        placeholder={getWord('admin', 'admin_enter_feature', memberOptions['Language'])}
                        name="fc-modal-text-input-first-name"
                        id="fc-modal-text-input-first-name"
                        className="block px-2.5 pb-2.5 pt-4 w-full fc-modal-form-input"
                        maxLength={200}
                        required
                    />
                    <label
                        htmlFor="fc-modal-text-input-first-name"
                        className="absolute"
                    >
                        { getIcon('IconFeature') }
                    </label>
                </div>               
                <div className="col-span-full fc-modal-button">
                    <div className="text-right">
                        <button type="submit" className="mb-3 flex justify-between fc-option-violet-btn fc-submit-button">
                            { getWord('admin', 'admin_save_feature', memberOptions['Language']) }
                            { getIcon('IconAutorenew', {className: 'fc-load-more-icon ml-3'}) }
                            { getIcon('IconArrowForward', {className: 'fc-next-icon ml-3'}) }
                        </button>
                    </div>
                </div> 
            </form>
        );

    };

    // Request the plan info
    const { isLoading, error, data } = useQuery('planInfo-' + params.slug, planInfo, {
        enabled: !fetchedData
    });

    // Run some code for the client
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

            // Set error message
            setPlanError((error as Error).message);

        } else if ( (typeof data != 'undefined') && !data.success ) { 
            
            // Set error message
            setPlanError(data.message);

        } else if (data) {

               // Update the field
               setFields((prev: any) => ({
                ...prev,
                Default: 0
            }));

            // Received fields
            const rFields: string[] = Object.keys(data.plan as object);

            // Calculate fields length
            const fieldsLength: number = rFields.length;

            // List the plan infos
            for ( let o = 0; o < fieldsLength; o++ ) {

                // Check if the field is features
                if ( rFields[o] === 'Features' ) {

                    // Check if features exists
                    if ( data.plan![rFields[o]].length > 0 ) {

                        // New list with features
                        const newList: string[] = [];

                        // Features container 
                        let featuresList: string = '';

                        // List all features
                        for ( const feature of data.plan![rFields[o]] ) {

                            // Add feature to the list
                            featuresList += '<li class="flex justify-between">'
                                + '<span>'
                                    + unescapeRegexString(feature)
                                + '</span>'
                                + '<button type="button" class="fc-option-list-manager-delete-item">'
                                    + getWord('default', 'default_delete', memberOptions['Language'])
                                + '</button>'
                            + '</li>';

                            // Add item to the list
                            newList.push(unescapeRegexString(feature));

                        }

                        // Display the features
                        document.querySelector('.fc-option-list-manager ul')!.innerHTML = featuresList; 

                        // Update features
                        setFeatures(newList);

                    }

                } else {

                    // Update the field
                    setFields((prev: any) => ({
                        ...prev,
                        [rFields[o]]: data.plan![rFields[o]]
                    }));

                }

            }     

        } 

        // Stop to fetch data
        setFetchedData(true); 
    
    }, [data]);

    // Track when the features are changed
    useEffect((): void => {

        // Verify if features exists
        if ( features.length > 0 ) {

            // Features container 
            let featuresList: string = '';

            // List all features
            for ( const feature of features ) {

                // Add feature to the list
                featuresList += '<li class="flex justify-between">'
                    + '<span>'
                        + feature
                    + '</span>'
                    + '<button type="button" class="fc-option-list-manager-delete-item">'
                        + getWord('default', 'default_delete', memberOptions['Language'])
                    + '</button>'
                + '</li>';

            }

            // Display the features
            document.querySelector('.fc-option-list-manager ul')!.innerHTML = featuresList;            

        } else {

            // Empty the features list
            document.querySelector('.fc-option-list-manager ul')!.innerHTML = '';

        }

    }, [features]);

    /**
     * Track any click
     * 
     * @param Event e
     */
    const trackClicks = (e: Event): void => {

        // Get the target
        const target = e.target;

        // Check if the click is inside dropdown
        if ( (target instanceof Element) && target.classList && target.classList.contains('fc-option-list-manager-new-item') ) {
            e.preventDefault();

            // Show modal
            setModalStatus('fc-modal-show');

        } else if ( (target instanceof Element) && target.classList && target.classList.contains('fc-option-list-manager-delete-item') ) {
            e.preventDefault();

            // Get all items
            const allItems: NodeListOf<Element> = document.querySelectorAll('.fc-option-list-manager ul > li > span');

            // New list with features
            const newList: string[] = [];

            // List the items
            for ( const item of allItems ) {

                // Verify if is not the deleted item
                if ( !target.closest('li')!.isSameNode(item.closest('li')) ) {

                    // Add item to the list
                    newList.push(item.textContent as string);

                }

            }

            // Update features
            setFeatures(newList);

        }

    };  
    
    // New feature form handler
    const newFeatureSave = (e: React.FormEvent<HTMLFormElement>): void => {
        e.preventDefault();

        // Check if there are more than 9 features
        if ( features.length > 9 ) {

            // Show notification
            showNotification('error', getWord('admin', 'admin_maximum_allowed_features', memberOptions['Language']));
            return;

        }

        // Get the feature
        const feature = e.currentTarget.getElementsByClassName('fc-modal-form-input')[0] as HTMLInputElement;

        // Update features
        setFeatures([...features, feature.value]);

        // Empty the feature input
        feature.value = '';

    };

    return (
        <>
            {(planError == '')?(
                <>
                    <UiModal size="fc-modal-md" title={getWord('admin', 'admin_new_feature', memberOptions['Language'])} status={modalStatus} updateStatus={setModalStatus}>{newFeature()}</UiModal>
                    <div className="fc-plan-container">
                        <div className="grid grid-cols-1 sm:grid-cols-1 md:grid-cols-4 lg:grid-cols-6 xl:grid-cols-6 gap-1 sm:gap-1 md:gap-6 lg:gap-6 xl:gap-6 mb-3">
                            <div className="fc-plan-options col-span-1 md:col-span-2 lg:col-span-3 xl:col-span-3">
                                <PlanData planId={params.slug} fields={fields} setFields={setFields} features={features} />          
                            </div>
                            <div className="fc-transactions-container col-span-1 md:col-span-2 lg:col-span-3 xl:col-span-3">
                                <div className="flex">
                                    <div className="w-full">
                                        <h3 className="fc-section-title">{ getWord('admin', 'admin_transaction_details', memberOptions['Language']) }</h3>
                                    </div>
                                </div>
                                <TransactionsList plan={params.slug} />
                            </div>
                        </div>          
                    </div>                
                </>
            ): (
                <div className="fc-plan-container">
                    <div className="fc-plan-not-found">
                        <p>{planError}</p>
                    </div>
                </div>
            )}
        </>
    );

};

// Export the page content
export default Page;