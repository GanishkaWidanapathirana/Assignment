using System.Text.Json;

namespace PublicApiCaller.Services
{
    public interface IRestCountriesApiService
    {
        Task<JsonDocument> GetCountriesAsync();
    }
}
