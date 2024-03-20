/**
 * IconSave
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconSave = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>save</span>
    );

}

// Export the function
export default IconSave;