/*
 * @class Subscriptions Transactions Create Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to create the transactions
 */

// Namespace for Subscriptions Transactions Repositories
namespace FeChat.Models.Repositories.Subscriptions.Transactions {

    // System Namespaces
    using Microsoft.Extensions.Caching.Memory;

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Transactions;
    using Models.Entities.Transactions;
    using Utils.General;
    using Utils.Configuration;

    /// <summary>
    /// Transactions Create Repository
    /// </summary>
    public class CreateRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Transactions table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Transactions Create Repository Constructor
        /// </summary>
        /// <param name="memoryCache">Membery cache instance</param>
        /// <param name="db">Db connection instance</param>
        public CreateRepository(IMemoryCache memoryCache, Db db) {

            // Save the memory chache
            _memoryCache = memoryCache;

            // Save the session
            _context = db;

        }

        /// <summary>
        /// Create a transaction
        /// </summary>
        /// <param name="transactionDto">Transaction information</param>
        /// <returns>Transaction id or error message</returns>
        public async Task<ResponseDto<TransactionDto>> CreateTransactionAsync(TransactionDto transactionDto) {

            try {

                // Create the entity with transaction data
                TransactionEntity transactionEntity = new() {
                    MemberId = transactionDto.MemberId,
                    SubscriptionId = transactionDto.SubscriptionId,
                    PlanId = transactionDto.PlanId,
                    OrderId = transactionDto.OrderId,
                    NetId = transactionDto.NetId,
                    Source = transactionDto.Source,
                    Created = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                // Add entity to the Transactions list
                _context.Transactions.Add(transactionEntity);

                // Save the changes
                int saveChanges = await _context.SaveChangesAsync();

                // Verify if the entity was saved
                if ( saveChanges > 0 ) {

                    // Remove the cache key in the group
                    new Cache(_memoryCache).Remove("transactions");

                    // Return success response
                    return new ResponseDto<TransactionDto> {
                        Result = new TransactionDto {
                            TransactionId = transactionEntity.TransactionId
                        },
                        Message = new Strings().Get("TransactionCreated")
                    };

                } else {

                    // Return error response
                    return new ResponseDto<TransactionDto> {
                        Result = null,
                        Message = new Strings().Get("TransactionNotCreated")
                    };

                }

            } catch ( Exception ex ) {

                // Return error message
                return new ResponseDto<TransactionDto> {
                    Result = null,
                    Message = ex.Message
                };

            }

        }

    }

}