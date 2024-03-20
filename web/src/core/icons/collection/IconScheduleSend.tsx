/**
 * IconScheduleSend
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconScheduleSend = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>schedule_send</span>
    );

}

// Export the function
export default IconScheduleSend;