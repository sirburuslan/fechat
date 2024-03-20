/*
 * @page Gateways
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-06
 *
 * This file contains the gateways plans in the user panel
 */

'use client'

// Import the Gateways List component
import GatewaysList from "@/core/components/user/gateways/GatewaysList";

// Create the page content
const Page = ({params}: {params: {slug: string}}): React.JSX.Element => {

    return (<GatewaysList plan={params.slug} />);

}

// Export the page
export default Page;