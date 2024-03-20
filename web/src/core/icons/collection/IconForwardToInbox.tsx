/**
 * IconForwardToInbox
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconForwardToInbox = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>forward_to_inbox</span>
    );

}

// Export the function
export default IconForwardToInbox;