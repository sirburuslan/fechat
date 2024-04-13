/*
 * @component Chart Line Widget
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains the Chart Line Widget used in the administrator panel
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
const ChartLineWidget: React.FC<{members: Array<{[key: string]: string | number}>, datasets: string[]}> = ({members, datasets}): React.JSX.Element => {

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
        if ( document.getElementById('fc-member-members-chart-menu')!.getAttribute('data-expand') === 'false' ) {

            // Change the dropdown status
            document.getElementById('fc-member-members-chart-menu')!.setAttribute('data-expand', 'true');

        } else {

            // Change the dropdown status
            document.getElementById('fc-member-members-chart-menu')!.setAttribute('data-expand', 'false');

        }

    }

    return (
        <div className="fc-dashboard-widget">
            <div className="fc-dashboard-widget-head flex justify-between">
                <h3>
                    { getIcon('IconSupervisorAccount', {className: 'fc-dashboard-widget-icon'}) }
                    <span className="fc-dashboard-widget-text">
                        { getWord('admin', 'admin_members', memberOptions['Language'])  }
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
                    <UiDropdown button="" options={ dropdown_items } id="fc-member-members-chart-menu" />                    
                </div>
            </div>
            <div className="fc-dashboard-widget-body">
                {(members.length > 0)?(
                    <ChartLine data={members} datasets={datasets} />
                ):(<p className="fc-no-results-found">{ getWord('admin', 'admin_no_members_were_joined', memberOptions['Language'])  }</p>)}
            </div>                    
        </div>
    );

}

// Export the Chart Line Widget
export default ChartLineWidget;