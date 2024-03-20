/*
 * @class Subscriptions Transactions Read Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-17
 *
 * This class is used to read the transactions
 */

// Namespace for Subscriptions Transactions Repositories
namespace FeChat.Models.Repositories.Subscriptions.Transactions {

    // Use Entity Framework Core to extend Linq features
    using Microsoft.EntityFrameworkCore;

    // Use Memory catching
    using Microsoft.Extensions.Caching.Memory;

    // Use General Dtos
    using FeChat.Models.Dtos;

    // Use Transactions Dtos
    using FeChat.Models.Dtos.Transactions;

    // Use Transactions Entities
    using FeChat.Models.Entities.Transactions;

    // Use General Utils to access the strings
    using FeChat.Utils.General;

    // Use the Configuration Utils
    using FeChat.Utils.Configuration;

    /// <summary>
    /// Transactions Read Repository
    /// </summary>
    public class ReadRepository {

        /// <summary>
        /// Memory cache container
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Transactions table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Transactions Read Repository Constructor
        /// </summary>
        /// <param name="memoryCache">Membery cache instance</param>
        /// <param name="db">Db connection instance</param>
        public ReadRepository(IMemoryCache memoryCache, Db db) {

            // Save the memory chache
            _memoryCache = memoryCache;

            // Save the session
            _context = db;

        }

        /// <summary>
        /// Gets transactions by page
        /// </summary>
        /// <param name="searchDto">Search parameters</param>
        /// <param name="planId">Plan Id</param>
        /// <returns>List with transactions</returns>
        public async Task<ResponseDto<ElementsDto<TransactionDetailsDto>>> GetTransactionsByPageAsync(SearchDto searchDto, int? planId) {
                    // Remove the cache key in the group
                    new Cache(_memoryCache).Remove("transactions");
            try {

                // Prepare the page
                int page = (searchDto.Page > 0)?searchDto.Page:1;

                // Prepare the total results
                int total = 10;

                // Create the cache key
                string cacheKey = "fc_transactions_" + searchDto.Page;

                // Verify if the cache is saved
                if ( !_memoryCache.TryGetValue(cacheKey, out Tuple<List<TransactionDetailsDto>, int>? transactionsResponse ) ) {

                    // Transactions entities
                    IQueryable<TransactionEntity> transactionEntities = _context.Transactions.AsQueryable();

                    // Verify if plan id is not null
                    if ( planId != null ) {

                        // Group by plan id
                        transactionEntities = transactionEntities.Where(t => t.PlanId == planId);

                    }

                    // Request the transactions
                List<TransactionDetailsDto> transactions = await transactionEntities
                    .Select(t => new TransactionDetailsDto {
                        TransactionId = t.TransactionId,
                        MemberId = t.MemberId,
                        SubscriptionId = t.SubscriptionId,
                        PlanId = t.PlanId,
                        OrderId = t.OrderId,
                        NetId = t.NetId,
                        Source = t.Source,
                        Created = t.Created
                    })
                    .Join(
                        _context.Plans,
                        t => t.PlanId,
                        p => p.PlanId,
                        (t, p) => new {
                            Transaction = t,
                            Plan = p
                        }
                    )
                    .GroupJoin(
                        _context.MembersOptions.Where(opt => opt.OptionName == "ProfilePhoto"),
                        tp => tp.Transaction.MemberId,
                        o => o.MemberId,
                        (tp, options) => new {
                            tp.Transaction,
                            tp.Plan,
                            ProfilePhotos = options
                        }
                    )
                    .SelectMany(
                        tp => tp.ProfilePhotos.DefaultIfEmpty(),
                        (tp, o) => new TransactionDetailsDto {
                            TransactionId = tp.Transaction.TransactionId,
                            MemberId = tp.Transaction.MemberId,
                            SubscriptionId = tp.Transaction.SubscriptionId,
                            PlanId = tp.Transaction.PlanId,
                            OrderId = tp.Transaction.OrderId,
                            NetId = tp.Transaction.NetId,
                            Source = tp.Transaction.Source,
                            Created = tp.Transaction.Created,
                            PlanName = tp.Plan.Name,
                            PlanPrice = tp.Plan.Price,
                            PlanCurrency = tp.Plan.Currency,
                            ProfilePhoto = o == null ? null : o.OptionValue
                        }
                    )
                    .OrderByDescending(m => m.TransactionId)
                    .Skip((page - 1) * total)
                    .Take(total)
                    .ToListAsync();


                    // Get the total count before pagination
                    int totalCount = await _context.Transactions.CountAsync();

                    // Add data to transaction response
                    transactionsResponse = new Tuple<List<TransactionDetailsDto>, int>(transactions, totalCount);

                    // Create the cache options for storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Save the request in the cache
                    _memoryCache.Set(cacheKey, transactionsResponse, cacheOptions);

                    // Save the cache key in the group
                    new Cache(_memoryCache).Save("transactions", cacheKey);

                }

                // Verify if transactions exists
                if ( (transactionsResponse != null) && (transactionsResponse.Item1.Count > 0) ) {

                    // Return the response
                    return new ResponseDto<ElementsDto<TransactionDetailsDto>> {
                        Result = new ElementsDto<TransactionDetailsDto> {
                            Elements = transactionsResponse.Item1,
                            Total = transactionsResponse.Item2,
                            Page = searchDto.Page
                        },
                        Message = null
                    };

                } else {

                    // Return the response
                    return new ResponseDto<ElementsDto<TransactionDetailsDto>> {
                        Result = null,
                        Message = new Strings().Get("NoTransactionsFound")
                    };

                }

            } catch ( InvalidOperationException e ) {

                // Return the response
                return new ResponseDto<ElementsDto<TransactionDetailsDto>> {
                    Result = null,
                    Message = e.Message
                };                

            }

        }

        /// <summary>
        /// Gets transactions by id
        /// </summary>
        /// <param name="transactionId">Transaction ID</param>
        /// <returns>Transaction details or error message</returns>
        public async Task<ResponseDto<TransactionDetailsDto>> GetTransactionAsync(int transactionId) {

            try {

                // Create the cache key
                string cacheKey = "fc_transaction_" + transactionId;

                // Verify if the cache is saved
                if ( !_memoryCache.TryGetValue(cacheKey, out TransactionDetailsDto? transactionResponse ) ) {

                    // Request the transaction by id
                    transactionResponse = await _context.Transactions
                    .Select(t => new TransactionDetailsDto {
                        TransactionId = t.TransactionId,
                        MemberId = t.MemberId,
                        SubscriptionId = t.SubscriptionId,
                        PlanId = t.PlanId,
                        OrderId = t.OrderId,
                        NetId = t.NetId,
                        Source = t.Source,
                        Created = t.Created
                    })
                    .Join(
                        _context.Members,
                        t => t.MemberId,
                        m => m.MemberId,
                        (t, m) => new { Transaction = t, Member = m }
                    )
                    .Join(
                        _context.Plans,
                        tm => tm.Transaction.PlanId,
                        p => p.PlanId,
                        (tm, p) => new { TransactionMember = tm, Plan = p }
                    )
                    .GroupJoin(
                        _context.MembersOptions.Where(opt => opt.OptionName == "ProfilePhoto"),
                        tmp => tmp.TransactionMember.Transaction.MemberId,
                        o => o.MemberId,
                        (tmp, o) => new { TransactionMemberPlan = tmp, ProfilePhoto = o.FirstOrDefault() }
                    )
                    .Select(tmpo => new TransactionDetailsDto {
                        TransactionId = tmpo.TransactionMemberPlan.TransactionMember.Transaction.TransactionId,
                        MemberId = tmpo.TransactionMemberPlan.TransactionMember.Transaction.MemberId,
                        SubscriptionId = tmpo.TransactionMemberPlan.TransactionMember.Transaction.SubscriptionId,
                        PlanId = tmpo.TransactionMemberPlan.TransactionMember.Transaction.PlanId,
                        OrderId = tmpo.TransactionMemberPlan.TransactionMember.Transaction.OrderId,
                        NetId = tmpo.TransactionMemberPlan.TransactionMember.Transaction.NetId,
                        Source = tmpo.TransactionMemberPlan.TransactionMember.Transaction.Source,
                        Created = tmpo.TransactionMemberPlan.TransactionMember.Transaction.Created,
                        PlanName = tmpo.TransactionMemberPlan.Plan.Name,
                        PlanPrice = tmpo.TransactionMemberPlan.Plan.Price,
                        PlanCurrency = tmpo.TransactionMemberPlan.Plan.Currency,
                        ProfilePhoto = tmpo.ProfilePhoto != null ? tmpo.ProfilePhoto.OptionValue : null,
                        FirstName = tmpo.TransactionMemberPlan.TransactionMember.Member.FirstName,
                        LastName = tmpo.TransactionMemberPlan.TransactionMember.Member.LastName
                    })
                    .FirstAsync(t => t.TransactionId == transactionId);

                    // Create the cache options for storing
                    MemoryCacheEntryOptions cacheOptions = new() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    };

                    // Save the request in the cache
                    _memoryCache.Set(cacheKey, transactionResponse, cacheOptions);

                }

                // Verify if transaction exists
                if ( transactionResponse != null ) {

                    // Return the response
                    return new ResponseDto<TransactionDetailsDto> {
                        Result = transactionResponse,
                        Message = null
                    };

                } else {

                    // Return the response
                    return new ResponseDto<TransactionDetailsDto> {
                        Result = null,
                        Message = new Strings().Get("TransactionNotFound")
                    };

                }

            } catch ( InvalidOperationException e ) {

                // Return the response
                return new ResponseDto<TransactionDetailsDto> {
                    Result = null,
                    Message = e.Message
                };                

            }

        }

    }

}