/*
 * @class Attachments Create Repository
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This class is used to create the attachments
 */

// Namespace for Messages Attachments Repositories
namespace FeChat.Models.Repositories.Messages.Attachments {

    // Use the General Dtos
    using FeChat.Models.Dtos;

    // Use the Messages Dtos
    using FeChat.Models.Dtos.Messages;

    // Use The Entities for Messages
    using FeChat.Models.Entities.Messages;

    // Use the Configuration Utils
    using FeChat.Utils.Configuration;

    // Use the General Utils for strings
    using FeChat.Utils.General;

    /// <summary>
    /// Attachments Create Repository
    /// </summary>
    public class CreateRepository {

        /// <summary>
        /// Plans table context container
        /// </summary>   
        private readonly Db _context;

        /// <summary>
        /// Attachments Create Repository Constructor
        /// </summary>
        /// <param name="db">Db connection instance</param>
        public CreateRepository(Db db) {

            // Save the session
            _context = db;

        }

        /// <summary>
        /// Create attachment
        /// </summary>
        /// <param name="attachmentDto">Attachment data</param>
        /// <returns>Saved attachment data or message</returns>
        public async Task<ResponseDto<AttachmentDto>> CreateAttachmentAsync(AttachmentDto attachmentDto) {

            try {

                // Create the attachment entity
                AttachmentEntity attachmentEntity = new() {
                    MessageId = attachmentDto.MessageId,
                    Link = attachmentDto.Link,
                    Created = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                // Add attachment to the entity
                _context.Add(attachmentEntity);

                // Save entity
                int saveEntity = await _context.SaveChangesAsync();

                // Check if the entity was saved
                if ( saveEntity > 0 ) {

                    return new ResponseDto<AttachmentDto> {
                        Result = new AttachmentDto {
                            AttachmentId = attachmentEntity.AttachmentId,
                            MessageId = attachmentEntity.MessageId,
                            Link = attachmentEntity.Link,
                            Created = attachmentEntity.Created
                        },
                        Message = new Strings().Get("MessageAttachmentSaved")
                    };

                } else {

                    return new ResponseDto<AttachmentDto> {
                        Result = null,
                        Message = new Strings().Get("MessageAttachmentNotSaved")
                    };

                }

            } catch ( Exception ex ) {

                return new ResponseDto<AttachmentDto> {
                    Result = null,
                    Message = ex.Message
                };

            }

        }

    }
    
}