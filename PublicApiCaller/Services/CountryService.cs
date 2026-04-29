using PublicApiCaller.DTOs;
using PublicApiCaller.Repositories;

namespace PublicApiCaller.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _repository;

        private readonly IRestCountriesApiService _apiService;

        private readonly ILogger<CountryService> _logger;

        public CountryService(
            ICountryRepository repository,
            IRestCountriesApiService apiService,
            ILogger<CountryService> logger)
        {
            _repository = repository;
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<List<CountryDto>> GetCountriesAsync()
        {
            try
            {
                var hasCountries =
                    await _repository.HasCountriesAsync();

                if (!hasCountries)
                {
                    _logger.LogInformation(
                        "Database empty. Fetching countries from external API."
                    );

                    var countriesJson =
                        await _apiService.GetCountriesAsync();

                    foreach (var country in countriesJson
                        .RootElement
                        .EnumerateArray())
                    {
                        await _repository.InsertCountryDataAsync(country);
                    }

                    _logger.LogInformation(
                        "Countries inserted successfully."
                    );
                }

                return await _repository.GetAllCountriesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while getting countries."
                );

                throw;
            }
        }

        public async Task<CountryDto?> GetCountryByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException(
                        "Invalid country ID."
                    );
                }

                var hasCountries =
                    await _repository.HasCountriesAsync();

                if (!hasCountries)
                {
                    await GetCountriesAsync();
                }

                return await _repository.GetCountryByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while getting country by ID."
                );

                throw;
            }
        }
    }
}
