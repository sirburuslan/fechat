/*
 * @component Plan List
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-16
 *
 * This file contains the plans list component in the administrator panel
 */

'use client'

// Import the react's hooks
import { useContext, useState, useEffect, useRef } from 'react';

// Import the Next JS Link component
import Link from 'next/link';

// Import axios module
import axios, { AxiosError, AxiosResponse } from 'axios';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the UseQuery hook
import { useQuery } from 'react-query';

// Import the incs
import { getIcon, getWord, showNotification, unescapeRegexString } from '@/core/inc/incIndex';

// Import the types
import { typePostHeader } from '@/core/types/typesIndex';

// Import the options for website and member
import { MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Import the Uis
import { UiDropdown, UiModal, UiNavigation } from '@/core/components/general/ui/UiIndex';

// Import the Confirmation component
import Confirmation from '@/core/components/general/Confirmation';

// Create the Plans List component
const PlansList = (): React.JSX.Element => {

    // Member options
    const {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);  

    // Modal status
    const [modalStatus, setModalStatus] = useState('');

    // New plan fields
    const [plan, setPlan] = useState({
        name: ''
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
        scope: 'plans',
        page: 0,
        total: 0,
        limit: 10
    });  

    // Plans list holder
    const [plans, setPlans] = useState<React.ReactNode | null>(null);

    // Hook to fetch data with useQuery
    const [fetchedData, setFetchedData] = useState(false);

    // Search pause container
    const searchPause = useRef<NodeJS.Timeout>();

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

    // Create the request for plans
    const plansListRequest = async (): Promise<any> => {

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

        // Request the plans list
        const response = await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/plans/list', search, headers);
        
        // Process the response
        return response.data;

    };

    // content for the New Plan modal
    const newPlan = (): React.JSX.Element => {

        return (
            <form onSubmit={createPlan} id="fc-create-plan-form">            
                <div className="col-span-full fc-modal-text-input">
                    <input
                        type="text"
                        placeholder={getWord('admin', 'admin_enter_plan_name', memberOptions['Language'])}
                        value={plan.name}
                        name="fc-modal-text-input-plan-name"
                        id="fc-modal-text-input-plan-name"
                        className="block px-2.5 pb-2.5 pt-4 w-full fc-modal-form-input"
                        onChange={(e: React.ChangeEvent<HTMLInputElement>): void => setPlan({...plan, name: e.target.value})}
                        required
                    />
                    <label
                        htmlFor="fc-modal-text-input-plan-name"
                        className="absolute"
                    >
                        { getIcon('IconShoppingBasket') }
                    </label>
                    <div className={(typeof (errors as {[key: string]: string}).PlanName !== 'undefined')?'fc-modal-form-input-error-message fc-modal-form-input-error-message-show':'fc-modal-form-input-error-message'}>
                        {(typeof (errors as {[key: string]: string}).PlanName !== 'undefined')?(errors as {[key: string]: string}).PlanName:''}
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

    // Request the plans list
    const { isLoading, error, data } = useQuery('plansList', plansListRequest, {
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

            // Update plans
            setPlans(<div className="fc-no-plans-found">{(error as Error).message}</div>);

        } else if ( (typeof data != 'undefined') && !data.success ) { 
            
            // Update plans
            setPlans(<div className="fc-no-plans-found">{data.message}</div>);

        } else if (data) {

            // Update plans
            setPlans(data.result.elements.map((plan: any, index: number) => {

                // Dropdown items
                const dropdownItems: Array<{text: string, url: string, id: string}> = [{
                    text: getWord('default', 'default_edit', memberOptions['Language']),
                    url: '/admin/plans/' + plan.planId,
                    id: 'edit-plan'
                }, {
                    text: getWord('default', 'default_delete', memberOptions['Language']),
                    url: '#',
                    id: 'delete-plan'
                }];

                return (
                    <div className="fc-plan flex justify-between" key={index} data-plan={plan.planId}>
                        <div>
                            <h4>
                                <Link href={'/admin/plans/' + plan.planId}>
                                    {unescapeRegexString(plan.name)}
                                </Link>
                            </h4>
                            <p className="fc-plan-free">{(plan.price != '')?plan.currency + ' ' + plan.price:getWord('default', 'default_free', memberOptions['Language'])}</p>
                        </div>
                        <div className="text-center">
                        </div>                        
                        <div className="text-right">
                            <button type="button" className="fc-manage-plan" onClick={ planDropdown }>
                                { getIcon('IconMoreHoriz') }
                            </button>
                            <UiDropdown button="" options={ dropdownItems } id={'fc-plan-menu-' + plan.planId} />
                        </div>
                    </div>
                );

            }));

            // Set limit
            const limit: number = ((data.result.page * 10) < data.result.total)?(data.result.page * 10):data.result.total;

            // Set text
            document.querySelector('#fc-navigation-plans h3')!.innerHTML = (((data.result.page - 1) * 10) + 1) + '-' + limit + ' ' + getWord('default', 'default_of', memberOptions['Language']) + ' ' + data.result.total + ' ' + getWord('default', 'default_results', memberOptions['Language']);

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
        if ( (target instanceof Element) && target.closest('.fc-dropdown-menu') && (target.nodeName === 'A') && (target.getAttribute('data-id') === 'delete-plan') ) {
            e.preventDefault();

            // Get the plan's ID
            const planId: string | null = target.closest('.fc-plan')!.getAttribute('data-plan');

            // Create a link
            const newLink: HTMLAnchorElement = document.createElement('a');

            // Set the plan's ID
            newLink.setAttribute('data-id', planId!);

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

        } else if ( (target instanceof Element) && target.closest('#fc-navigation-plans') && (target.nodeName === 'A') ) {
            e.preventDefault();

            // Get the page
            const page: string | null = target.getAttribute('data-page');

            // Set search value
            search.Page = parseInt(page!);

            // Search for plans
            setSearch(search);

            // Request the plans list
            setFetchedData(false); 

        }

    };

    // Create the function which will handle the plan's menu click
    const planDropdown = (e: React.MouseEvent<HTMLElement>): void => {
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
     * Create plan form handler
     * 
     * @param FormEvent e 
     */
    const createPlan = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
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

            // Create a new plan request
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/plans', {
                Name: plan.name
            }, headers)

            // Process the response
            .then((response: AxiosResponse) => {

                // Verify if the response is successfully
                if ( response.data.success ) {
                    
                    // Show success notification
                    showNotification('success', response.data.message);

                    // Reset the form
                    setPlan({
                        name: ''
                    });

                    // Request the plans list
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
                    throw new Error(getWord('admin', 'admin_plan_not_created', memberOptions['Language']));                    

                }

            })

            // Catch the error message
            .catch((e: AxiosError): void => {

                // Show error notification
                throw new Error(e.message);

            })

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

    /**
     * Search for plans
     * 
     * @param React.ChangeEvent e 
     */
    const searchPlans = (e: React.ChangeEvent<HTMLInputElement>): void => {

        // Add fc-search-active to show the animation
        e.target.closest('.fc-search-box')!.classList.add('fc-search-active');

        // Set search page
        search.Page = 1;

        // Set search value
        search.Search = e.target.value;

        // Search for plans
        setSearch(search);

        // Schedule a search
        scheduleSearch((): void => { 
            
            // Request the plans list
            setFetchedData(false); 

        }, 1000);

    };

    /**
     * Cancel search for plans
     * 
     * @param React.ChangeEvent e 
     */
    const cancelSearchPlans = (e: React.MouseEvent<HTMLAnchorElement>): void => {

        // Add fc-search-complete to hide the animation
        e.currentTarget.closest('.fc-search-box')!.classList.remove('fc-search-complete');

        // Reset search
        search.Search = '';

        // Search for plans
        setSearch(search);

        // Set a pause
        setTimeout(() => {

            // Request the plans list
            setFetchedData(false); 

        }, 500);

        // Empty the search value
        (document.getElementById('fc-search-for-plans') as HTMLInputElement).value = '';



    };

    /**
     * Delete plan
     * 
     * @param string planId
     */
    const deletePlan = async (id: string): Promise<void> => {

        try {

            // Set the bearer token
            const token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');

            // Set the headers
            const headers: typePostHeader = {
                headers: {
                    Authorization: `Bearer ${token}`
                }
            }; 

            // Delete plan
            await axios.delete(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/plans/' + id, headers)

            // Process the response
            .then((response: AxiosResponse) => {

                // Verify if the response is successfully
                if ( response.data.success ) {
                    
                    // Show success notification
                    showNotification('success', response.data.message);

                    // Check if this was the last plan in the list
                    if ( document.querySelectorAll('.fc-plans-list > .fc-plan').length < 2 ) {

                        // Set previous page
                        search.Page = (search.Page > 1)?(search.Page - 1):search.Page;

                        // Search for plans
                        setSearch(search);

                        // Request the plans list
                        setFetchedData(false); 

                    } else {

                        // Request the plans list
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

    return (
        <>
            <UiModal size="fc-modal-md" title={getWord('admin', 'admin_new_plan', memberOptions['Language'])} status={modalStatus} updateStatus={setModalStatus}>{newPlan()}</UiModal>
            <Confirmation confirmAction={deletePlan}><>{getWord('admin', 'admin_deleting_plan_permanent', memberOptions['Language'])}</></Confirmation>
            <div className="fc-plans-container">
                <div className="flex mb-3">
                    <div className="w-full">
                        <h2 className="fc-page-title">
                            { getWord('admin', 'admin_plans', memberOptions['Language'])  }
                        </h2>
                    </div>
                </div>
                <div className="flex mb-3">
                    <div className="w-full flex">
                        <div className="flex fc-search-box fc-transparent-color">
                            <span className="fc-search-icon">
                                { getIcon('IconSearch') }
                            </span>
                            <input type="text" className="form-control fc-search-input" placeholder={ getWord('admin', 'admin_search_for_plans', memberOptions['Language']) } id="fc-search-for-plans" onInput={searchPlans} />
                            <Link href="#" onClick={cancelSearchPlans}>
                                { getIcon('IconAutorenew', {className: 'fc-load-more-icon'}) }
                                { getIcon('IconCancel', {className: 'fc-cancel-icon'}) }
                            </Link>
                        </div>
                        <button className="ec-search-btn fc-new-plan-button" onClick={(): void => setModalStatus('fc-modal-show')}>
                            { getIcon('IconShoppingCartCheckout') }
                            { getWord('admin', 'admin_new_plan', memberOptions['Language'])  }
                        </button>                
                    </div>
                </div>  
                <div className="flex mb-3">
                    <div className="w-full gap-4 fc-plans-list">{plans}</div>
                </div> 
                <div className="fc-navigation flex justify-between items-center justify-center pl-3 pr-3 fc-transparent-color" id="fc-navigation-plans">
                    <h3></h3>
                    <UiNavigation scope={navigation.scope} page={navigation.page} total={navigation.total} limit={navigation.limit} />
                </div>
            </div>
        </>
    )

}

// Export the Plans List component
export default PlansList;