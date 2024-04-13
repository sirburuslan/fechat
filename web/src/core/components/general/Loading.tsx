/*
 * @component Loading
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This component shows the loading page in the app
 */

'use client'

// Import the React module
import {useEffect, useState} from 'react';

// Create the loading structure
const Loading = (): React.JSX.Element => {

    // Hook for loading
    const [loading, hideLoading] = useState(1);

    // Set hook to run the code after content load
    useEffect((): () => void => {

        // Get looading animation
        const loading: HTMLCollectionOf<Element> = document.getElementsByClassName('fc-page-loading');

        // Verify if loading exists
        if ( loading.length > 0 ) {

            // Add overflow hide to the body
            document.getElementsByTagName('body')[0].style.overflow = 'hidden';

            // Default counter
            let c = 0;

            // Timer
            const timer = setInterval(() => {

                // Verify if the limit was reached
                if ( c === 100 ) {

                    // Wait for 700 milleseconds
                    setTimeout((): void => {    
                        
                        // Verify if loading exists
                        if ( loading.length > 0 ) {

                            // Add the fc-page-loading-hide class
                            loading[0].classList.add('fc-page-loading-hide');

                        }

                        // Wait for 300 milleseconds
                        setTimeout((): void => {

                            // Verify if loading exists
                            if ( loading.length > 0 ) {

                                // Stop
                                clearInterval(timer);

                                // Remove the loader
                                hideLoading(0);

                                // Remove style from the body
                                document.getElementsByTagName('body')[0].removeAttribute('style');

                            }

                        }, 300);

                    }, 700);                        

                } else {

                    // Verify if loading still exists
                    if ( loading.length > 0 ) {

                        // Increase counter
                        c = c + 1;

                        // Display the percentage
                        loading[0].getElementsByClassName('fc-loading-text')[0]!.textContent = c + '%';

                    }

                }

            }, 10);

        }

        return () => {

            // Remove style from the body
            document.getElementsByTagName('body')[0].removeAttribute('style');

        };

    }, []);

    return (
        <>
            {(loading > 0)?(
                <div className="fc-page-loading">
                    <div className="fc-loading-container">
                        <div className="fc-loading-circle">
                            <div className="fc-loading-circle-box fc-loading-circle-box-complete">
                                <div className="fc-loading-circle-fill"></div>
                            </div>
                            <div className="fc-loading-circle-box">
                                <div className="fc-loading-circle-fill"></div>
                            </div>
                            <div className="fc-loading-text"></div>
                        </div>
                    </div>
                </div>
            ):''}        
        </>
    );

}

// Export the loading structure
export default Loading;