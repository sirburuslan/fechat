/**
 * IconMoreVertical
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconMoreVertical = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>more_vert</span>
    );

}

// Export the function
export default IconMoreVertical;