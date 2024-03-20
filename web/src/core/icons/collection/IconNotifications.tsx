/**
 * IconNotifications
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconNotifications = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>notifications</span>
    );

}

// Export the function
export default IconNotifications;