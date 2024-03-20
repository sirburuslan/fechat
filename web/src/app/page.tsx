/*
 * @page Home Page
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains the home page
 */

// Import the Top Bar component
import TopBar from "@/core/components/public/TopBar";

// Import the Presentation component
import Presentation from "@/core/components/public/Presentation";

// Import the the Stats component
import Stats from "@/core/components/public/Stats";

// Import the Features component
import Features from "@/core/components/public/Features";

// Import the Plans component
import Plans from "@/core/components/public/Plans";

// Import the Faq component
import Faq from "@/core/components/public/Faq";

// Import the Footer component
import Footer from "@/core/components/public/Footer";

// Import the Cookies component
import Cookies from "@/core/components/public/Cookies";

// Import styles
import '@/styles/public/_main.scss';

// Create the Home Page
const HomePage = (): React.JSX.Element => {

    return (<main className='fc-main'>
        <TopBar />
        <Presentation />
        <Stats />
        <Features />
        <Plans />
        <Faq />
        <Footer />
        <Cookies />
    </main>);

};

// Export the Home Page
export default HomePage;