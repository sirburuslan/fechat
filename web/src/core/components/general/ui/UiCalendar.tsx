/*
 * @component Ui Calendar
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This component shows the ui calendar in the app
 */

'use client'

// Import the React's hooks
import { useRef, useEffect, useState, useContext } from 'react';

// Import the incs
import { getMonth, getWord, getIcon } from '@/core/inc/incIndex';

// Import the options for website and member
import { MemberOptionsContext } from '@/core/contexts/OptionsContext';

// The date type
type Date = {
    date: string;
    month: string;
    year: string;
};

// Create the Calendar
const UiCalendar: React.FC<Date> = ({date, month, year}): React.JSX.Element => {

    // Member options
    const {memberOptions, setMemberOptions} = useContext(MemberOptionsContext);   

    // Register default value for calendar
    const [calendar, updateCalendar] = useState('');

    // Register default value for year
    const [iYear, setIYear] = useState(parseInt(year));

    // Register default value for month
    const [iMonth, setIMonth] = useState(parseInt(month)); 

    // Register default value for selected date
    const [selectedDate, updateSelectedDate] = useState(''); 

    // Create a reference for the calendar
    const calendarRef = useRef<HTMLDivElement>(null);

    /**
     * Generate calendar
     * 
     * @returns string with calendar
     */
    const get_calendar = (): string => {

        // Get year
        const year: number = iYear;

        // Get month
        const month: number = iMonth;

        // Set current date
        const current = new Date();

        // Get day
        const day: string = current.getDate().toString().padStart(2, '0');

        // Set day
        const d = new Date(year, month, 0);

        // Get time
        const e = new Date(d.getFullYear(), d.getMonth(), 1);
        
        // Current day
        let fday = e.getDay();
        
        // Show option
        let show: number = 1;

        // Increase day
        fday++;

        // Set days of the week
        let calendar: string = '<tr>'
                + '<td>'
                    + getWord('default', 'default_day_m', memberOptions['Language'])
                + '</td>'
                + '<td>'
                    + getWord('default', 'default_day_tu', memberOptions['Language'])
                + '</td>'
                + '<td>'
                    + getWord('default', 'default_day_w', memberOptions['Language'])
                + '</td>'
                + '<td>'
                    + getWord('default', 'default_day_th', memberOptions['Language'])
                + '</td>'
                + '<td>'
                    + getWord('default', 'default_day_f', memberOptions['Language'])
                + '</td>'
                + '<td>'
                    + getWord('default', 'default_day_sa', memberOptions['Language'])
                + '</td>'
                + '<td>'
                    + getWord('default', 'default_day_su', memberOptions['Language'])
                + '</td>'
            + '</tr>'
        + '<tr>';

        // Calculate days
        for ( let s = 2; s < 40; s++ ) {

            // Verify if show is greater than current date
            if ( show > d.getDate() ) {
                break;
            }
            
            // Verify if should be added new row
            if ( (s - 2) % 7 === 0 ) {
                calendar += '</tr>'
                + '<tr>';
            }
            
            // Verify if the date is in the current month
            if ( fday <= s ) {

                // Prepare the month
                const this_month: string = month.toString().padStart(2, '0');

                // Prepare the date
                const this_date: string = show.toString().padStart(2, '0');

                // Selected date container
                let selected_date: string = '';
                
                // Verify if the date is selected
                if ( year + '-' + this_month + '-' + this_date === selectedDate ) {

                    // Add the date is selected
                    selected_date = ' fc-calendar-selected-date';

                }

                // Verify if is the current day
                if ( ( this_date === day.toString().padStart(2, '0') ) && ( month === current.getMonth() + 1 ) && ( year === current.getFullYear() ) ) {

                    // Add date with current day class
                    calendar += '<td>'
                        + '<a href="#" class="fc-calendar-current-day' + selected_date + '" data-date="' + year + '-' + this_month + '-' + this_date + '">'
                            + this_date
                        + '</a>'
                    + '</td>';

                } else {

                    // Selected date class
                    const selected_date_class: string = selected_date?' class="' + selected_date + '"':'';

                    // Add a date
                    calendar += '<td>'
                        + '<a href="#" data-date="' + year + '-' + this_month + '-' + this_date + '"' + selected_date_class + '>'
                            + this_date
                        + '</a>'
                    + '</td>';

                }

                // Increase the date to show
                show++;

            } else {

                // Add empty column
                calendar += '<td>'
                + '</td>';

            }

        }

        // End calendar
        calendar += '</tr>';
        
        return calendar;
        
    };

    // Monitor changes
    useEffect((): () => void => {

        // Register one listener for all links
        document.addEventListener('click', trackClicks);

        return (): void => {

            // Remove listener
            document.removeEventListener('click', trackClicks);

        }

    }, []);

    // Monitor changes
    useEffect(() => {

        // Update the calendar
        updateCalendar(get_calendar());

        // Wait a moment
        setTimeout((): void => {

            // Check if calendar exists
            if ( calendarRef.current ) {

                // List all links
                Array.from(calendarRef.current!.getElementsByTagName('a')).forEach((element) => {
                    element.classList.remove('fc-calendar-selected-date');
                });

                // Verify if date exists
                if ( calendarRef.current!.querySelectorAll('a[data-date="' + year + '-' + month.toString().padStart(2, '0') + '-' + date.toString().padStart(2, '0') + '"]').length > 0 ) {

                    // Add fc-calendar-selected-date class
                    calendarRef.current?.querySelector('a[data-date="' + year + '-' + month.toString().padStart(2, '0') + '-' + date.toString().padStart(2, '0') + '"]')!.classList.add('fc-calendar-selected-date');

                }

            }

        }, 300);

    }, [date, month, year]);

    /**
     * Detect mouse click on calendar link
     * 
     * @param MouseEvent e 
     */
    const trackClicks = (e: MouseEvent): void => {

        // Save the target
        const target = e.target as HTMLElement;

        // Verify if the click is inside the calendar
        if ( target.closest('.fc-calendar') && (target.nodeName === 'A') ) {
            e.preventDefault();

            // Get the calendar box
            const calendarBox = calendarRef.current;
            
            // Verify if calendarBox exists
            if ( calendarBox ) {

                // List all links
                Array.from(calendarBox.getElementsByTagName('a')).forEach((element) => {
                    element.classList.remove('fc-calendar-selected-date');
                });

                // Add the class fc-calendar-selected-date
                (e.target as HTMLElement).classList.add('fc-calendar-selected-date');

            }

        }

    };

    // Get the previous month in the calendar
    const previousMonth = (): void => {

        // Get the calendar box
        const calendarBox = calendarRef.current;
        
        // Verify if calendarBox exists
        if ( calendarBox ) {

            // Verify if a selected class exists
            if ( calendarBox.getElementsByClassName('fc-calendar-selected-date').length > 0 ) {

                // Add selected date
                updateSelectedDate(calendarBox.getElementsByClassName('fc-calendar-selected-date')[0].getAttribute('data-date')!); 

            }

        }

        // Verify if month is 1
        if ( iMonth < 2 ) {
            
            // Update Year
            setIYear(iYear - 1);
            
            // Update month
            setIMonth(12);
            
        } else {

            // Update month
            setIMonth((iMonth - 1));

        }
        
    };

    // Get the next month in the calendar
    const nextMonth = (): void => {

        // Get the calendar box
        const calendarBox = calendarRef.current;
        
        // Verify if calendarBox exists
        if ( calendarBox ) {

            // Verify if a selected class exists
            if ( calendarBox.getElementsByClassName('fc-calendar-selected-date').length > 0 ) {

                // Add selected date
                updateSelectedDate(calendarBox.getElementsByClassName('fc-calendar-selected-date')[0].getAttribute('data-date')!); 

            }

        }

        // If next month is hight than 11
        if ( iMonth > 11 ) {

            // Update Year
            setIYear(iYear + 1);
            
            // Update month
            setIMonth(1);
            
        } else {

            // Update month
            setIMonth((iMonth + 1));
            

        }
        
    };

    return (
        <div className="fc-calendar" ref={calendarRef}>
            <table>
                <thead>
                    <tr>
                        <th>
                            <button type="button" className="fc-calendar-date-previous-btn" onClick={previousMonth}>
                                { getIcon('IconChevronLeft') }
                            </button>
                        </th>                                                                                                           
                        <th className="fc-calendar-year-month">
                            { getMonth(iMonth.toString().padStart(2, '0')) } { iYear }
                        </th>
                        <th>
                            <button type="button" className="fc-calendar-date-next-btn" onClick={nextMonth}>
                                { getIcon('IconChevronRight') }
                            </button>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td colSpan={3}>
                            <table>
                                <tbody className="fc-calendar-dates" dangerouslySetInnerHTML={{ __html: calendar }}></tbody>
                            </table>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    );

}

// Export the calendar
export default UiCalendar;