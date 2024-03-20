/**
 * IconPayments
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconPayments = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>payments</span>
    );

}

// Export the function
export default IconPayments;