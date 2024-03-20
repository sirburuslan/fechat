/**
 * IconHistory
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconHistory = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>history</span>
    );

}

// Export the function
export default IconHistory;