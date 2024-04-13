/*
 * @class Ip Lookup
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-18
 *
 * This class is used to parse data from IP address
 */

// Namespace for General Utils
namespace FeChat.Utils.General {

    // App Namespaces
    using Models.Dtos;

    /// <summary>
    /// Ip Lookup
    /// </summary>
    public class IpLookup {

        /// <summary>
        /// Get data from IP
        /// </summary>
        /// <param name="optionsList">Website Options</param>
        /// <param name="ip">IP</param>
        /// <returns>Ip information or error message</returns>
        public async Task<ResponseDto<IpDto>> GetIpData(Dictionary<string, string> optionsList, string ip) {

            try {

                // Get the Ip 2 Location Status
                optionsList.TryGetValue("Ip2LocationEnabled", out string? ip2LocationEnabled);
                
                // Get the Ip 2 Location Key
                optionsList.TryGetValue("Ip2LocationKey", out string? ip2LocationKey); 

                // Verify if Ip 2 Location is configured
                if ( (ip2LocationEnabled == null) || (ip2LocationEnabled == "0") || (ip2LocationKey == null) || (ip2LocationKey == "") ) {

                    return new ResponseDto<IpDto> {
                        Result = null,
                        Message = "Ip2Location is not configured."
                    };
                    
                }        

                // Init the Http Client
                using HttpClient httpClient = new();

                // Request ip details
                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("https://api.ip2location.io/?ip=" + ip + "&key=" + ip2LocationKey);

                // Verify if data exists
                if ( httpResponseMessage.IsSuccessStatusCode ) {

                    // Get the response as string
                    string ipDataJson = await httpResponseMessage.Content.ReadAsStringAsync();

                    // Decode the data
                    dynamic dataDecode = Newtonsoft.Json.JsonConvert.DeserializeObject(ipDataJson)!;

                    return new ResponseDto<IpDto> {
                        Result = new IpDto {
                            Ip = dataDecode.ip,
                            CountryCode = dataDecode.country_code,
                            CountryName = dataDecode.country_name,
                            RegionName = dataDecode.region_name,
                            CityName = dataDecode.city_name,
                            Latitude = dataDecode.latitude,
                            Longitude = dataDecode.longitude,
                            ZipCode = dataDecode.zip_code,
                            TimeZone = dataDecode.time_zone
                        },
                        Message = null
                    };

                } else {

                    // Request failed
                    string errorMessage = await httpResponseMessage.Content.ReadAsStringAsync();

                    // Decode the error message
                    dynamic errorMessageDecode = Newtonsoft.Json.JsonConvert.DeserializeObject(errorMessage)!;                    

                    // Return error
                    return new ResponseDto<IpDto> {
                        Result = null,
                        Message = errorMessageDecode.message
                    };

                }

            } catch ( Exception ex ) {

                return new ResponseDto<IpDto> {
                    Result = null,
                    Message = ex.Message
                };

            }

        }

    }

}