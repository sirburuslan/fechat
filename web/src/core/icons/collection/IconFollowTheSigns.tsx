/**
 * IconFollowTheSigns
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconFollowTheSigns = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>follow_the_signs</span>
    );

}

// Export the function
export default IconFollowTheSigns;