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

    // Use the Entity Framework Core for the DbContext class
    using Microsoft.EntityFrameworkCore;

    // Use the Settings Entities
    using FeChat.Models.Entities.Settings;

    // Use the Events Entities
    using FeChat.Models.Entities.Events;    

    // Use the Members Entities
    using FeChat.Models.Entities.Members;

    // Use the Messages Entities
    using FeChat.Models.Entities.Messages;

    // Use the Plans Entities
    using FeChat.Models.Entities.Plans;

    // Use Subscriptions Entities
    using FeChat.Models.Entities.Subscriptions;

    // Use the Transactions Entities
    using FeChat.Models.Entities.Transactions;    

    // Use the Websites Entities
    using FeChat.Models.Entities.Websites;

    /// <summary>
    /// Database connection
    /// </summary>
    public class Db: DbContext {

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
        /// Configuration container
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="options">The entity framework settings</param>
        /// <param name="configuration">Contains the configuration settongs</param>
        public Db(DbContextOptions<Db> options, IConfiguration configuration): base(options) {

            // Set the app's configuration
            _configuration = configuration;

        }

        /// <summary>
        /// Members table connection
        /// </summary>
        /// <param name="optionsBuilder">The entity framework settings builder</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {

            // Get the connection string
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            // Set the connection string and connect the database
            optionsBuilder.UseNpgsql(connectionString);

        }

    }

}