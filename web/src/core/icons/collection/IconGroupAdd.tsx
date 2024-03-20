/**
 * IconGroupAdd
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconGroupAdd = (params: {[key: string]: string | number}): React.JSX.Element => {

    // Create the icon class
    let icon_class: string = params?.className?params.className as string + ' material-icons-outlined':'material-icons-outlined';

    return (
        <span className={icon_class}>group_add</span>
    );

}

// Export the function
export default IconGroupAdd;