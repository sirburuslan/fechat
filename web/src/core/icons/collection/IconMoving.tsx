/**
 * IconMoving
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconMoving = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>moving</span>
    );

}

// Export the function
export default IconMoving;