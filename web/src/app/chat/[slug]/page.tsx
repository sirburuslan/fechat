/*
 * @page Chat
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains the chat box
 */

'use client'

// Import the React's hooks
import { MouseEventHandler, useState, useEffect, useRef } from 'react';

// Import the Next JS Link component
import Link from 'next/link';

// Import axios module
import axios, { AxiosError, AxiosProgressEvent, AxiosResponse } from 'axios';

// Import the Secure Storage module for react
import SecureStorage from 'react-secure-storage';

// Import the incs
import { getIcon, getWord, getToken, getMonth, showNotification, unescapeRegexString } from '@/core/inc/incIndex';

// Import types
import { typeToken, typePostHeader } from '@/core/types/typesIndex';

// Import the Person icon
import IconPerson from '@/core/icons/collection/IconPerson.ts';

// Import styles
import '@/styles/chat/_main.scss';

// Create the page content
const Page = ({params}: {params: {slug: string}}): React.JSX.Element => {

    // Chat Disabled status
    let [chatDisabled, disableChat] = useState(0);    

    // Show chat state
    let [chat, showChat] = useState('');

    // Hook for chat header
    let [chatHeader, updateChatHeader] = useState(<>
        <span className="fc-member-picture-cover">
            {getIcon('IconPerson')}
        </span>
        <span></span>
    </>);

    // Hook for messages
    let [htmlMessages, setHtmlMessages] = useState('');

    // Hook for new message
    let [message, setMessage] = useState('');

    // Set a hook for pagination parameters
    let [pagination, setPagination] = useState({
        Page: 1
    });  

    // Init Chat fields
    let [initChat, setInitChat] = useState({
        name: '',
        email: '',
        message: ''
    });

    // Set a hook for errors value
    let [errors, setErrors] = useState({});   
    
    // Set a hook for chat start
    let [startChat, setStartChat] = useState(1); 

    // Set a hook for message input
    let [inputPause, updateInputPause] = useState(0);  

    // Messages ids container
    let messagesIds = useRef<string[]>([]);

    // Temporary dates container
    let tempDates = useRef<{[key: string]: number}>({});

    // Websocket container
    let websocket = useRef<WebSocket>(); 
    
    // Typing active container
    let typingActive = useRef<NodeJS.Timeout>();

    // Check if document is defined
    if ( typeof document !== 'undefined' ) {

        // Update the page title dynamically
        document.title = getWord('public', 'public_chat');

        // Check if thread secret is saved
        if ( SecureStorage.getItem('fc_website_thread_secret_' + params.slug) ) {

            // Verify if chat exists
            if ((chat !== '') && (htmlMessages != '')) {

                // Verify if websocket.current has value
                if ( !(websocket.current instanceof WebSocket) ) {

                    // Create a Web Socket connection
                    websocket.current = new WebSocket(process.env.NEXT_PUBLIC_API_URL?.replace('http', 'ws') + 'api/v1/websocket');

                    // Register an event when the connection opens
                    websocket.current.onopen = (event: Event): void => {

                        // Prepare the data to send
                        let fields = {
                            WebsiteId: params.slug,
                            ThreadSecret: SecureStorage.getItem('fc_website_thread_secret_' + params.slug)
                        };

                        // Send fields as string
                        websocket.current!.send(JSON.stringify(fields));

                    };

                    // Catch the messages
                    websocket.current.onmessage = (event: MessageEvent<any>): void => {

                        // Decode the event data
                        let eventData = JSON.parse(event.data);

                        // Check if unseen messages exists
                        if ( typeof eventData.unseen !== 'undefined' ) {

                            // Select the chat body
                            let chatBody: HTMLCollectionOf<Element> = document.getElementsByClassName('fc-chat-body');

                            // Check if chat body exists
                            if ( (chatBody.length > 0) && (chatBody[0] instanceof HTMLElement) ) {

                                // Check if the div is scrolled up to the bottom
                                if ( ((chatBody[0].offsetHeight + chatBody[0].scrollTop) >= chatBody[0].scrollHeight) || (chatBody[0].scrollHeight < 200) ) {

                                    // Reset the pagination
                                    pagination.Page = 1;

                                    // Reset Messages ids
                                    messagesIds.current = [];

                                    // Reset temporary dates
                                    tempDates.current = {};

                                    // Get the unseen messages
                                    getMessages();

                                    // Hide the unseen notification
                                    document.getElementsByClassName('fc-chat-unseen-messages')[0].classList.remove('fc-chat-unseen-messages-show');

                                } else {

                                    // Show the unseen notification
                                    document.getElementsByClassName('fc-chat-unseen-messages')[0].classList.add('fc-chat-unseen-messages-show');

                                }

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

                }

            } else {

                // Verify if websocket.current has value
                if ( websocket.current instanceof WebSocket ) {

                    // Web Socket connection
                    websocket.current.close();

                    // Cancel Websocket instance
                    websocket.current = undefined;

                }

            }

        }

    };

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

    // Get the website's information
    let websiteInfo = async (): Promise<void> => {

        try {

            // Generate a new csrf token
            let csrfToken: typeToken = await getToken();

            // Set the headers
            let headers: typePostHeader = {
                headers: {
                    CsrfToken: csrfToken.token
                }
            };            

            // Request the fields value
            await axios.get(process.env.NEXT_PUBLIC_API_URL + 'api/v1/websites/' + params.slug, headers)

            // Process the response
            .then((response: AxiosResponse) => {

                // Verify if the response is successfully
                if ( response.data.success ) {

                    // Update the chat header
                    updateChatHeader(
                        <>
                            {(response.data.website.ProfilePhoto !== '')?(
                                <img className="h-10 w-10 rounded-full" src={response.data.website.ProfilePhoto as string} alt="Member Photo" onError={(e) => {e.currentTarget.src = '/assets/img/cover.png';}} />
                            ):(
                                <span className="fc-member-picture-cover">
                                    {getIcon('IconPerson')}
                                </span>                            
                            )}
                            <span>
                                {(response.data.website.Header !== '')?(response.data.website.Header):(response.data.website.FirstName + ' ' + response.data.website.LastName)}
                            </span>                        
                        </>
                    );

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

            // Disable chat
            disableChat(1);
            
            // Check if error is known
            if ( e instanceof Error ) {

                // Set website error message
                console.log('ChatError: ' + e.message);

            } else {

                // Set website error message
                console.log('ChatError: ' + getWord('user', 'user_website_was_not_found'));
                
            }

        }

    };

    // Get the messages from the database
    let getMessages = async (): Promise<void> => {

        try {

            // Initial chat height
            let initChatHeight = 0;

            // Generate a new csrf token
            let csrfToken: typeToken = await getToken();

            // Check if csrf token is missing
            if ( !csrfToken.success ) {

                // Show error notification
                throw new Error(getWord('errors', 'error_csrf_token_not_generated'));

            }

            // Set the headers
            let headers: typePostHeader = {
                headers: {
                    CsrfToken: csrfToken.token
                }
            };

            // Request the fields value
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/messages/list', {
                WebsiteId: params.slug,
                ThreadSecret: SecureStorage.getItem('fc_website_thread_secret_' + params.slug),
                Page: pagination.Page
            }, headers)

            // Process the response
            .then((response: AxiosResponse) => {

                // Verify if the response is successfully
                if ( response.data.success ) {

                    // Reverse the messages
                    let elements = response.data.result.elements.reverse();

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
                            content = `<p>${unescapeRegexString(element.message.split("\\n").join('</p><p>'))}</p>`;

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

                            // Add messages to the list
                            messagesList += `<li class="fc-guest-message" data-message="${element.messageId}">
                                ${messageDate}
                                <div class="flex">
                                    <div class="fc-author-photo flex flex-col">
                                        <div class="flex-grow"></div>
                                        <img src="https://i.imgur.com/sgFTCPQ.jpeg" />
                                    </div>
                                    <div class="fc-message">
                                        ${content}
                                    </div>
                                </div>
                            </li>`;

                        } else {

                            // Add messages to the list
                            messagesList += `<li data-message="${element.messageId}">
                                ${messageDate}
                                <div class="flex">
                                    <div class="fc-message">
                                        ${content}
                                    </div>
                                    <div class="fc-author-photo flex flex-col">
                                        <div class="flex-grow"></div>
                                        ${IconPerson()}
                                    </div>                                            
                                </div>
                            </li>`;

                        }

                    }
                    
                    // Check if the page is greater than 1
                    if ( pagination.Page > 1 ) {

                        // Replace Initial chat height
                        initChatHeight = document.getElementsByClassName('fc-chat-body')[0].scrollHeight;

                        // Update the messages list
                        setHtmlMessages(prevHtml => messagesList + prevHtml);

                    } else {

                        // Set messages
                        setHtmlMessages(messagesList);

                    }

                    // Select the chat body
                    let chatBody: Element = document.getElementsByClassName('fc-chat-body')[0];

                    // Check if the div is scrolled up to the bottom
                    if ( (((chatBody as HTMLElement).offsetHeight + chatBody.scrollTop) >= chatBody.scrollHeight) || (chatBody.scrollHeight < 200) ) {

                        // Wait until messages will be appended
                        setTimeout((): void => {

                            // Select the chat body
                            let chatBody: Element = document.getElementsByClassName('fc-chat-body')[0];

                            // Scroll the messages to the bottom
                            chatBody.scrollTop = chatBody.scrollHeight;

                        }, 1);

                    } else {

                        // Wait until messages will be appended
                        setTimeout((): void => {

                            // Select the chat body
                            let chatBody: Element = document.getElementsByClassName('fc-chat-body')[0];

                            // Scroll the messages to the bottom
                            chatBody.scrollTop = chatBody.scrollHeight - initChatHeight - 44;

                        }, 1);                        

                        // Show the unseen notification
                        //document.getElementsByClassName('fc-chat-unseen-messages')[0].classList.add('fc-chat-unseen-messages-show');

                    }

                    // Verify if there are more pages
                    if ( (response.data.result.page * 10) < response.data.result.total ) {

                        // Show the navigation button
                        document.getElementsByClassName('fc-chat-pagination')[0].classList.add('fc-chat-pagination-show');

                    } else {

                        // Add the navigation button
                        document.getElementsByClassName('fc-chat-pagination')[0].classList.remove('fc-chat-pagination-show');

                    }

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

            // Display in the console the error
            console.log(error);

        }

    };

    // Run some code after content load
    useEffect((): () => void => {

        // Load the website
        websiteInfo();

        // Check if thread secret is saved
        if ( SecureStorage.getItem('fc_website_thread_secret_' + params.slug) ) {

            // Open the chat
            setStartChat(0);

        }

        // Register an event for clicks tracking
        document.addEventListener('click', trackClicks);

        return (): void => {

            // Verify if websocket.current has value
            if ( websocket.current instanceof WebSocket ) {

                // Web Socket connection
                websocket.current.close();

                // Cancel Websocket instance
                websocket.current = undefined;

            }

            // Remove event for clicks tracking
            document.removeEventListener('click', trackClicks);

        }
        
    }, []);

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
                    throw new Error(getWord('errors', 'error_csrf_token_not_generated'));

                }

                // Set the headers
                let headers: typePostHeader = {
                    headers: {
                        CsrfToken: csrfToken.token
                    }
                };   
                
                // Prepare the thread secret
                let threadSecret: string = SecureStorage.getItem('fc_website_thread_secret_' + params.slug)?SecureStorage.getItem('fc_website_thread_secret_' + params.slug)!.toString():''; 

                // Request the messages
                await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/threads/' + params.slug + '/' + threadSecret + '/typing', null, headers)

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
            getMessages();

        }

    };

    /**
     * Open the chat handler
     * 
     * @param Event e 
     */
    let openChat = (e: React.MouseEvent<HTMLButtonElement>): void => {
        e.preventDefault();

        // Sent message to the parent
        window.parent.postMessage('show', '*');

        // Show the chat
        showChat('show');

        // Verify if messages are already loaded
        if ( !htmlMessages ) {

            // Check if thread secret is saved
            if ( SecureStorage.getItem('fc_website_thread_secret_' + params.slug) ) {

                // Reset the pagination
                pagination.Page = 1;

                // Reset Messages ids
                messagesIds.current = [];

                // Reset temporary dates
                tempDates.current = {};

                // Gets the chat messages
                getMessages();

            }

        } else {

            // Wait until messages will be appended
            setTimeout((): void => {

                // Select the chat body
                let chatBody: Element = document.getElementsByClassName('fc-chat-body')[0];

                // Scroll the messages to the bottom
                chatBody.scrollTop = chatBody.scrollHeight;

            }, 100);

        }

    };

    /**
     * Hide the chat handler
     * 
     * @param Event e 
     */
    let hideChat: MouseEventHandler<HTMLAnchorElement> = (e): void => {
        e.preventDefault();

        // Sent message to the parent
        window.parent.postMessage('hide', '*');

        // Set a pause
        setTimeout((): void => {

            // Hide the chat
            showChat('');

        }, 300);

    };

    /**
     * Handle the init chat form submit
     * 
     * @param Event e 
     */
    let initChatSubmit: React.FormEventHandler = async (e): Promise<void> => {
        e.preventDefault();
        
        // Reset the errors messages
        setErrors({});

        try {

            // Generate a new csrf token
            let csrfToken: typeToken = await getToken();

            // Check if csrf token is missing
            if ( !csrfToken.success ) {

                // Show error notification
                throw new Error(getWord('errors', 'error_csrf_token_not_generated'));

            }

            // Set the headers
            let headers: typePostHeader = {
                headers: {
                    CsrfToken: csrfToken.token
                }
            };  

            // Request the fields value
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/threads', {
                WebsiteId: params.slug,
                Name: initChat.name,
                Email: initChat.email,
                Message: initChat.message
            }, headers)

            // Process the response
            .then((response: AxiosResponse) => {

                // Verify if the response is successfully
                if ( response.data.success ) {

                    // Check if thread secret is saved
                    if ( !SecureStorage.getItem('fc_website_thread_secret_' + params.slug) ) {

                        // Save the thread secret
                        SecureStorage.setItem('fc_website_thread_secret_' + params.slug, response.data.thread.threadSecret);

                    }

                    // Empty message
                    setMessage('');

                    // Reload the chat messages
                    getMessages();

                    // Open the chat
                    setStartChat(0);
                    
                } else if ( typeof response.data.Message !== 'undefined' ) {

                    // Set error notification
                    throw new Error(response.data.Message[0]);                  

                } else {

                    // Keys container
                    let keys: string[] = Object.keys(response.data);

                    // Count the keys
                    let keysTotal: number = keys.length;

                    // Check if keys exists
                    if ( keysTotal > 0 ) {

                        // Errors container
                        let errorsHolder: {[key: string]: string} = {};

                        // List the keys
                        for ( let e = 0; e < keysTotal; e++ ) {

                            // Save error in the container
                            errorsHolder[keys[e]] = response.data[keys[e]][0];

                        }

                        // Update the errors
                        setErrors(errorsHolder);

                    }

                    // Run error notification
                    throw new Error(getWord('public', 'public_chat_not_started'));      
                    
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

    /**
     * Images uploader
     * 
     * @param Event e
     */
    let uploadImages = async (e: React.ChangeEvent): Promise<void> => {

        // Select progress bar
        let progressBar = e.currentTarget.closest('.fc-chat-footer')!.getElementsByClassName('fc-progress-bar')[0] as HTMLElement;
        
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

            // Prepare the thread secret
            let threadSecret: string = SecureStorage.getItem('fc_website_thread_secret_' + params.slug)?SecureStorage.getItem('fc_website_thread_secret_' + params.slug)!.toString():'';       

            // Generate a new csrf token
            let csrfToken: typeToken = await getToken();

            // Check if csrf token is missing
            if ( !csrfToken.success ) {

                // Show error notification
                throw new Error(getWord('errors', 'error_csrf_token_not_generated'));

            }

            // Upload the images on the server
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/messages/attachments/' + params.slug + '/' + threadSecret, form, {
                headers: {
                    'Content-Type': 'multipart/form-data',
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
                    getMessages();

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

    /**
     * Handle the new message form submit
     * 
     * @param Event e 
     */
    let newMessageSubmit: React.FormEventHandler = async (e): Promise<void> => {
        e.preventDefault();

        try {

            // Generate a new csrf token
            let csrfToken: typeToken = await getToken();

            // Check if csrf token is missing
            if ( !csrfToken.success ) {

                // Show error notification
                throw new Error(getWord('errors', 'error_csrf_token_not_generated'));

            }

            // Set the headers
            let headers: typePostHeader = {
                headers: {
                    CsrfToken: csrfToken.token
                }
            };  

            // Request the fields value
            await axios.post(process.env.NEXT_PUBLIC_API_URL + 'api/v1/messages', {
                WebsiteId: params.slug,
                ThreadSecret: SecureStorage.getItem('fc_website_thread_secret_' + params.slug),
                Message: message
            }, headers)

            // Process the response
            .then((response: AxiosResponse) => {

                // Verify if the response is successfully
                if ( response.data.success ) {

                    // Empty message
                    setMessage('');

                    // Reset the pagination
                    pagination.Page = 1;

                    // Reset Messages ids
                    messagesIds.current = [];

                    // Reset temporary dates
                    tempDates.current = {};

                    // Reload the chat messages
                    getMessages();
                    
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
            {(chatDisabled < 1)?(
                <div className="fc-chat-container">
                    {(chat === '')?(
                        <button className="fc-start-button" onClick={openChat}>
                            { getIcon('IconChatBubble') }
                        </button>                
                    ):(
                        <div className="fc-chat">
                            <div className="fc-chat-header flex items-center justify-between">
                                <h3>
                                    {chatHeader}
                                    <div className="fc-message-typing"></div>
                                </h3>
                                <Link href="#" onClick={hideChat}>
                                    { getIcon('IconClose') }
                                </Link>
                            </div>
                            <div className="fc-chat-body">
                                <Link href="#" className="fc-chat-pagination" data-page={(pagination.Page + 1)}>
                                    { getIcon('IconAutorenew', {className: 'fc-load-more-icon'}) }
                                    { getWord('user', 'user_load_older_messages')  }
                                </Link>
                                <ul className="fc-chat-messages" dangerouslySetInnerHTML={{__html: htmlMessages}} />
                            </div>
                            <div className="fc-chat-footer">
                                {(startChat < 1)?(
                                    <>
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
                                        <form className="flex" id="fc-chat-new-message-form" onSubmit={newMessageSubmit}>
                                            <textarea placeholder={getWord('public', 'public_type_message')} value={message} id="fc-chat-message" onInput={handleChange}></textarea>
                                            <button type="button" className="fc-attach-message-button" onClick={(e: React.MouseEvent<Element>): void => {
                                                ((e.target as Element).closest('.fc-chat-footer')!.getElementsByClassName('fc-chat-message-file')[0] as HTMLButtonElement).click();
                                            }}>
                                                { getIcon('IconAttachment') }
                                            </button> 
                                            <button type="submit" className="fc-send-message-button">
                                                { getIcon('IconSend') }
                                            </button> 
                                        </form>
                                        <form id="fc-chat-upload-form">
                                            <input type="file" accept="image/jpeg,image/gif,image/png" name="fc-chat-message-file" multiple className="fc-chat-message-file" id="fc-chat-message-file" onChange={uploadImages} />
                                        </form>
                                    </>
                                ):(
                                    <form className="fc-chat-start-form" id="fc-chat-start-form" onSubmit={initChatSubmit}>
                                        <div className="col-span-full mb-5">
                                            <p>{getWord('public', 'public_thanks_for_contacting_us')}</p>
                                        </div>
                                        <div className="col-span-full fc-start-chat-input">
                                            <input
                                                type="text"
                                                placeholder={getWord('public', 'public_enter_guest_name')}
                                                value={initChat.name}
                                                name="fc-start-chat-input-guest-name"
                                                id="fc-start-chat-input-guest-name"
                                                className="block px-2.5 pb-2.5 pt-4 w-full fc-init-chat-form-input"
                                                onChange={(e: React.ChangeEvent<HTMLInputElement>): void => setInitChat({...initChat, name: e.target.value})}
                                                required
                                            />
                                            <label
                                                htmlFor="fc-start-chat-input-guest-name"
                                                className="absolute"
                                            >
                                                { getIcon('IconDriveFileRenameOutline') }
                                            </label>
                                            <div className={(typeof (errors as {[key: string]: string}).Name !== 'undefined')?'fc-init-chat-form-input-error-message fc-init-chat-form-input-error-message-show':'fc-init-chat-form-input-error-message'}>
                                                {(typeof (errors as {[key: string]: string}).Name !== 'undefined')?(errors as {[key: string]: string}).eName:''}
                                            </div>
                                        </div>
                                        <div className="col-span-full fc-start-chat-input">
                                            <input
                                                type="email"
                                                placeholder={getWord('public', 'public_enter_your_email')}
                                                value={initChat.email}
                                                name="fc-start-chat-input-email-address"
                                                id="fc-start-chat-input-email-address"
                                                className="block px-2.5 pb-2.5 pt-4 w-full fc-init-chat-form-input"
                                                onChange={(e: React.ChangeEvent<HTMLInputElement>): void => setInitChat({...initChat, email: e.target.value})}
                                                required
                                            />
                                            <label
                                                htmlFor="fc-start-chat-input-email-address"
                                                className="absolute"
                                            >
                                                { getIcon('IconAt') }
                                            </label>
                                            <div className={(typeof (errors as {[key: string]: string}).Email !== 'undefined')?'fc-init-chat-form-input-error-message fc-init-chat-form-input-error-message-show':'fc-init-chat-form-input-error-message'}>
                                                {(typeof (errors as {[key: string]: string}).Email !== 'undefined')?(errors as {[key: string]: string}).Email:''}
                                            </div>
                                        </div> 
                                        <div className="col-span-full fc-start-textarea-input">
                                            <textarea
                                                placeholder={getWord('public', 'public_type_message')}
                                                value={initChat.message}
                                                name="fc-start-chat-input-message-url"
                                                id="fc-start-chat-input-message-url"
                                                className="block px-2.5 pb-2.5 pt-4 w-full fc-init-chat-form-textarea"
                                                onChange={(e: React.ChangeEvent<HTMLTextAreaElement>): void => setInitChat({...initChat, message: e.target.value})}
                                                required
                                            ></textarea>
                                            <div className={(typeof (errors as {[key: string]: string}).Url !== 'undefined')?'fc-init-chat-form-input-error-message fc-init-chat-form-input-error-message-show':'fc-init-chat-form-input-error-message'}>
                                                {(typeof (errors as {[key: string]: string}).Url !== 'undefined')?(errors as {[key: string]: string}).Url:''}
                                            </div>
                                        </div> 

                                        <div className="col-span-full fc-modal-button">
                                            <div className="text-right">
                                                <button type="submit" className="mb-3 fc-option-violet-btn fc-submit-button">
                                                    { getWord('public', 'public_start_chat') }
                                                    { getIcon('IconAutorenew', {className: 'fc-load-more-icon ml-3'}) }
                                                    { getIcon('IconArrowForward', {className: 'fc-next-icon ml-3'}) }
                                                </button>
                                            </div>
                                        </div> 
                                    </form>                              
                                )}
                            </div>
                        </div>
                    )}
                </div>
            ):(<></>)}
        </>

    );

};

// Export the page content
export default Page;