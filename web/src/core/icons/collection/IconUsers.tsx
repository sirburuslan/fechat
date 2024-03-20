/**
 * IconUsers
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconUsers = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>diversity_3</span>
    );

}

// Export the function
export default IconUsers;