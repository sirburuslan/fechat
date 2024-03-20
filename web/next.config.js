/** @type {import('next').NextConfig} */

// Require the Node's path module
const path = require('path');

const nextConfig = {

    // Options for styles
    sassOptions: {

        includePaths: [
            path.join(__dirname, 'src/app/styles')
        ],
        
    },
    reactStrictMode: false,
    images: {
        remotePatterns: [
            {
                protocol: 'https',
                hostname: 'i.imgur.com',
                port: ''
            }
        ]
    }

}

module.exports = nextConfig
