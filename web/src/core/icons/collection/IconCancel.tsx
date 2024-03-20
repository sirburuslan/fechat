/**
 * IconCancel
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconCancel = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>cancel</span>
    );

}

// Export the function
export default IconCancel;