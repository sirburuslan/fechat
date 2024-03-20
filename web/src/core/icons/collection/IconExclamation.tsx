// Import HeroIcons module
import { ExclamationCircleIcon } from '@heroicons/react/24/outline';

/**
 * IconExclamationCircle
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconExclamation = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        params?.className?<ExclamationCircleIcon className={params.className as string} />:<ExclamationCircleIcon />
    );

}

// Export the function
export default IconExclamation;