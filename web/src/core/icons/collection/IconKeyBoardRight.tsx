/**
 * IconKeyBoardRight
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconKeyBoardRight = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>keyboard_arrow_right</span>
    );

}

// Export the function
export default IconKeyBoardRight;