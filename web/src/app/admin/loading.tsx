/*
 * @file Loading
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains the animation for the administrator panel
 */

// Create the Loading component
const Loading = (): React.JSX.Element => {

    return (
        <div className="fc-loading-animation">
            <div className="fc-loading-content">
                <div></div>
                <div></div>
            </div>
        </div>
    );

};

// Export the Loading component
export default Loading;