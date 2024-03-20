/**
 * IconCheck
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconCheck = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>check</span>
    );

}

// Export the function
export default IconCheck;