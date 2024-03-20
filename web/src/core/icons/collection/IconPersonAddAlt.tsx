/**
 * IconPersonAddAlt
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconPersonAddAlt = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>person_add_alt</span>
    );

}

// Export the function
export default IconPersonAddAlt;