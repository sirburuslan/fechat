/**
 * IconAdd
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconAdd = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>add</span>
    );

}

// Export the function
export default IconAdd;