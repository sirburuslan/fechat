/*
 * @page User Profile
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This file contains the page profile in the user panel
 */

'use client'

// Import the React hooks
import { useState, useEffect, useContext, SyntheticEvent } from 'react';

// Import the Next JS Link component
import Link from 'next/link';

// Import the Next JS Image component
import Image from 'next/image';

// Import axios module
import axios, { AxiosError, AxiosProgressEvent, AxiosResponse } from 'axios';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the UseQuery hook
import { useQuery } from 'react-query';

// Import the incs
import { getIcon, getWord, getToken, getOptions, updateOptions, showNotification } from '@/core/inc/incIndex';

// Import types
import { typeToken, typePostHeader, typeOptions } from '@/core/types/typesIndex';

// Import the options for website and member
import { WebsiteOptionsContext, MemberOptionsContext } from '@/core/contexts/OptionsContext';

// Import the Profile Data component
import ProfileData from "@/core/components/user/profile/ProfileData";

// Import the Profile Notifications component
import ProfileNotifications from "@/core/components/user/profile/ProfileNotifications";

// Create the page content
const Page = (): React.JSX.Element => {

    // Website options
    let {websiteOptions, setWebsiteOptions} = useContext(WebsiteOptionsContext); 

    // Member options
    let {memberOptions, setMemberOptions} = useContext(MemberOptionsContext); 

    // Set a hook for fields value
    let [fields, setFields] = useState({
        MemberId: '',
        ProfilePhoto: '',
        FirstName: '',
        LastName: '',
        Email: '',
        Phone: '',
        Language: '',
        Role: '0',
        Password: '',
        RepeatPassword: '',
        NotificationsEnabled: 0,
        NotificationsEmail: ''
    });

    // Set a hook for error message if member can't be reached
    let [profileError, setProfileError] = useState('');

    // Hook to fetch data for Member with useQuery
    let [memberFetchedData, setMemberFetchedData] = useState(false);   

    // Check if document is defined
    if ( typeof document !== 'undefined' ) {

        // Update the page title dynamically
        document.title = getWord('user', 'user_profile', memberOptions['Language']);

    }

    // Get the member's information
    let profileInfo = async (): Promise<any> => {

        // Generate a new csrf token
        let csrfToken: typeToken = await getToken();

        // Check if csrf token is missing
        if ( !csrfToken.success ) {

            // Return error response
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
        let response = await axios.get(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/profile', headers);

        // Process the response
        return response.data;

    };
    
    // Request the profile info
    let { isLoading, error, data } = useQuery('profileInfo', profileInfo, {
        enabled: !memberFetchedData
    });

    // Run some code for the client
    useEffect((): () => void => {

        // Register an event for all clicks
        document.addEventListener('click', trackClicks);      

        return (): void => {

            // Remove the event listener for clicks
            document.removeEventListener('click', trackClicks);           

        };

    }, []);

    // Monitor the Member data change
    useEffect((): void => {

        // Check if has occurred an error
        if ( error ) {

            // Show profile error
            setProfileError((error as Error).message);

        } else if ( (typeof data != 'undefined') && !data.success ) { 
            
            // Show profile error
            setProfileError(data.message);

        } else if (data) {

            // Received fields
            let rFields: string[] = Object.keys(data.member as object);

            // Calculate fields length
            let fieldsLength: number = rFields.length;

            // List the member infos
            for ( let o = 0; o < fieldsLength; o++ ) {

                // Verify if rFields[o] is PlanId or Name
                if ((rFields[o] === 'PlanId') || (rFields[o] === 'PlanName')) {
                    continue;
                } else {

                    // Update the field
                    setFields((prev: { MemberId: string; ProfilePhoto: string; FirstName: string; LastName: string; Email: string; Phone: string; Language: string; Role: string; Password: string; RepeatPassword: string; NotificationsEnabled: number, NotificationsEmail: string }) => ({
                        ...prev,
                        [rFields[o]]: data.member![rFields[o]]
                    }));

                }

            } 

        } 

        // Stop to fetch data
        setMemberFetchedData(true); 
    
    }, [data]);

    /**
     * Track any click
     * 
     * @param Event e
     */
    let trackClicks = (e: Event): void => {

        // Get the target
        let target = e.target;
        
        // Check if the click is inside dropdown
        if ( (target instanceof Element) && target.closest('.fc-option-dropdown') && !target.closest('#fc-option-dropdown-plan') && (target.nodeName === 'A') ) {
            e.preventDefault();
            
            // Get the element's ID
            let elementId: string | null = target.getAttribute('data-id');

            // Get the option
            let option: string | null = target.closest('.fc-extra-option')!.getAttribute('data-option');

            // Verify if option is not null
            if ( option !== null ) {
        
                // Update the field
                setFields((prev: any) => ({
                    ...prev,
                    [option!]: elementId as string
                }));

            }

        }

    };

    /**
     * Images uploader
     */
    let uploadImage = async (e: React.ChangeEvent): Promise<void> => {

        // Select media
        let mediaContainer: HTMLElement | null = e.currentTarget.closest('.fc-profile-photo-area');        

        try {

            // Add fc-uploading-active class
            mediaContainer!.classList.add('fc-uploading-active');

            // Set the bearer token
            let token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');

            // Get the file
            let file = (e.target as HTMLInputElement).files![0];

            // Create an instance for the form data
            let form: FormData = new FormData();
            
            // Set text input
            form.append('file', file);   

            // Generate a new csrf token
            let csrfToken: typeToken = await getToken();

            // Check if csrf token is missing
            if ( !csrfToken.success ) {

                // Show error notification
                throw new Error(getWord('errors', 'error_csrf_token_not_generated', memberOptions['Language']));

            }

            // Upload the image on the server
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/image', form, {
                headers: {
                    'Content-Type': 'multipart/form-data',
                    Authorization: `Bearer ${token}`,
                    CsrfToken: csrfToken.token
                },
                onUploadProgress: (progressEvent: AxiosProgressEvent) => {

                    // Verify if total is numeric
                    if ( typeof progressEvent.total === 'number' ) {

                        // Calculate the progress percentage
                        let progress: number = (progressEvent.loaded / progressEvent.total) * 100;

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

                    // Request the options
                    let optionsList: {success: boolean, options?: typeOptions} = await getOptions();

                    // Update memberOptions
                    updateOptions(optionsList, setWebsiteOptions, setMemberOptions);

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

    return (
        <>
            {(profileError == '')?(
                <div className="fc-profile-container">
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
                                    <div className="fc-profile-photo fc-profile-photo-blue-background flex items-center justify-center">
                                        {(fields.ProfilePhoto !== '')?(
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
                                    <Link href="#" className="text-slate-200" onClick={(e: React.MouseEvent<HTMLAnchorElement>): void => {
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
                            <ProfileData fields={fields} setFields={setFields} />          
                        </div>
                        <div className="fc-member-events col-span-1 md:col-span-2 lg:col-span-3 xl:col-span-3">
                            <ProfileNotifications fields={fields} setFields={setFields} /> 
                        </div>
                    </div>          
                </div>            
            ): (
                <div className="fc-profile-container">
                    <div className="fc-profile-not-found">
                        <p>{profileError}</p>
                    </div>
                </div>
            )}
        </>

    );

};

// Export the page content
export default Page;