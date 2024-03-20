/**
 * IconClose
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconClose = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>close</span>
    );

}

// Export the function
export default IconClose;