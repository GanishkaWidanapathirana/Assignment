using System.Text.Json;

namespace PublicApiCaller.Services
{
    public class RestCountriesApiService : IRestCountriesApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RestCountriesApiService> _logger;

        public RestCountriesApiService(
            HttpClient httpClient,
            ILogger<RestCountriesApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<JsonDocument> GetCountriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    "v3.1/all?fields=name,capital,currencies,region,population,languages,capital,area"
                );

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(
                        "External API returned status code: {StatusCode}",
                        response.StatusCode
                    );

                    throw new ApplicationException(
                        "Failed to fetch countries from external API."
                    );
                }

                var content = await response.Content.ReadAsStringAsync();

                return JsonDocument.Parse(content);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed.");

                throw new ApplicationException(
                    "Failed to connect to external API."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred.");

                throw;
            }
        }
    }
}
