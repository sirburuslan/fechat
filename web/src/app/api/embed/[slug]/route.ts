/*
 * @library FeChat
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-02-25
 */

// Export the JS code
export async function GET(request: Request, { params }: { params: { slug: string } } ) {

    return new Response(`
(() => {
          
    let iframe = document.createElement('iframe');

    iframe.src = '${process.env.NEXT_PUBLIC_SITE_URL}chat/${params.slug}';

    iframe.classList.add('fc-iframe-chat');

    iframe.style.cssText = 'position:fixed;right:15px;bottom:15px;z-index:9999;width:80px;height:70px;border:0;transition: height 0.2s ease-in;';

    document.getElementsByTagName('body')[0].appendChild(iframe);

    let chat = document.getElementsByClassName('fc-iframe-chat')[0];      

    window.addEventListener('message', function(event) {

        if (event.source === chat.contentWindow) {
            
            if ( event.data === 'show' ) {
                
                if ( document.documentElement.clientWidth < 500 ) {

                    chat.style.width = (document.documentElement.clientWidth - 40) + 'px';

                } else {

                    chat.style.width = '460px';

                }

                if ( document.documentElement.clientHeight < 760 ) {

                    chat.style.height = (document.documentElement.clientHeight - 40) + 'px';

                } else {

                    chat.style.height = '700px';

                }

            } else if ( event.data === 'hide' ) {
                
                chat.style.height = '0';

                setTimeout(() => {
                    chat.style.width = '80px'
                    chat.style.height = '70px';
                }, 300);
                
            } else if ( typeof event.data.domain != 'undefined' ) {

                if ( window.location.hostname !== event.data.domain ) {

                    document.getElementsByTagName('body')[0].removeChild(iframe);

                    console.log('ChatError: The chat is created for another domain.');

                }

            }

        }

    });

})();
        `,
        {
          headers: {
            'Content-Type': 'text/javascript',
          },
        }
    )

}