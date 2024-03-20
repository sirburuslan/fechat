/**
 * IconTune
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconTune = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>tune</span>
    );

}

// Export the function
export default IconTune;