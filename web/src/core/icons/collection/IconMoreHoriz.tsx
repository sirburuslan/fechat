/**
 * IconMoreHoriz
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconMoreHoriz = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>more_horiz</span>
    );

}

// Export the function
export default IconMoreHoriz;