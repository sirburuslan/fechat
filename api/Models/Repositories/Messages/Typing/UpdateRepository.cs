/*
 * @class Typing Update Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to update the typing
 */

// Namespace for Messages Typing Repositories
namespace FeChat.Models.Repositories.Messages.Typing {

    // System Namespaces
    using Microsoft.EntityFrameworkCore;

    // App Namespaces
    using Models.Entities.Messages;
    using Utils.Configuration;

    /// <summary>
    /// Typing Update Repository
    /// </summary>
    public class UpdateRepository {

        /// <summary>
        /// Plans table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Typing Update Repository Constructor
        /// </summary>
        /// <param name="db">Db connection instance</param>
        public UpdateRepository(Db db) {

            // Save the session
            _context = db;

        }
        
        /// <summary>
        /// Update Typing ID
        /// </summary>
        /// <param name="typingId">Typing Id</param>
        public async Task UpdateTypingAsync(int typingId) {

            try {

                // Get the entity
                TypingEntity? typingEntity = await _context.Typing.FirstOrDefaultAsync(t => t.Id == typingId);

                // Verify if entity type exists
                if ( typingEntity != null ) {

                    // Update the time
                    typingEntity.Updated = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                    // Mark the item as modified
                    _context.Entry(typingEntity).State = EntityState.Modified;

                    // Save the changes
                    await _context.SaveChangesAsync();

                }

            } catch (Exception ex) {

                Console.WriteLine(ex.Message);                

            }

        }

    }
    
}