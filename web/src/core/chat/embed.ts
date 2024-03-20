/*
 * @library FeChat
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-25
 */

// Run library
((): void => {
    
    // Create an iframe
    let iframe: HTMLIFrameElement = document.createElement('iframe');

    // Set src
    iframe.src = process.env.NEXT_PUBLIC_SITE_URL + 'chat';

    // Add iframe class
    iframe.classList.add('fc-iframe-chat');

    // Set css text
    iframe.style.cssText = `position:fixed;right:15px;bottom:15px;z-index:9999;width:80px;height:70px;transition: height 0.2s ease-in;`;

    // Insert iframe in the page
    document.getElementsByTagName('body')[0].appendChild(iframe);

    // Select the chat
    let chat = document.getElementsByClassName('fc-iframe-chat')[0] as HTMLIFrameElement;      

    // Register even listener for post messages
    window.addEventListener('message', function(event) {

        // Verify if the source is current iframe
        if (event.source === chat.contentWindow) {
            
            // Verify if data is show
            if ( event.data === 'show' ) {

                // Maximize chat
                chat.style.width = '460px';
                chat.style.height = '700px';

            } else {

                // Set height 0 for animation
                chat.style.height = '0';

                // Set pause and show the chat button
                setTimeout(() => {
                    chat.style.width = '80px'
                    chat.style.height = '70px';
                }, 300);

            }

        }

    });

})();