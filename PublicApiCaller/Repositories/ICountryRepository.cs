using PublicApiCaller.DTOs;
using System.Text.Json;

namespace PublicApiCaller.Repositories
{
    public interface ICountryRepository
    {
        Task<List<CountryDto>> GetAllCountriesAsync();

        Task<CountryDto?> GetCountryByIdAsync(int id);

        Task<bool> HasCountriesAsync();

        Task InsertCountryDataAsync(JsonElement country);
    }
}
