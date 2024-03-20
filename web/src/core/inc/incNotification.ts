/*
 * @inc Notifications
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains a function to show notifications in the app
 */

/**
 * Show notification
 * 
 * @param string icon_name
 * @param object params?
 */
const showNotification = (type: string, message: string): void => {

    // Status status class
    let bgColor = (type === 'success')?'fc-popup-notification-success':'fc-popup-notification-error';

    // Create the popup
    let popup: string = '<div class="fc-popup-notification ' + bgColor + '">'
        + message
    + '</div>'
    
    // Insert the popup
    document.getElementsByTagName('body')[0].insertAdjacentHTML('beforeend', popup);

    // Set pause for notification hidding
    setTimeout(function () {

        // Set opacity
        (document.getElementsByClassName('fc-popup-notification')[0] as HTMLElement).style.opacity = '0';

    }, 2500);

    // Set pause for notification removing
    setTimeout(function () {

        // Remove notification
        document.getElementsByClassName('fc-popup-notification')[0].remove();

    }, 3000);

}

// Export the show notification function
export default showNotification;