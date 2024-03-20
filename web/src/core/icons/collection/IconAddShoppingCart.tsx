/**
 * IconAddShoppingCart
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconAddShoppingCart = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        <span className={`material-icons-outlined${params?.className?' ' + params?.className:''}`}>add_shopping_cart</span>
    );

}

// Export the function
export default IconAddShoppingCart;