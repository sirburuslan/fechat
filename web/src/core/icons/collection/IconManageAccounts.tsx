/**
 * IconManageAccounts
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconManageAccounts = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>manage_accounts</span>
    );

}

// Export the function
export default IconManageAccounts;