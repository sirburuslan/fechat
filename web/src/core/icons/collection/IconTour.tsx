/**
 * IconTour
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconTour = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>tour</span>
    );

}

// Export the function
export default IconTour;