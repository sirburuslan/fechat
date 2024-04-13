/*
 * @component Chart Line Widget
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This file shows the chart line widget in the user's panel
 */

'use client'

// Import the react hooks
import { useContext } from 'react';

// Import the incs
import { getIcon, getWord } from '@/core/inc/incIndex';

// Import the options for website and member
import {MemberOptionsContext} from '@/core/contexts/OptionsContext';

// Import the UI Components
import { UiDropdown } from '@/core/components/general/ui/UiIndex';

// Import the Charts
import { ChartLine } from '@/core/components/general/charts/ChartsIndex';

// Create the Chart Line Widget
const ChartLineWidget: React.FC<{threads: Array<{[key: string]: string | number}>, datasets: string[]}> = ({threads, datasets}): React.JSX.Element => {

    // Member options
    const {memberOptions} = useContext(MemberOptionsContext);

    // Dropdown items
    const dropdown_items: Array<{text: string, url: string, id: string}> = [{
        text: getWord('admin', 'admin_30_days', memberOptions['Language']),
        url: '#',
        id: '1'
    }, {
        text: getWord('admin', 'admin_90_days', memberOptions['Language']),
        url: '#',
        id: '2'
    }, {
        text: getWord('admin', 'admin_180_days', memberOptions['Language']),
        url: '#',
        id: '3'
    }, {
        text: getWord('admin', 'admin_360_days', memberOptions['Language']),
        url: '#',
        id: '4'
    }];

    // Create the function which will open or close dropdown
    const openTimeDropdown = (e: React.MouseEvent<HTMLElement>): void => {
        e.preventDefault();

        // Verify if the dropdown is showed
        if ( document.getElementById('fc-threads-chart-menu')!.getAttribute('data-expand') === 'false' ) {

            // Change the dropdown status
            document.getElementById('fc-threads-chart-menu')!.setAttribute('data-expand', 'true');

        } else {

            // Change the dropdown status
            document.getElementById('fc-threads-chart-menu')!.setAttribute('data-expand', 'false');

        }

    }

    return (
        <div className="fc-dashboard-widget">
            <div className="fc-dashboard-widget-head flex justify-between">
                <h3>
                    { getIcon('IconThreads', {className: 'fc-dashboard-widget-icon'}) }
                    <span className="fc-dashboard-widget-text">
                        { getWord('user', 'user_threads', memberOptions['Language']) }
                    </span>
                </h3>
                <div>
                    <button type="button" className="fc-button" onClick={openTimeDropdown}>
                        { getIcon('IconWatchLater', {className: 'fc-dashboard-widget-calendar-icon'}) }
                        <span className="fc-dashboard-widget-dropdown-text">
                            { getWord('admin', 'admin_30_days', memberOptions['Language'])  }
                        </span>
                        { getIcon('IconExpandMore', {className: 'fc-dashboard-widget-arrow-down-icon'}) }
                    </button>
                    <UiDropdown button="" options={ dropdown_items } id="fc-threads-chart-menu" />                    
                </div>
            </div>
            <div className="fc-dashboard-widget-body">
                {(threads.length > 0)?(
                    <ChartLine data={threads} datasets={datasets} />
                ):(<p className="fc-no-results-found">{ getWord('user', 'user_no_threads_found', memberOptions['Language'])  }</p>)}
            </div>                    
        </div>
    );

}

// Export the Chart Line Widget
export default ChartLineWidget;