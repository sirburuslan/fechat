/**
 * IconSupervisorAccount
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconSupervisorAccount = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>supervisor_account</span>
    );

}

// Export the function
export default IconSupervisorAccount;