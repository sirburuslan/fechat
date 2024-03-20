/**
 * IconDuo
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconDuo = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>duo</span>
    );

}

// Export the function
export default IconDuo;