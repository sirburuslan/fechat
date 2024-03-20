/**
 * IconContentCopy
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconContentCopy = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>content_copy</span>
    );

}

// Export the function
export default IconContentCopy;