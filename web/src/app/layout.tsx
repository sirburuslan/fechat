/*
 * @layout Main
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file contains the main app layout
 */

'use client'

// Import function from TanStack Query
import { QueryClient, QueryClientProvider } from 'react-query';

// Use the member options provider
import {OptionsProvider} from '@/core/contexts/OptionsContext';

// Import the Loading
import Loading from "@/core/components/general/Loading";

// Import Tailwind
import './globals.css';

// Create an instance of QueryClient for managing caching, fetching, and updating data
const queryClient = new QueryClient();

// Export the Root Layout
export default function RootLayout({ children }: { children: React.ReactNode }) {

  return (
    <>
      <QueryClientProvider client={queryClient}>
        <OptionsProvider>
          <html>
            <body>
              {children}
              <Loading />
            </body>
          </html>
        </OptionsProvider>      
      </QueryClientProvider>
    </>
  );
  
}
