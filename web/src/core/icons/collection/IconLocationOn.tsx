/**
 * IconLocationOn
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconLocationOn = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>location_on</span>
    );

}

// Export the function
export default IconLocationOn;