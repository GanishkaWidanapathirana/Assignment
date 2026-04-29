using Microsoft.AspNetCore.Mvc;
using PublicApiCaller.Services;

namespace PublicApiCaller.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryService _service;

        private readonly ILogger<CountriesController> _logger;

        public CountriesController(
            ICountryService service,
            ILogger<CountriesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            try
            {
                var countries =
                    await _service.GetCountriesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Countries retrieved successfully.",
                    data = countries
                });
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, ex.Message);

                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error.");

                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error."
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCountry(int id)
        {
            try
            {
                var country =
                    await _service.GetCountryByIdAsync(id);

                if (country == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Country not found."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Country retrieved successfully.",
                    data = country
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, ex.Message);

                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error.");

                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error."
                });
            }
        }
    }
}
