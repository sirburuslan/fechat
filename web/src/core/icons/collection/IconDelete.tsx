/**
 * IconDelete
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconDelete = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>delete</span>
    );

}

// Export the function
export default IconDelete;