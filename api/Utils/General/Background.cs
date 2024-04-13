/*
 * @class Background Service
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-25
 *
 * This class runs tasks in background
 */

// Namespace for General Utils
namespace FeChat.Utils.General {

    // App Namespaces
    using Models.Dtos;
    using Models.Dtos.Messages;
    using Utils.Interfaces.Repositories.Messages;
    using Utils.Interfaces.Repositories.Settings;

    /// <summary>
    /// Background Service Class
    /// </summary>
    public class Background: BackgroundService {

        // Services container
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="serviceProvider">Services holder</param>
        public Background(IServiceProvider serviceProvider) {

            // Save service holder
            _serviceProvider = serviceProvider;

        }

        /// <summary>
        /// Ovveride the BackgroundService method
        /// </summary>
        /// <param name="stoppingToken">Cancellation Token</param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            
            // Creates a new IServiceScope
            using var scope = _serviceProvider.CreateScope();

            // Get the settings repository
            ISettingsRepository settingsRepository = scope.ServiceProvider.GetRequiredService<ISettingsRepository>();
            
            // Get the messages repository
            IMessagesRepository messagesRepository = scope.ServiceProvider.GetRequiredService<IMessagesRepository>();

            // Run
            while (!stoppingToken.IsCancellationRequested) {

                // Get all unseen messages
                ResponseDto<List<UnseenMessageDto>> messagesList = await messagesRepository.AllMessagesUnseenAsync();

                // Verify if unseen messages exists
                if ( messagesList.Result != null ) {

                    // Get the options saved in the database
                    ResponseDto<List<Models.Dtos.Settings.OptionDto>> savedOptions = await settingsRepository.OptionsListAsync();

                    // Lets create a new dictionary list
                    Dictionary<string, string> optionsList = new();

                    // Verify if options exists
                    if ( savedOptions.Result != null ) {

                        // Get options length
                        int optionsLength = savedOptions.Result.Count;

                        // List the saved options
                        for ( int o = 0; o < optionsLength; o++ ) {

                            // Add option to the dictionary
                            optionsList.Add(savedOptions.Result[o].OptionName, savedOptions.Result[o].OptionValue!);

                        }

                    }

                    // Saved emails
                    List<int> savedMembers = new();

                    // Get total messages
                    int totalMessages = messagesList.Result.Count;

                    // List the total messages
                    for ( int m = 0; m < totalMessages; m++ ) {

                        // Verify if member is saved
                        if ( savedMembers.Contains(messagesList.Result[m].ThreadOwner) ) {
                            continue;
                        }

                        // Create email body content
                        string body = "<p>" + new Strings().Get("YouHaveNewUnreadMessage") + "</p><p>" + new Strings().Get("BestRegards") + "</p>";

                        // Send email
                        await new Sender().Send(optionsList, messagesList.Result[m].Email ?? string.Empty, new Strings().Get("NewUnreadMessage"), body);

                        // Save member
                        savedMembers.Add(messagesList.Result[m].ThreadOwner);

                    }

                }
                
                // Set a delay for one minute
                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);

            }
            

        }

    }

}