/**
 * IconSupportAgent
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconSupportAgent = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>support_agent</span>
    );

}

// Export the function
export default IconSupportAgent;