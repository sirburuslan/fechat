// Import React module
import React from 'react';

// Import styles
import '@/styles/errors/_main.scss';

// Create layout for the administrator panel
const Layout = async ({children}: {children: React.ReactNode}): Promise<React.JSX.Element> => {

    return <main className="flex fc-errors-main">
        <div className="w-full fc-errors-content">
            {children}
        </div>
    </main>
};

// Export the layout
export default Layout;