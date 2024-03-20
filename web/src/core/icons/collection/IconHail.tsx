/**
 * IconHail
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconHail = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>hail</span>
    );

}

// Export the function
export default IconHail;