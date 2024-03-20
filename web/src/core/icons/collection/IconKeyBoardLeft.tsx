/**
 * IconKeyBoardLeft
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconKeyBoardLeft = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>keyboard_arrow_left</span>
    );

}

// Export the function
export default IconKeyBoardLeft;