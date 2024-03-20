/**
 * IconTouchApp
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconTouchApp = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>touch_app</span>
    );

}

// Export the function
export default IconTouchApp;