/*
 * @layout Sidebar
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains the sidebar layout with the menu for admin panel
 */

'use client';

// Import the React's features
import { MouseEventHandler, useContext, SyntheticEvent } from 'react';

// Import the Next JS Link component
import Link from 'next/link';

// Import the Next JS Image component
import Image from 'next/image';

// Get usePathname from the navigation
import { usePathname } from 'next/navigation';

// Import the axios module
import axios, { AxiosResponse, AxiosError } from 'axios';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the incs
import { getIcon, getWord, showNotification } from '@/core/inc/incIndex';

// Import the options for website and member
import {WebsiteOptionsContext, MemberOptionsContext} from '@/core/contexts/OptionsContext';

// Import the UI Components
import { UiDropdown } from '@/core/components/general/ui/UiIndex';

// Import the types
import { typePostHeader } from '@/core/types/typesIndex';

// Create the sidebar layout
const SidebarLayout = (): React.JSX.Element => {

    // Website options
    const {websiteOptions} = useContext(WebsiteOptionsContext); 

    // Member options
    const {memberOptions} = useContext(MemberOptionsContext);  

    // Get the current path name
    const pathname: string = usePathname();

    // Dropdown items
    const dropdownitems: Array<{text: string, url: string}> = [{
        text: getWord('admin', 'admin_settings', memberOptions['Language']),
        url: '/admin/settings'
    }, {
        text: getWord('default', 'default_sign_out', memberOptions['Language']),
        url: '/auth/signout'
    }];

    /**
     * Change the sidebar status
     * 
     * @param number sidebarStatus
     */
    const changeSidebarStatus = async (sidebarStatus: number): Promise<void> => {

        try {

            // Set the bearer token
            const token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');

            // Set the post's fields
            const fields: {
                [key: string]: string | number | null
            } = {
                sidebarStatus: sidebarStatus
            };

            // Set the headers
            const headers: typePostHeader = {
                headers: {
                    Authorization: `Bearer ${token}`
                }
            };

            // Run the request
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/members/options', fields, headers)

            // Process the response
            .then((response: AxiosResponse) => {

                // Verify if an error has been occurred
                if ( !response.data.success && (typeof response.data.message !== 'undefined') ) {
                    
                    // Throw error on message
                    throw new Error(response.data.message);                    
                    
                } else if ( typeof response.data.SidebarStatus !== 'undefined' ) {

                    // Throw error on sidebar
                    throw new Error(response.data.SidebarStatus);
                    
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

    // Minimize or maximize sidebar
    const MinimizeMaximizeSidebar: (e: React.MouseEvent<HTMLButtonElement>) => void = (e: React.MouseEvent<HTMLButtonElement>): void => {

        // Get the target
        const target = e.target as HTMLElement;

        // Verify if the sidebar is minimized
        if ( target.closest('.fc-dashboard-sidebar')!.classList.contains('fc-minimized-sidebar') ) {

            // Maximize sidebar
            target.closest('.fc-dashboard-sidebar')!.classList.add('fc-maximize-sidebar');            

            // Set a pause
            setTimeout(async (): Promise<void> => {

                // Maximize sidebar
                target.closest('.fc-dashboard-sidebar')!.classList.remove('fc-minimized-sidebar');
                target.closest('.fc-dashboard-sidebar')!.classList.remove('fc-maximize-sidebar');

                // Change the sidebar status
                await changeSidebarStatus(1);

            }, 200);

        } else {

            // Minimize sidebar
            target.closest('.fc-dashboard-sidebar')!.classList.add('fc-member-minimize-sidebar');

            // Set a pause
            setTimeout(async (): Promise<void> => {

                // Minimize sidebar
                target.closest('.fc-dashboard-sidebar')!.classList.add('fc-minimized-sidebar');
                target.closest('.fc-dashboard-sidebar')!.classList.remove('fc-member-minimize-sidebar');

                // Change the sidebar status
                await changeSidebarStatus(0);

            }, 200);

        }

    }

    // Create the function which will open or close dropdown
    const openProfileDropdown: MouseEventHandler = (e: React.MouseEvent<HTMLElement>): void => {
        e.preventDefault();

        // Verify if the dropdown is showed
        if ( document.getElementById('fc-member-profile-menu')!.getAttribute('data-expand') === 'false' ) {

            // Change the dropdown status
            document.getElementById('fc-member-profile-menu')!.setAttribute('data-expand', 'true');

            // Get menu
            const menu: Element = document.getElementById('fc-member-profile-menu')!.getElementsByClassName('fc-dropdown-menu')[0];

            // Remove the position absolute from the menu
            (menu as HTMLElement).style.cssText = `position:fixed;top:inherit;`;

            // Check if the sidebar is minimized
            if ( document.getElementsByClassName('fc-dashboard-sidebar')[0].classList.contains('fc-minimized-sidebar') ) {

                // Set margin
                (menu as HTMLElement).style.margin = `-85px 0 0 55px`;

            } else {

                // Set margin
                (menu as HTMLElement).style.margin = `-153px 0px 0px 5px`;

            }

        } else {

            // Change the dropdown status
            document.getElementById('fc-member-profile-menu')!.setAttribute('data-expand', 'false');

        }

    }

    return (
        <>
            <div className={(memberOptions.SidebarStatus === '0')?'fc-dashboard-sidebar fc-minimized-sidebar':'fc-dashboard-sidebar'}>
                <div className="fc-dashboard-sidebar-header">
                    <h4>
                        <Link href={process.env.NEXT_PUBLIC_SITE_URL as unknown as URL}>
                            {( websiteOptions.DashboardLogoSmall && websiteOptions.DashboardLogoLarge)?(
                                <>
                                    <Image src={websiteOptions.DashboardLogoSmall} layout="fill" alt="Small Logo" className="fc-dashboard-sidebar-header-logo-small" />
                                    <Image src={websiteOptions.DashboardLogoLarge} layout="fill" alt="Large Logo" className="fc-dashboard-sidebar-header-logo-large" />
                                </>                            
                            ):(
                                <>
                                    {getIcon('IconDuo', {className: 'fc-sidebar-logo-icon'})}
                                    <span className="fc-sidebar-logo-text">
                                        {(websiteOptions.WebsiteName !== '')?websiteOptions.WebsiteName:getWord('default', 'default_website_name', memberOptions['Language'])}
                                    </span>                            
                                </>
                            )}
                        </Link>
                    </h4>
                </div>
                <div className="fc-dashboard-sidebar-body">
                    <ul>
                        <li>
                            <Link href="/admin/dashboard" className={(pathname === '/admin/dashboard')?'fc-sidebar-item-active':''} scroll={false}>
                                { getIcon('IconDashboard', {className: 'fc-dashboard-sidebar-icon'}) }
                                <span className="fc-dashboard-sidebar-menu-item">
                                    { getWord('admin', 'admin_dashboard', memberOptions['Language']) }
                                </span>
                            </Link>
                        </li>
                        <li>
                            <Link href="/admin/members" className={(pathname === '/admin/members')?'fc-sidebar-item-active':''} scroll={false}>
                                { getIcon('IconUsers', {className: 'fc-dashboard-sidebar-icon'}) }
                                <span className="fc-dashboard-sidebar-menu-item">
                                    { getWord('admin', 'admin_members', memberOptions['Language']) }
                                </span>
                            </Link>
                        </li>
                        <li>
                            <Link href="/admin/plans" className={(pathname === '/admin/plans')?'fc-sidebar-item-active':''} scroll={false}>
                                { getIcon('IconPlans', {className: 'fc-dashboard-sidebar-icon'}) }
                                <span className="fc-dashboard-sidebar-menu-item">
                                    { getWord('admin', 'admin_plans', memberOptions['Language']) }
                                </span>
                            </Link>
                        </li>
                        <li>
                            <Link href="/admin/transactions" className={(pathname === '/admin/transactions')?'fc-sidebar-item-active':''} scroll={false}>
                                { getIcon('IconAccountBalanceWallet', {className: 'fc-dashboard-sidebar-icon'}) }
                                <span className="fc-dashboard-sidebar-menu-item">
                                    { getWord('admin', 'admin_transactions', memberOptions['Language']) }
                                </span>
                            </Link>
                        </li>
                    </ul>
                </div>
                <div className="fc-dashboard-sidebar-bottom">
                    <Link href="#" className="flex fc-member-picture" onClick={openProfileDropdown}>
                        {(memberOptions.ProfilePhoto !== '')?(
                            <Image src={memberOptions.ProfilePhoto as string} width={36} height={36} alt="Member Photo" className="h-10 w-10 rounded-full" onError={(e: SyntheticEvent<HTMLImageElement, Event>): void => {e.currentTarget.src = '/assets/img/cover.png'}} />
                        ): (
                            <span className="fc-member-picture-cover">
                                {getIcon('IconPerson')}
                            </span>
                        )}
                        <div className="ml-3">
                            <p className="text-sm">{(memberOptions.FirstName && memberOptions.LastName)?memberOptions.FirstName + ' ' + memberOptions.LastName:memberOptions.Email}</p>
                            <p className="text-sm">{ getWord('admin', 'admin_administrator', memberOptions['Language']).toLowerCase() }</p>
                        </div>
                    </Link>
                    <UiDropdown button="" options={ dropdownitems } id="fc-member-profile-menu" />
                    <button type="button" className="fc-member-menu-maximize-minimize-button" onClick={ MinimizeMaximizeSidebar }>
                        { getIcon('IconKeyBoardRight', {className: 'fc-member-menu-maximize-icon'}) }
                        { getIcon('IconKeyBoardLeft', {className: 'fc-member-menu-minimize-icon'}) }
                    </button>                              
                </div>
            </div>
        </>
    );

}

// Export the sidebar layout
export default SidebarLayout;