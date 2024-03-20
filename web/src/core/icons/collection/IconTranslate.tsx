/**
 * IconTranslate
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconTranslate = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>translate</span>
    );

}

// Export the function
export default IconTranslate;