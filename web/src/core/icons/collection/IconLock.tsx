// Import HeroIcons module
import { LockClosedIcon } from '@heroicons/react/24/outline';

/**
 * IconLock
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconLock = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        params?.className?<LockClosedIcon className={params.className as string} />:<LockClosedIcon />
    );

}

// Export the function
export default IconLock;