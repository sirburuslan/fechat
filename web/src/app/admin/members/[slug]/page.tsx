/*
 * @page Member
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This file contains the page member in the administrator panel
 */

'use client'

// Import the React hooks
import { useState, useEffect, useContext, useRef, SyntheticEvent } from 'react';

// Import the Next JS Link component
import Link from 'next/link';

// Import the Next JS Image component
import Image from 'next/image';

// Import axios module
import axios, { AxiosError, AxiosProgressEvent, AxiosResponse } from 'axios';

// Import the UseQuery hook
import { useQuery } from 'react-query';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the incs
import { getIcon, getWord, getOptions, getMonth, calculateTime, updateOptions, showNotification } from '@/core/inc/incIndex';

// Import types
import { typePostHeader, typeOptions } from '@/core/types/typesIndex';

// Import the options for website and member
import { WebsiteOptionsContext, MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Import the ui components
import {UiCalendar} from '@/core/components/general/ui/UiIndex';

// Import the Member Data component
import MemberData from "@/core/components/admin/members/MemberData";

// Create the page content
const Page = ({ params }: { params: { slug: string } }): React.JSX.Element => {

    // Website options
    const {websiteOptions, setWebsiteOptions} = useContext(WebsiteOptionsContext); 

    // Member options
    const {memberOptions, setMemberOptions} = useContext(MemberOptionsContext); 

    // Set a hook for fields value
    const [fields, setFields] = useState({
        MemberId: params.slug,
        ProfilePhoto: '',
        FirstName: '',
        LastName: '',
        Email: '',
        Phone: '',
        Language: '',
        Role: '0',
        Password: '',
        RepeatPassword: ''
    });

    // Set a hook for error message if member can't be reached
    const [memberError, setMemberError] = useState('');

    // Set a hook for events
    const [events, setEvents] = useState<{list: Array<{[key: string]: string}>, time: number}>({
        list: [],
        time: 0
    });

    // Register default value for year
    const [iYear, setIYear] = useState(new Date().getFullYear());

    // Register default value for month
    const [iMonth, setIMonth] = useState(new Date().getMonth()); 

    // Register default value for date
    const [iDate, setIDate] = useState(new Date().getDate()); 

    // Hook to fetch data for Member with useQuery
    const [memberFetchedData, setMemberFetchedData] = useState(false);   

    // Init the reference for the calendar
    const calendarBtn = useRef<HTMLDivElement>(null);

    // Check if document is defined
    if ( typeof document !== 'undefined' ) {

        // Update the page title dynamically
        document.title = getWord('admin', 'admin_member_profile', memberOptions['Language']);

    }

    // Get the member's information
    const memberInfo = async (): Promise<any> => {

        // Set the bearer token
        const token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');

        // Set the headers
        const headers: typePostHeader = {
            headers: {
                Authorization: `Bearer ${token}`
            }
        };            

        // Request the fields value
        const response = await axios.get(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/members/' + params.slug, headers);

        // Process the response
        return response.data;

    };

    // Load events from the database
    const eventsList = async (): Promise<void> => {

        try {

            // Set the bearer token
            const token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');

            // Prepare the headers
            const headers: typePostHeader = {
                headers: {
                    Authorization: `Bearer ${token}`
                }
            };

            // Update the fields
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/events/list', {
                MemberId: fields.MemberId,
                Year: iYear.toString(),
                Month: iMonth.toString(),
                Date: iDate.toString()
            }, headers)

            // Process the response
            .then(async (response: AxiosResponse) => {
    
                // Verify if the response is successfully
                if ( response.data.success ) {

                    // Update the events
                    setEvents(prev => ({
                        list: response.data.events,
                        time: response.data.time
                    }));

                } else {

                    // Update the events
                    setEvents(prev => ({
                        ...prev,
                        list: []
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

    // Request the members info
    const { isLoading, error, data } = useQuery('memberInfo-' + params.slug, memberInfo, {
        enabled: !memberFetchedData
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

    // Monitor the Member data change
    useEffect((): void => {

        // Check if has occurred an error
        if ( error ) {

            // Update member error
            setMemberError((error as Error).message);

        } else if ( (typeof data != 'undefined') && !data.success ) { 
            
            // Update member error
            setMemberError(data.message);

        } else if (data) {

            // Received fields
            const rFields: string[] = Object.keys(data.member as object);

            // Calculate fields length
            const fieldsLength: number = rFields.length;

            // List the member infos
            for ( let o = 0; o < fieldsLength; o++ ) {

                // Verify if rFields[o] is PlanId or Name
                if ((rFields[o] === 'PlanId') || (rFields[o] === 'PlanName')) {
                    continue;
                } else {

                    // Update the field
                    setFields((prev: any) => ({
                        ...prev,
                        [rFields[o]]: data.member![rFields[o]]
                    }));

                }

            }  

            // Set a pause
            setTimeout((): void => {

                // Verify if plan exists in the member data
                if ( (typeof data.member!['PlanId'] !== 'undefined') && (typeof data.member!['PlanName'] !== 'undefined') ) {

                    // Verify if role is not administrator
                    if ( parseInt(data.member!['Role']) === 1 ) {

                        // Get plan's button
                        const planButton: Element | null = document.querySelector('#fc-option-dropdown-plan button');

                        // Verify if plan button exists
                        if ( planButton ) {

                            // Set plan id
                            document.querySelector('#fc-option-dropdown-plan button')!.setAttribute('data-id', data.member!['PlanId']);

                            // Set plan name
                            document.querySelectorAll('#fc-option-dropdown-plan button span')[0].textContent = data.member!['PlanName'];

                        }

                    }

                }                     

            }, 300);

        } 

        // Stop to fetch data
        setMemberFetchedData(true); 
    
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
    const trackClicks = async (e: Event): Promise<void> => {

        // Get the target
        const target = e.target;

        // Check if the click is outside calendar
        if ( (target instanceof Element) && !target.closest('.fc-events-calendar') && calendarBtn.current ) {

            // Change the calendar status
            calendarBtn.current!.closest('.fc-events-calendar')!.setAttribute('data-expand', 'false');

        } else if ( (target instanceof Element) && target.closest('.fc-calendar-selected-date') && (target.nodeName === 'A') ) {

            // Get active date
            const activeDate = new Date(target.getAttribute('data-date')!);
            
            // Set date
            setIDate(activeDate.getDate());

            // Set month
            setIMonth(activeDate.getMonth());    
            
            // Set year
            setIYear(activeDate.getFullYear());
            
        }

    };
    
    /**
     * Images uploader
     */
    const uploadImage = async (e: React.ChangeEvent): Promise<void> => {

        // Select media
        const mediaContainer: HTMLElement | null = e.currentTarget.closest('.fc-profile-photo-area');        

        try {

            // Add fc-uploading-active class
            mediaContainer!.classList.add('fc-uploading-active');

            // Set the bearer token
            const token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');

            // Get the file
            const file = (e.target as HTMLInputElement).files![0];

            // Create an instance for the form data
            const form: FormData = new FormData();
            
            // Set text input
            form.append('file', file); 

            // Upload the image on the server
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/admin/members/image/' + params.slug, form, {
                headers: {
                    'Content-Type': 'multipart/form-data',
                    Authorization: `Bearer ${token}`
                },
                onUploadProgress: (progressEvent: AxiosProgressEvent) => {

                    // Verify if total is numeric
                    if ( typeof progressEvent.total === 'number' ) {

                        // Calculate the progress percentage
                        const progress: number = (progressEvent.loaded / progressEvent.total) * 100;

                        // Set the progress percentage
                        mediaContainer!.style.cssText = `--width: ${progress.toFixed(2)}%`;

                    }

                }
            })

            // Process the response
            .then(async (response: AxiosResponse): Promise<void> => {

                // Remove fc-uploading-active class
                mediaContainer!.classList.remove('fc-uploading-active');

                // Check if the file was uploaded
                if ( response.data.success ) {

                    // Show success notification
                    showNotification('success', response.data.message);

                    // Get the member's information
                    setMemberFetchedData(false);

                    // Verify if was updated data for the logged administrator
                    if ( parseInt(params.slug) === response.data.memberId ) {

                        // Request the options
                        const optionsList: {success: boolean, options?: typeOptions} = await getOptions();

                        // Update memberOptions
                        updateOptions(optionsList, setWebsiteOptions, setMemberOptions);

                    }

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

            // Remove fc-uploading-active class
            mediaContainer!.classList.remove('fc-uploading-active');

            // Check if error is known
            if ( error instanceof Error ) {

                // Show error notification
                showNotification('error', error.message);

            } else {

                // Display in the console the error
                console.log(error);

            }

        }

    };

    /**
     * Catch the calendar button click
     * 
     * @param Event e
     */
    const calendarButtonClick = (e: React.MouseEvent<Element>): void => {
        e.preventDefault();

        // Get the target
        const target = e.target as HTMLElement;

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
    const prevDate = (): void => {
        
        // Get next date
        const nextDate = new Date(new Date((iMonth + 1) + '/' + iDate + '/' + iYear).getTime() - (86400 * 1000));
        
        // Set date
        setIDate(nextDate.getDate());

        // Set month
        setIMonth(nextDate.getMonth());    
        
        // Set year
        setIYear(nextDate.getFullYear());

    };

    // Detect next date click
    const nextDate = (): void => {

        // Get next date
        const nextDate = new Date(new Date((iMonth + 1) + '/' + iDate + '/' + iYear).getTime() + (86400 * 1000));
        
        // Set date
        setIDate(nextDate.getDate());

        // Set month
        setIMonth(nextDate.getMonth());    
        
        // Set year
        setIYear(nextDate.getFullYear());  

    };

    return (
        <>
            {(memberError == '')?(
                <div className="fc-member-container">
                    <div className="grid grid-cols-1 sm:grid-cols-1 md:grid-cols-5 lg:grid-cols-7 xl:grid-cols-7 gap-1 sm:gap-1 md:gap-6 lg:gap-6 xl:gap-6 mb-3">
                        <div className="fc-profile-photo-area mb-10">
                            <div className="flex">
                                <div className="w-full">
                                    <h3 className="fc-section-title">
                                        { getWord('default', 'default_profile_image', memberOptions['Language']) }
                                    </h3>
                                </div>
                            </div>
                            <div className="flex mb-3">
                                <div className="w-full">
                                    <div className="fc-profile-photo fc-profile-photo-green-background flex items-center justify-center">
                                        {(typeof fields.ProfilePhoto === 'string' && fields.ProfilePhoto !== '')?(
                                            <Image src={fields.ProfilePhoto} width={200} height={200} alt="Member Photo" onError={(e: SyntheticEvent<HTMLImageElement, Event>): void => {e.currentTarget.src = '/assets/img/cover.png'}} />
                                        ): (
                                            <div className="fc-profile-photo-cover">
                                                {getIcon('IconPerson')}
                                            </div>
                                        )}
                                    </div>
                                </div>
                            </div>  
                            <div className="flex">
                                <div className="w-full text-center">
                                    <Link href="#" onClick={(e: React.MouseEvent<HTMLAnchorElement>): void => {
                                        e.preventDefault();
                                        ((e.target as Element).closest('.flex')!.getElementsByClassName('fc-profile-upload-file')[0] as HTMLButtonElement).click();
                                    }}>
                                        { getIcon('IconCropOriginal', {className: 'fc-uploading-icon'}) }
                                        { getIcon('IconAddPhotoAlternate', {className: 'fc-default-icon'}) }
                                        { getWord('default', 'default_change_profile_image', memberOptions['Language']) }
                                    </Link>
                                    <form>
                                        <input type="file" accept="image/jpeg,image/gif,image/png" name={"fc-profile-file-input-profile-photo"} id={"fc-profile-file-input-profile-photo"} className="fc-profile-upload-file" onChange={uploadImage} />
                                    </form>
                                </div>
                            </div>                                        
                        </div>
                        <div className="fc-member-options col-span-1 md:col-span-2 lg:col-span-3 xl:col-span-3">
                            <MemberData memberId={params.slug} fields={fields} setFields={setFields} />          
                        </div>
                        <div className="fc-member-events col-span-1 md:col-span-2 lg:col-span-3 xl:col-span-3">
                            <div className="flex">
                                <div className="w-full">
                                    <h3 className="fc-section-title">
                                        { getWord('default', 'default_events', memberOptions['Language']) }
                                    </h3>
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
                                            {(events.list.length > 0)?(events.list.map((event: {[key: string]: string | number}, index: Number): React.JSX.Element => {

                                                // Verify if typeID is 1
                                                if ( event.typeId === 1 ) {

                                                    return (<div className="justify-start items-start p-3 fc-event fc-event-new-member flex" key={event.eventId}>
                                                        {(typeof event.profilePhoto === 'string')?(
                                                            <Image className="h-10 w-10 rounded-full" width={40} height={40} src={event.profilePhoto} alt="Member Photo" onError={(e: SyntheticEvent<HTMLImageElement, Event>): void => {e.currentTarget.src = '/assets/img/cover.png'}} />
                                                        ): (
                                                            <div className="flex fc-event-icon items-center justify-center">
                                                                { getIcon('IconPerson') }
                                                            </div>
                                                        )}
                                                        <div className="fc-event-text">
                                                            <h4>
                                                                <Link href={'/admin/members/' + event.memberId}>{(event.firstName !== '')?event.firstName + ' ' + event.lastName:'#' + event.memberId}</Link> {getWord('admin', 'admin_has_joined', memberOptions['Language'])} {getWord('default', 'default_website_name', memberOptions['Language'])}.
                                                            </h4>
                                                            <h6>{calculateTime(events.time, event.created as number, memberOptions['Language'])}</h6>
                                                        </div>                                    
                                                    </div>);

                                                }

                                                return (<div className="justify-start items-start p-3 fc-event fc-event-new-transaction flex" key={event.eventId}>
                                                    {(typeof event.profilePhoto === 'string')?(
                                                        <Image className="h-10 w-10 rounded-full" src={event.profilePhoto} width={40} height={40} alt="Member Photo" onError={(e: SyntheticEvent<HTMLImageElement, Event>): void => {e.currentTarget.src = '/assets/img/cover.png'}} />
                                                    ): (
                                                        <div className="flex fc-event-icon items-center justify-center">
                                                            { getIcon('IconPerson') }
                                                        </div>
                                                    )}
                                                    <div className="fc-event-text">
                                                        <h4>
                                                            <Link href={'/admin/members/' + event.memberId}>{(event.firstName !== '')?event.firstName + ' ' + event.lastName:'#' + event.memberId}</Link> {getWord('admin', 'admin_has_made_a_purchase', memberOptions['Language'])}
                                                        </h4>
                                                        <h6>{calculateTime(events.time, event.created as number, memberOptions['Language'])}</h6>
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
                    </div>          
                </div>            
            ): (
                <div className="fc-member-container">
                    <div className="fc-member-not-found">
                        <p>{memberError}</p>
                    </div>
                </div>
            )}
        </>

    );

};

// Export the page content
export default Page;