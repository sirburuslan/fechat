/*
 * @class Transactions Read Controller
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to read the transactions
 */

// Namespace for Administrator Transactions Controllers
namespace FeChat.Controllers.Administrator.Transactions {

    // Used Mvc to get the Controller feature
    using Microsoft.AspNetCore.Mvc;

    // Use the Authorization to restrict access for guests
    using Microsoft.AspNetCore.Authorization;

    // Use the Cors feature to control the access
    using Microsoft.AspNetCore.Cors;

    // Use the Versioning to add version in url
    using Asp.Versioning;
    
    // Use General dtos classes
    using FeChat.Models.Dtos;

    // Use Transactions dtos classes
    using FeChat.Models.Dtos.Transactions;
    
    // Use the General Utils classes for Strings
    using FeChat.Utils.General;

    // Use the Transactions Repositories
    using FeChat.Utils.Interfaces.Repositories.Subscriptions;

    /// <summary>
    /// Transactions Read Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/transactions")]
    public class ReadController: Controller {

        /// <summary>
        /// Container for app's configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor for this controller
        /// </summary>
        /// <param name="configuration">App configuration</param>
        public ReadController(IConfiguration configuration) {

            // Add configuration to the container
            _configuration = configuration;

        }

        /// <summary>
        /// Get the list with transactions
        /// </summary>
        /// <param name="searchDto">Search parameters</param>
        /// <param name="planId">Plan Id</param>
        /// <param name="subscriptionsRepository">An instance to the Subscription repository</param>
        /// <returns>Transactions list or error message</returns>
        [Authorize]
        [HttpPost("list/{planId?}")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> TransactionsList([FromBody] SearchDto searchDto, int? planId, ISubscriptionsRepository subscriptionsRepository) {

            // Verify if antiforgery is valid
            if ( await new Antiforgery(HttpContext, _configuration).Validate() == false ) {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("InvalidCsrfToken")
                });

            }

            // Get the transactions
            ResponseDto<ElementsDto<TransactionDetailsDto>> transactionsList = await subscriptionsRepository.GetTransactionsByPageAsync(searchDto, planId);

            // Verify if transactions exists
            if ( transactionsList.Result != null ) {

                // Return transactions response
                return new JsonResult(new {
                    success = true,
                    transactionsList.Result,
                    time = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                });

            } else {

                // Return error response
                return new JsonResult(new {
                    success = false,
                    message = transactionsList.Message
                });

            }

        }

        /// <summary>
        /// Gets the transaction information
        /// </summary>
        /// <param name="transactionId">Contains the transaction's ID</param>
        /// <param name="subscriptionsRepository">An instance to the Subscription repository</param>
        /// <returns>Transaction details or error message</returns>
        [Authorize]
        [HttpGet("{transactionId}")]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> Transaction(int transactionId, ISubscriptionsRepository subscriptionsRepository) {

            // Get the transaction's data
            ResponseDto<TransactionDetailsDto> transactionDetails = await subscriptionsRepository.GetTransactionAsync(transactionId);

            // Verify if transaction exists
            if ( transactionDetails.Result != null ) {

                // Return a json
                return new JsonResult(new {
                    success = true,
                    transaction = transactionDetails,
                    time = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                });

            } else {

                // Return a json
                return new JsonResult(new {
                    success = false,
                    message = new Strings().Get("TransactionNotFound")
                });

            }

        }

    }

}