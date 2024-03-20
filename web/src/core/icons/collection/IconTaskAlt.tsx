/**
 * IconTaskAlt
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconTaskAlt = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>task_alt</span>
    );

}

// Export the function
export default IconTaskAlt;