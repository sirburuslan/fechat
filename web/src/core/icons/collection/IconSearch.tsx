/**
 * IconSearch
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconSearch = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>search</span>
    );

}

// Export the function
export default IconSearch;