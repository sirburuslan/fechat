/*
 * @component Chart Line
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This component draws the chart line in the app
 */

'use client'

// Import React and hooks
import {useEffect, useRef, useContext} from 'react';

// Import Chart JS module
import Chart from 'chart.js/auto';

// Import the inc
import { getMonth } from '@/core/inc/incIndex';

// Import the options for website and member
import { MemberOptionsContext } from '@/core/contexts/OptionsContext';

/**
 * Replaces the chart js tooltip
 * 
 * @param context 
 * @param string customLang
 */
const fcChartJsTooltip = (context: any, customLang?: string): void => {

    // Tooltip Element
    let tooltip_element: HTMLElement | null = document.getElementById('fc-chart-js-tooltip');

    // Create element on first render
    if ( !tooltip_element ) {

        // Create a new div for tooltip
        tooltip_element = document.createElement('div');

        // Set the div's id
        tooltip_element.id = 'fc-chart-js-tooltip';

        // Add a table
        tooltip_element.innerHTML = '<table></table>';

        // Add div's element
        document.body.appendChild(tooltip_element);

    }

    // Get the tooltip's content
    const tooltip_model = context.tooltip;

    // Set caret Position
    tooltip_element.classList.remove('above', 'below', 'no-transform');

    // Verify if the tooltip should be modified vertically
    if (tooltip_model.yAlign) {

        // Add class
        tooltip_element.classList.add(tooltip_model.yAlign);

    } else {

        // Add class
        tooltip_element.classList.add('no-transform');
        
    }

    // Get the body's lines
    const get_lines = (bodyItem: {after: [], before: [], lines: []}): [] => {

        return bodyItem.lines;
    }

    // Set Text
    if (tooltip_model.body) {

        // Get the title's lines
        const title_lines: string[] = tooltip_model.title || [];

        // Get the body's lines
        const body_lines: string[] = tooltip_model.body.map(get_lines);

        // Lets create the tooltip html content
        let tooltip_html: string = '<thead>';

        // List the title lines
        title_lines.forEach((title: string): void => {

            // Split the title
            const values: string[] = title.split('/');

            // Prepare the date and month
            const date: string = values[2] + ' ' + getMonth(values[1].padStart(2, '0'), customLang);

            // Add the date and month to the tooltip html
            tooltip_html += '<tr><th>' + date + '</th></tr>';

        });

        // End the title and start the body
        tooltip_html += '</thead><tbody>';

        // List the body lines
        body_lines.forEach((body: string, i: number): void => {

            // Set span
            let span: string = '<span></span>';

            // Verify if tooltip text exists
            if ( typeof context.chart.config._config.options.tooltip_text !== 'undefined' ) {

                // Set the tooltip text
                span += context.chart.config._config.options.tooltip_text + ': '

            }

            // Set body and span to the tooltip
            tooltip_html += '<tr><td>' + span + body + '</td></tr>';


        });

        // End html
        tooltip_html += '</tbody>';

        // Get the tooltip parent
        const tooltip_parent: HTMLElement | null = tooltip_element.querySelector('table');

        // Add tooltip content to the parent
        tooltip_parent!.innerHTML = tooltip_html;
    }

    // Get the position of the chart
    const chart_position: DOMRect = context.chart.canvas.getBoundingClientRect();

    // Set left position
    let left_position: number = chart_position.left + window.pageXOffset + tooltip_model.caretX;

    // Set tooltip with
    const tooltip_width: number = tooltip_element.offsetWidth;

    // Verify if left position is over screen
    if ( (left_position + tooltip_width) > window.innerWidth ) {

        // Set new left position
        left_position = (left_position - tooltip_width);

    }

    // Set top position
    let top_position: number = chart_position.top + window.pageYOffset + tooltip_model.caretY;

    // Set tooltip height
    const tooltip_height: number = tooltip_element.offsetHeight;   
    
    // Verify if top position is over screen
    if ( (top_position + tooltip_height) > window.innerHeight ) {

        // Set new top position
        top_position = (top_position - (tooltip_height + 15));

    } 

    // Display, position, and set styles for tooltip
    tooltip_element.style.opacity = '1';
    tooltip_element.style.position = 'absolute';
    tooltip_element.style.left = left_position + 'px';
    tooltip_element.style.top = top_position + 'px';
    tooltip_element.style.padding = tooltip_model.padding + 'px ' + tooltip_model.padding + 'px';
    tooltip_element.style.pointerEvents = 'none';

}

// Create the ChartLine
const ChartLine: React.FC<{data: Array<{[key: string]: string | number}>, datasets: string[]}> = ({data, datasets}): React.JSX.Element => {

    // Member options
    const {memberOptions, setMemberOptions} = useContext(MemberOptionsContext); 

    // Create reference for canvas
    const chartRef = useRef<HTMLCanvasElement | null>(null);

    // My chart container
    let myChart: Chart<"line", string[], string> | null = null;   
    
    // Run content after load
    useEffect((): () => void => {

        return (): void => {

            // Get tooltip
            const tooltip: HTMLElement | null = document.getElementById('fc-chart-js-tooltip');

            // Verify if tooltip exists
            if ( tooltip ) {

                // Remove tooltip
                tooltip.remove();

            }

        };

    }, []);    

    // Run content after load
    useEffect(() => {

        // Verify if chart exists
        if ( chartRef.current ) {

            // Labels container
            const labels: string[] = [];

            // Values container
            const values: string[] = []; 

            // Verify if data exists
            if ( data.length > 0 ) {

                // List the data
                for ( const value of data ) {

                    // Prepare the value
                    const readyValue = value as {key: string, count: string};

                    // Set label
                    labels.push(readyValue.key);

                    // Set value
                    values.push(readyValue.count);                    

                }

            }

            // Datasets container
            const datasetsList = [];

            // Check if datasets is not empty
            if ( datasets.length > 0 ) {

                // List the datasets
                for ( const dataset of datasets ) {

                    // Add dataset
                    datasetsList.push({
                        label: dataset,
                        data: values,
                        fill: false,
                        borderColor: '#33478c',
                        tension: 0.4,
                        borderWidth: 2,
                        pointBorderWidth: 2,
                        pointRadius: 3,
                        pointBackgroundColor: "#FFFFFF",
                        drawBorder: false
                    });

                }

            }

            // Set data
            const chartData = {
                labels: labels,
                datasets: datasetsList
            };

            // Draw the chart
            myChart = new Chart(chartRef.current!, {
                type: 'line',
                data: chartData,
                options: {
                    scales: {
                        y: {
                            border: {
                                display: false,
                            },
                            grid:  {
                                tickLength: 0,
                                lineWidth: 0,
                                color: function() {
                                    return '#000000';
                                }
                            },
                            ticks: {
                                callback: function () {
                                    return "";
                                }
                            }
                        },
                        x: {
                            border: {
                                display: false,
                            },
                            grid: {
                                display: false,
                            },
                            ticks: {
                                padding: 15,
                                font: {
                                    family: "'Roboto', 'Open Sans', 'Nunito Sans', sans-serif",
                                    size: 10,
                                    weight: '400'
                                },
                                color: "#6f78a9",
                                callback: function (val: any) {

                                    // Split
                                    const values: string[] = this.getLabelForValue(val).split('/');

                                    // Set date
                                    const texts: string[] = [values[2]];

                                    // Set month
                                    texts.push(getMonth(values[1].padStart(2, '0'), memberOptions['Language']));
                        
                                    return texts;
                                }

                            }

                        }
                        
                    },
                    plugins: {
                        
                        legend: {
                            display: false,
                        },
                        tooltip: {
                            enabled: false,
                            mode: 'index',
                            position: 'average',
                            external: (context: any): void => {
                                
                                // Replace the tooltip
                                fcChartJsTooltip(context, memberOptions['Language']);
                                
                            }
                        },
                    },
                    maintainAspectRatio: false,
                    responsive: true
                }

            });

            // Return a cleanup function to destroy the chart
            return () => {

                // Check if chart exists
                if (myChart) {
                    myChart.destroy();
                }

            };

        }

    }, [data]);

    // Handle mouse leave from chart
    const hideTooltip = (): void => {

        // Get tooltip
        const tooltip: HTMLElement | null = document.getElementById('fc-chart-js-tooltip');

        // Verify if tooltip exists
        if ( tooltip ) {

            // Remove tooltip
            tooltip.remove();

        }

    };

    // Return the chart
    return (
        <canvas ref={chartRef} onMouseLeave={hideTooltip} />
    );

}

export default ChartLine;