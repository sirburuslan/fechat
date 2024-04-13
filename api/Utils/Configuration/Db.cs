/*
 * @class Db
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-01-26
 *
 * This class is used create a session to the database
 */

// Namespace for Configuration Utils
namespace FeChat.Utils.Configuration {

    // System Namespaces
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    // App Namespaces
    using Models.Entities.Events;
    using Models.Entities.Members;
    using Models.Entities.Messages;
    using Models.Entities.Plans;
    using Models.Entities.Subscriptions;
    using Models.Entities.Settings;
    using Models.Entities.Transactions;
    using Models.Entities.Websites;
    using Utils.General;

    /// <summary>
    /// Database connection
    /// </summary>
    public class Db: DbContext {

        /// <summary>
        /// App Settings container.
        /// </summary>
        private readonly AppSettings _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="Db"/> class.
        /// </summary>
        /// <param name="options">All App Options.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> is null.</exception>
        public Db(IOptions<AppSettings> options) {

            // Save the configuration
            _options = options.Value ?? throw new ArgumentNullException(nameof(options), new Strings().Get("OptionsNotFound"));
            
        }

        /// <summary>
        /// Set the entity for Attachments
        /// </summary>
        public DbSet<AttachmentEntity> Attachments { get; set; }   
        
        /// <summary>
        /// Set the entity for Events
        /// </summary>
        public DbSet<EventEntity> Events { get; set; }          

        /// <summary>
        /// Set the entity for Guests
        /// </summary>
        public DbSet<GuestEntity> Guests { get; set; }

        /// <summary>
        /// Set the entity for Members
        /// </summary>
        public DbSet<MemberEntity> Members { get; set; }

        /// <summary>
        /// Set the entity for Members Options
        /// </summary>
        public DbSet<MemberOptionsEntity> MembersOptions { get; set; }

        /// <summary>
        /// Set the entity for Messages
        /// </summary>
        public DbSet<MessageEntity> Messages { get; set; }

        /// <summary>
        /// Set the entity for Plans
        /// </summary>
        public DbSet<PlanEntity> Plans { get; set; }

        /// <summary>
        /// Set the entity for Plans Meta
        /// </summary>
        public DbSet<PlansMetaEntity> PlansMeta { get; set; }

        /// <summary>
        /// Set the entity for Plans Restrictions
        /// </summary>
        public DbSet<PlansRestrictionsEntity> PlansRestrictions { get; set; }

        /// <summary>
        /// Set the entity for Plans Features
        /// </summary>
        public DbSet<PlansFeaturesEntity> PlansFeatures { get; set; }

        /// <summary>
        /// Set the entity for Settings
        /// </summary>
        public DbSet<SettingsEntity> Settings { get; set; }

        /// <summary>
        /// Set the entity for Subscriptions
        /// </summary>
        public DbSet<SubscriptionEntity> Subscriptions { get; set; }

        /// <summary>
        /// Set the entity for Subscriptions Meta
        /// </summary>
        public DbSet<SubscriptionsMetaEntity> SubscriptionsMeta { get; set; }

        /// <summary>
        /// Set the entity for Transactions
        /// </summary>
        public DbSet<TransactionEntity> Transactions { get; set; }

        /// <summary>
        /// Set the entity for Threads
        /// </summary>
        public DbSet<ThreadEntity> Threads { get; set; }

        /// <summary>
        /// Set the entity for Typing
        /// </summary>
        public DbSet<TypingEntity> Typing { get; set; }        

        /// <summary>
        /// Set the entity for Websites
        /// </summary>
        public DbSet<WebsiteEntity> Websites { get; set; }   

        /// <summary>
        /// Members table connection
        /// </summary>
        /// <param name="optionsBuilder">The entity framework settings builder</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {

            // Get the connection string
            string connectionString = _options.ConnectionStrings.DefaultConnection ?? throw new ArgumentNullException(new Strings().Get("OptionsNotFound"));

            // Set the connection string and connect the database
            optionsBuilder.UseNpgsql(connectionString);

        }

    }

}