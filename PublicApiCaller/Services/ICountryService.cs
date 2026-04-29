using PublicApiCaller.DTOs;

namespace PublicApiCaller.Services
{
    public interface ICountryService
    {
        Task<List<CountryDto>> GetCountriesAsync();

        Task<CountryDto?> GetCountryByIdAsync(int id);
    }
}
