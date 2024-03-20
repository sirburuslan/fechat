/**
 * IconSettings
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconSettings = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>manage_search</span>
    );

}

// Export the function
export default IconSettings;