// Import HeroIcons module
import { InformationCircleIcon } from '@heroicons/react/24/outline';

/**
 * IconInformationCircle
 * 
 * @param object params?
 * 
 * @returns React.JSX.Element
 */
const IconInformation = (params: {[key: string]: string | number}): React.JSX.Element => {

    return (
        params?.className?<InformationCircleIcon className={params.className as string} />:<InformationCircleIcon />
    );

}

// Export the function
export default IconInformation;