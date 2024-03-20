/*
 * @page Thread
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-04
 *
 * This file contains the page for a single thread
 */

'use client'

// Import the react hooks
import { useState, useContext, useEffect, useRef } from 'react';

// Import the Next JS Link component
import Link from 'next/link';

// Import axios module
import axios, { AxiosError, AxiosProgressEvent, AxiosResponse } from 'axios';

// Import the UseQuery hook
import { useQuery } from 'react-query';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the incs
import { getIcon, getWord, getToken, getMonth, showNotification, unescapeRegexString } from '@/core/inc/incIndex';

// Import types
import { typeToken, typePostHeader } from '@/core/types/typesIndex';

// Import the options for website and member
import {WebsiteOptionsContext, MemberOptionsContext} from '@/core/contexts/OptionsContext';

// Import the Person icon
import IconPerson from '@/core/icons/collection/IconPerson.ts';

// Create the page content
const Page = ({params}: {params: {slug: string}}): React.JSX.Element => {

    // Website options
    let {websiteOptions} = useContext(WebsiteOptionsContext); 

    // Member options
    let {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);

    // Set a hook for error message if thread can't be reached
    let [threadError, setThreadError] = useState<string>();

    // Hook for messages
    let [htmlMessages, setHtmlMessages] = useState('');

    // Hook for guest's information
    let [guestInfo, setGuestInfo] = useState({
        GuestId: '',
        Name: '',
        Email: '',
        Ip: '',
        Latitude: '',
        Longitude: ''
    });    

    // Hook for new message
    let [message, setMessage] = useState('');

    // Set a hook for pagination parameters
    let [pagination, setPagination] = useState({
        Page: 1
    });  

    // Set a hook for message input
    let [inputPause, updateInputPause] = useState(0);  

    // Hook to fetch data with useQuery
    let [fetchedData, setFetchedData] = useState(false);

    // Messages ids container
    let messagesIds = useRef<string[]>([]);

    // Temporary dates container
    let tempDates = useRef<{[key: string]: number}>({});

    // Typing active container
    let typingActive = useRef<NodeJS.Timeout>();

    /*
     * Schedule animation
     * 
     * @param funcion fun contains the function
     * @param integer interval contains time
     */
    let scheduleAnimation = ($fun: () => void, interval: number): void => {

        // Verify if an event was already scheduled
        if (typingActive.current) {

            // Clear the previous timeout
            clearTimeout(typingActive.current);

        }

        // Add to queue
        typingActive.current = setTimeout($fun, interval);
        
    };

    // Get the thread's information
    let threadInfo = async (): Promise<any> => {

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
        let response = await axios.get(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/threads/' + params.slug, headers);

        // Process the response
        return response.data;

    };

    // Get the thread's messages
    let threadMessages = async (): Promise<void> => {

        try {

            // Generate a new csrf token
            let csrfToken: typeToken = await getToken();

            // Check if csrf token is missing
            if ( !csrfToken.success ) {

                // Show error notification
                throw new Error(getWord('errors', 'error_csrf_token_not_generated', memberOptions['Language']));

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

            // Request the messages
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/threads/' + params.slug + '/messages', pagination, headers)

            // Process the response
            .then((response: AxiosResponse) => {

                // Verify if the response is successfully
                if ( response.data.success ) {

                    // Set the messages in the queue to be displayed
                    elementsList(response.data.result);

                } else {

                    // Set error notification
                    throw new Error(response.data.message);
                    
                }

            })

            // Catch the error message
            .catch((e: AxiosError): void => {

                // Set error notification
                throw new Error(e.message);

            });

        } catch (e: unknown) {

            // Check if error is known
            if ( e instanceof Error ) {

                // Set thread error message
                setThreadError(e.message);

            } else {

                // Set thread error message
                setThreadError(getWord('user', 'user_thread_was_not_found', memberOptions['Language']));
                
            }

        }

    };

    /**
     * Elements list which should be displayed
     * 
     * @param object messages
     */
    let elementsList = (messages: any): void => {

        // Initial chat height
        let initChatHeight = 0;

        // Reverse the messages
        let elements = messages.elements.reverse();

        // Verify if temporary dates missing
        if ( Object.keys(tempDates.current).length > 0 ) {

            // Parse data
            let created = new Date(elements[0].created * 1000);

            // Extrate the date
            let date: string = created.getDate().toString().padStart(2, '0');

            // Extrate the month
            let month: string = (created.getMonth() + 1).toString().padStart(2, '0');   
            
            // Extrate the year
            let year: string = created.getFullYear().toString();   
            
            // Select the added date in the chat
            let dateHeader = document.querySelector('.fc-date[data-date="' + date + '-' + month + '-' + year + '"]');
            
            // Verify if date exists already
            if ( dateHeader ) {

                // Remove the header
                dateHeader.remove();

                // Update the messages list
                setHtmlMessages(prevHtml => prevHtml.replace(dateHeader!.textContent!, ''));

                // Delete temporary date
                delete tempDates.current[date + '-' + month + '-' + year];

            }

        }

        // Messages container
        let messagesList: string = '';

        // List the messages
        for ( let element of elements ) {

            // Verify if the message is already displayed
            if ( messagesIds.current.includes(element.messageId) ) {
                continue;
            }

            // Parse data
            let created = new Date(element.created * 1000);

            // Extrate the date
            let date: string = created.getDate().toString().padStart(2, '0');

            // Extrate the month
            let month: string = (created.getMonth() + 1).toString().padStart(2, '0');   
            
            // Extrate the year
            let year: string = created.getFullYear().toString();

            // Message date container
            let messageDate: string = '';

            // Verify if the date is temporary saved
            if ( typeof tempDates.current[date + '-' + month + '-' + year] === 'undefined' ) {

                // Set message date
                messageDate = `<div class="fc-date" data-date="${date + '-' + month + '-' + year}">
                    ${date + ' ' + getMonth(month) + ' ' + year}
                </div>`;

                // Save temporary date
                tempDates.current[date + '-' + month + '-' + year] = element.messageId;

            }

            // Save message id
            messagesIds.current.push(element.messageId);

            // Message content
            let content: string = '';

            // Verify if message exists
            if ( element.message ) {

                // Set message as content
                content = `<p>${unescapeRegexString(element.message.replaceAll(/\\n/g, '</p><p>'))}</p>`;

            } else if ( element.attachments.length > 0 ) {

                // List attached files
                for ( let attachment of element.attachments ) {

                    // Append image
                    content += `<a href="${attachment}" target="_blank">
                        <img src="${attachment}" alt="Attached file" onError="this.src='${process.env.NEXT_PUBLIC_SITE_URL}assets/img/cover.png';" />
                    </a>`;

                }

            }

            // Check if is the member's message
            if ( element.memberId > 0 ) {

                // User's photo
                let userPhoto = IconPerson();

                // Verify if the member has selected a photo
                if (memberOptions.ProfilePhoto !== '') {
                    userPhoto = `<img src="${memberOptions.ProfilePhoto}" alt="User Photo" onError="this.src='${process.env.NEXT_PUBLIC_SITE_URL}assets/img/cover.png';" />`;
                }

                // Add messages to the list
                messagesList += `<li data-message="${element.messageId}">
                    ${messageDate}
                    <div class="flex">
                        <div class="fc-message">
                            ${content}
                        </div>
                        <div class="fc-author-photo flex flex-col">
                            <div class="flex-grow"></div>
                            ${userPhoto}
                        </div>                
                    </div>
                </li>`;

            } else {

                // Add messages to the list
                messagesList += `<li class="fc-guest-message" data-message="${element.messageId}">
                    ${messageDate}
                    <div class="flex">
                        <div class="fc-author-photo flex flex-col">
                            <div class="flex-grow"></div>
                            ${IconPerson()}
                        </div> 
                        <div class="fc-message">
                            ${content}
                        </div>
                    </div>
                </li>`;

            }

        }
        
        // Check if the page is greater than 1
        if ( pagination.Page > 1 ) {

            // Replace Initial chat height
            initChatHeight = document.getElementsByClassName('fc-thread-chat-body')[0].scrollHeight;

            // Update the messages list
            setHtmlMessages(prevHtml => messagesList + prevHtml);

        } else {

            // Set messages
            setHtmlMessages(messagesList);

        }

        // Select the chat body
        let chatBody: Element = document.getElementsByClassName('fc-thread-chat-body')[0];

        // Check if the div is scrolled up to the bottom
        if ( (((chatBody as HTMLElement).offsetHeight + chatBody.scrollTop) >= chatBody.scrollHeight) || (chatBody.scrollHeight < 200) ) {

            // Wait until messages will be appended
            setTimeout((): void => {

                // Select the chat body
                let chatBody: Element = document.getElementsByClassName('fc-thread-chat-body')[0];

                // Scroll the messages to the bottom
                chatBody.scrollTop = chatBody.scrollHeight;

            }, 1);

        } else {

            // Wait until messages will be appended
            setTimeout((): void => {

                // Select the chat body
                let chatBody: Element = document.getElementsByClassName('fc-thread-chat-body')[0];

                // Scroll the messages to the bottom
                chatBody.scrollTop = chatBody.scrollHeight - initChatHeight - 44;

            }, 1);

        }

        // Verify if there are more pages
        if ( (messages.page * 10) < messages.total ) {

            // Show the navigation button
            document.getElementsByClassName('fc-chat-pagination')[0].classList.add('fc-chat-pagination-show');

        } else {

            // Add the navigation button
            document.getElementsByClassName('fc-chat-pagination')[0].classList.remove('fc-chat-pagination-show');

        }

    };

    // Get the guest map
    let guestMap = (guestInfo: any): React.JSX.Element => {

        // Verify if google map is configured
        if ( (websiteOptions.GoogleMapsEnabled !== '1') || (websiteOptions.GoogleMapsKey === '') ) {
            return <></>;            
        }

        // Verify if guest latitude and longitude exists
        if ( (guestInfo.Longitude === '') || (guestInfo.Latitude === '') ) {
            return <></>; 
        }

        return (
            <div className="fc-thread-guest-location">
                <iframe
                width="600"
                height="450"
                loading="lazy"
                allowFullScreen
                src={"https://www.google.com/maps/embed/v1/view?key=" + websiteOptions.GoogleMapsKey + "&center=" + guestInfo.Latitude + "," + guestInfo.Longitude + "&zoom=15"}>
                </iframe>
            </div>
        );

    };

    // Request the thread information
    let { isLoading, error, data } = useQuery('threadInfo-' + params.slug, threadInfo, {
        enabled: !fetchedData
    });

    // Run code after content load
    useEffect((): () => void => {

        // Set the bearer token
        let token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');

        // Create a Web Socket connection
        let socket = new WebSocket(process.env.NEXT_PUBLIC_API_URL?.replace('http', 'ws') + 'api/v1/user/websocket');

        // Register an event when the connection opens
        socket.onopen = (event: Event): void => {

            // Prepare the data to send
            let fields = {
                AccessToken: token,
                ThreadId: params.slug
            };

            // Send fields as string
            socket.send(JSON.stringify(fields));

        };

        // Catch the messages
        socket.onmessage = (event: MessageEvent<any>): void => {

            // Decode the event data
            let eventData = JSON.parse(event.data);

            // Check if unseen messages exists
            if ( typeof eventData.unseen !== 'undefined' ) {

                // Select the chat body
                let chatBody: Element = document.getElementsByClassName('fc-thread-chat-body')[0];

                // Check if the div is scrolled up to the bottom
                if ( (((chatBody as HTMLElement).offsetHeight + chatBody.scrollTop) >= chatBody.scrollHeight) || (chatBody.scrollHeight < 200) ) {

                    // Reset the pagination
                    pagination.Page = 1;

                    // Reset Messages ids
                    messagesIds.current = [];

                    // Reset temporary dates
                    tempDates.current = {};

                    // Get the unseen messages
                    threadMessages();

                } else {

                    // Show the unseen notification
                    document.getElementsByClassName('fc-chat-unseen-messages')[0].classList.add('fc-chat-unseen-messages-show');

                }

            } else if ( typeof eventData.typing !== 'undefined' ) {

                // Show typing animation
                document.getElementsByClassName('fc-message-typing')[0].classList.add('fc-message-typing-show');

                // Schedule a search
                scheduleAnimation((): void => { 
                    
                    // Remove typing animation
                    document.getElementsByClassName('fc-message-typing')[0].classList.remove('fc-message-typing-show');                                

                }, 3000);
                
            }

        };

        // Register an event for clicks tracking
        document.addEventListener('click', trackClicks);
        
        return (): void => {

            // Web Socket connection
            socket.close();

            // Remove event for clicks tracking
            document.removeEventListener('click', trackClicks);

        };

    }, []);

    // Monitor the data change
    useEffect((): void => {

        // Check if has occurred an error
        if ( error ) {
console.log(1);
            // Set thread error message
            setThreadError((error as Error).message);

        } else if ( (typeof data != 'undefined') && !data.success ) {
console.log(2);
            // Set thread error message
            setThreadError(data.message);

        } else if (data) {
console.log(data);
            // Check if guest exists
            if ( typeof data.guest !== 'undefined' ) {

                // Set page's title
                document.title = data.guest.guestName;

                // Set guest
                setGuestInfo({
                    GuestId: data.guest.guestId,
                    Name: data.guest.guestName,
                    Email: data.guest.guestEmail,
                    Ip: data.guest.guestIp,
                    Latitude: data.guest.guestLatitude,
                    Longitude: data.guest.guestLongitude
                }); 

            }
            
            // Check if thread exists
            if ( typeof data.thread !== 'undefined' ) {

                // Verify if the thread has messages
                if ( typeof data.thread.messages !== 'undefined' ) {

                    // Check if member profile is missing
                    if ( !memberOptions.ProfilePhoto ) {

                        // Check if the thread has user photo
                        if ( typeof data.thread.userPhoto !== 'undefined' ) {

                            // Set user photo
                            memberOptions.ProfilePhoto = data.thread.userPhoto;

                        }

                    }

                    // Set the messages in the queue to be displayed
                    elementsList(data.thread.messages);

                }

            }

        } 

        // Stop to fetch data
        setFetchedData(true); 

    }, [data]);

    /**
     * Track any click
     * 
     * @param Event e
     */
    let trackClicks = async (e: Event): Promise<void> => {

        // Get the target
        let target = e.target;

        // Check if the click is inside dropdown
        if ( (target instanceof Element) && target.classList && target.classList.contains('fc-chat-unseen-messages') ) {
            e.preventDefault();

            // Select the chat body
            let chatBody: Element = document.getElementsByClassName('fc-chat-body')[0];

            // Scroll the messages to the bottom
            chatBody.scrollTop = chatBody.scrollHeight;

            // Hide the unseen messages button
            document.getElementsByClassName('fc-chat-unseen-messages')[0].classList.remove('fc-chat-unseen-messages-show');            

        } else if ( (target instanceof Element) && target.classList && target.classList.contains('fc-chat-pagination') ) {
            e.preventDefault();

            // Get the page
            let page: string | null = target.getAttribute('data-page');

            // Set pagination's page
            pagination.Page = parseInt(page!);

            // Update the pagination
            setPagination(pagination);

            // Get the chat messages
            threadMessages();

        }

    };

    /**
     * Handle chat message change
     * 
     * @param Event e 
     */
    let handleChange: React.FormEventHandler<HTMLTextAreaElement> = async (e): Promise<void> => {

        // Update the message value
        setMessage(e.currentTarget.value);

        // Verify if inputPause is less than 3 seconds
        if ( Date.now()/1000 > inputPause ) {

            try {

                // Generate a new csrf token
                let csrfToken: typeToken = await getToken();

                // Check if csrf token is missing
                if ( !csrfToken.success ) {

                    // Show error notification
                    throw new Error(getWord('errors', 'error_csrf_token_not_generated', memberOptions['Language']));

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

                // Request the messages
                await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/threads/' + params.slug + '/typing', null, headers)

                // Process the response
                .then((response: AxiosResponse) => {

                    // Verify if the response is successfully
                    if ( !response.data.success ) {

                        // Set error notification
                        throw new Error(response.data.message);
                        
                    }

                })

                // Catch the error message
                .catch((e: AxiosError): void => {

                    // Set error notification
                    throw new Error(e.message);

                });

            } catch ( error: unknown ) {

                console.log(error);

            }

            // Update input pause
            updateInputPause((Date.now()/1000 + 3));

        }

    };

    /**
     * Images uploader
     * 
     * @param event e
     */
    let uploadImages = async (e: React.ChangeEvent): Promise<void> => {

        // Select progress bar
        let progressBar = e.currentTarget.closest('.fc-thread-chat-footer')!.getElementsByClassName('fc-progress-bar')[0] as HTMLElement;
        
        // Show progress box
        progressBar.closest('.fc-chat-uploading-attachments')!.classList.add('fc-chat-uploading-attachments-show');

        try {

            // Check if there are more than 3 images
            if ( (e.target as HTMLInputElement).files!.length > 3 ) {

                // Show error notification
                throw new Error(getWord('errors', 'errors_only_3_images_allowed'));

            }

            // Get the files
            let files: FileList | null = (e.target as HTMLInputElement).files;

            // Create an instance for the form data
            let form: FormData = new FormData();

            // List the files
            for ( let file of files! ) {

                // Append the file
                form.append('files', file); 

            }  

            // Generate a new csrf token
            let csrfToken: typeToken = await getToken();

            // Check if csrf token is missing
            if ( !csrfToken.success ) {

                // Show error notification
                throw new Error(getWord('errors', 'error_csrf_token_not_generated'));

            }

            // Set the bearer token
            let token: string | number | boolean | object | null = SecureStorage.getItem('fc_jwt');

            // Upload the images on the server
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/messages/attachments/' + params.slug, form, {
                headers: {
                    ContentType: `multipart/form-data`,
                    Authorization: `Bearer ${token}`,
                    CsrfToken: csrfToken.token
                },
                onUploadProgress: (progressEvent: AxiosProgressEvent) => {

                    // Verify if total is numeric
                    if ( typeof progressEvent.total === 'number' ) {

                        // Calculate the progress percentage
                        let progress: number = (progressEvent.loaded / progressEvent.total) * 100;

                        // Set the progress percentage
                        progressBar!.style.width = `${progress.toFixed(2)}%`;
                        document.querySelector('.fc-chat-uploading-attachments p')!.textContent = `${progress.toFixed(2)}%`;

                    }

                }
            })

            // Process the response
            .then((response: AxiosResponse) => {

                // Hide progress box
                progressBar.closest('.fc-chat-uploading-attachments')!.classList.remove('fc-chat-uploading-attachments-show');

                // Set a pause
                setTimeout((): void => {

                    // Reset the progress bar
                    progressBar!.style.width = `0`;
                    document.querySelector('.fc-chat-uploading-attachments p')!.textContent = `0%`;

                }, 1000);
                
                // Check if the file was uploaded
                if ( response.data.success ) {

                    // Reset the pagination
                    pagination.Page = 1;

                    // Reset Messages ids
                    messagesIds.current = [];

                    // Reset temporary dates
                    tempDates.current = {};

                    // Reload the chat messages
                    threadMessages();

                } else {

                    // Show error notification
                    throw new Error(response.data.message);

                }

            })
            
            // Catch the error message
            .catch((error: AxiosError): void => {

                // Set a pause
                setTimeout((): void => {

                    // Reset the progress bar
                    progressBar!.style.width = `0`;
                    document.querySelector('.fc-chat-uploading-attachments p')!.textContent = `0%`;

                }, 1000);

                // Check if message exists
                if ( typeof error.message !== 'undefined' ) {

                    // Show error notification
                    throw new Error(error.message);

                }

            });

        } catch (error: unknown) {

            // Hide progress box
            progressBar.closest('.fc-chat-uploading-attachments')!.classList.remove('fc-chat-uploading-attachments-show');

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

    // Handle the form submit
    let formSubmit: React.FormEventHandler = async (e): Promise<void> => {
        e.preventDefault();

        try {

            // Generate a new csrf token
            let csrfToken: typeToken = await getToken();

            // Check if csrf token is missing
            if ( !csrfToken.success ) {

                // Show error notification
                throw new Error(getWord('errors', 'error_csrf_token_not_generated'));

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
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/user/messages', {
                ThreadId: params.slug,
                Message: message
            }, headers)

            // Process the response
            .then((response: AxiosResponse) => {

                // Verify if the response is successfully
                if ( response.data.success ) {

                    // Check if thread secret is saved
                    if ( !SecureStorage.getItem('fc_website_thread_secret') ) {

                        // Save the thread secret
                        SecureStorage.setItem('fc_website_thread_secret', response.data.thread.threadSecret);

                    }

                    // Empty message
                    setMessage('');

                    // Reset the pagination
                    pagination.Page = 1;

                    // Reset Messages ids
                    messagesIds.current = [];

                    // Reset temporary dates
                    tempDates.current = {};

                    // Reload the chat messages
                    threadMessages();
                    
                } else if ( typeof response.data.Message !== 'undefined' ) {

                    // Set error notification
                    throw new Error(response.data.Message[0]);                  

                } else {

                    // Set error notification
                    throw new Error(response.data.message);
                    
                }

            })

            // Catch the error message
            .catch((e: AxiosError): void => {

                // Set error notification
                throw new Error(e.message);

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

    };

    return (
        <>
            {(typeof threadError === 'undefined')?(
                <div className="fc-thread-container">
                    <div className="grid grid-cols-2 sm:grid-cols-2 md:grid-cols-2 lg:grid-cols-4 xl:grid-cols-4">
                        <div className="fc-thread-chat col-span-2 md:col-span-2 lg:col-span-2 xl:col-span-3">
                            <div className="fc-thread-chat-header">
                                <Link href="/user/threads" className="fc-threads-go-back">
                                    { getIcon('IconWest', {className: 'fc-threads-go-back-icon'}) }
                                    { getWord('user', 'user_all_threads', memberOptions['Language'])  }
                                </Link>
                            </div>
                            <div className="fc-thread-chat-body">
                                <Link href="#" className="fc-chat-pagination" data-page={(pagination.Page + 1)}>
                                    { getIcon('IconAutorenew', {className: 'fc-load-more-icon'}) }
                                    { getWord('user', 'user_load_older_messages', memberOptions['Language'])  }
                                </Link>
                                <ul className="fc-chat-messages" dangerouslySetInnerHTML={{__html: htmlMessages}} />
                            </div>
                            <div className="fc-thread-chat-footer">
                                <div className="fc-chat-uploading-attachments">
                                    <div className="w-full">
                                        <div className="flex justify-between">
                                            <h3>{ getWord('default', 'default_uploading')  }</h3>
                                            <p>0%</p>
                                        </div>
                                    </div>
                                    <div className="w-full">
                                        <div className="w-full rounded-full h-1 dark:bg-gray-700 fc-progress">
                                            <div className="bg-blue-600 h-1 rounded-full fc-progress-bar"></div>
                                        </div>
                                    </div>                            
                                </div>
                                <Link href="#" className="fc-chat-unseen-messages">
                                    { getIcon('IconArrowDownward', {className: 'fc-show-unseen-icon'}) }
                                    { getWord('user', 'user_unseen_messages')  }
                                </Link>
                                <form className="grid grid-cols-12 fc-thread-chat-input" onSubmit={formSubmit}>
                                    <div>
                                        <button type="button" onClick={(e: React.MouseEvent<Element>): void => {
                                                ((e.target as Element).closest('.fc-thread-chat-footer')!.getElementsByClassName('fc-thread-chat-message-file')[0] as HTMLButtonElement).click();
                                            }}>
                                            { getIcon('IconAttachment') }
                                        </button>
                                    </div>
                                    <div className="col-span-10">
                                        <textarea placeholder={ getWord('user', 'user_type_a_message', memberOptions['Language']) } value={message} id="fc-thread-chat-message" onInput={handleChange}></textarea>
                                    </div>
                                    <div>
                                        <button type="submit">
                                            { getIcon('IconSend') }
                                        </button>
                                    </div>                                    
                                </form>  
                                <form id="fc-chat-upload-form">
                                    <input type="file" accept="image/jpeg,image/gif,image/png" name="fc-thread-chat-message-file" multiple className="fc-thread-chat-message-file" id="fc-thread-chat-message-file" onChange={uploadImages} />
                                </form>                                
                            </div>                            
                        </div>
                        <div className="fc-thread-info col-span-2 md:col-span-2 lg:col-span-2 xl:col-span-1">
                            <div className="fc-thread-guest-info">
                                <div className="fc-thread-guest-icon">
                                    {getIcon('IconPerson')}                                 
                                </div>
                                <div className="fc-thread-guest-name">
                                    {guestInfo.Name}  <div className="fc-message-typing"></div>                           
                                </div>
                                <div className="fc-thread-guest-email">
                                    <a href={"mailto:" + guestInfo.Email}>
                                        {guestInfo.Email}
                                    </a>                             
                                </div>                                
                            </div>
                            {guestMap(guestInfo)}                            
                        </div>
                    </div>          
                </div> 
            ): (
                <div className="fc-thread-container">
                    <div className="fc-thread-not-found">
                        <p>{threadError}</p>
                    </div>
                </div>
            )}
        </>

    );

}

// Export the page
export default Page;