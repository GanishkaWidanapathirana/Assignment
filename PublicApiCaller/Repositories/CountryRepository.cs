using Microsoft.Data.SqlClient;
using PublicApiCaller.Data;
using PublicApiCaller.DTOs;
using System.Text.Json;

namespace PublicApiCaller.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DbConnectionFactory _factory;

        public CountryRepository(DbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<bool> HasCountriesAsync()
        {
            using var connection = _factory.CreateConnection();

            await connection.OpenAsync();

            var query = "SELECT COUNT(*) FROM countries";

            using var command = new SqlCommand(query, connection);

            var count = (int)await command.ExecuteScalarAsync();

            return count > 0;
        }

        public async Task<List<CountryDto>> GetAllCountriesAsync()
        {
            var countries = new Dictionary<int, CountryDto>();

            using var connection = _factory.CreateConnection();

            await connection.OpenAsync();

            var query = @"
                        SELECT
                        c.id,
                        c.common_name,
                        c.official_name,
                        c.region,
                        c.area,
                        c.population,
                        cp.capital_name,
                        l.name AS language_name,
                        cu.name AS currency_name
                        FROM countries c
                        LEFT JOIN capitals cp ON cp.country_id = c.id
                        LEFT JOIN country_languages cl ON cl.country_id = c.id
                        LEFT JOIN languages l ON l.id = cl.language_id
                        LEFT JOIN country_currencies cc ON cc.country_id = c.id
                        LEFT JOIN currencies cu ON cu.id = cc.currency_id
                        ORDER BY c.id
                        ";

            using var command = new SqlCommand(query, connection);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var id = reader.GetInt32(0);

                if (!countries.ContainsKey(id))
                {
                    countries[id] = new CountryDto
                    {
                        Id = id,
                        CommonName = reader[1]?.ToString() ?? "",
                        OfficialName = reader[2]?.ToString(),
                        Region = reader[3]?.ToString(),
                        Area = Convert.ToDecimal(reader[4]),
                        Population = Convert.ToInt64(reader[5])
                    };
                }

                var capital = reader[6]?.ToString();
                var language = reader[7]?.ToString();
                var currency = reader[8]?.ToString();

                if (!string.IsNullOrEmpty(capital) &&
                    !countries[id].Capitals.Contains(capital))
                {
                    countries[id].Capitals.Add(capital);
                }

                if (!string.IsNullOrEmpty(language) &&
                    !countries[id].Languages.Contains(language))
                {
                    countries[id].Languages.Add(language);
                }

                if (!string.IsNullOrEmpty(currency) &&
                    !countries[id].Currencies.Contains(currency))
                {
                    countries[id].Currencies.Add(currency);
                }
            }

            return countries.Values.ToList();
        }

        public async Task<CountryDto?> GetCountryByIdAsync(int id)
        {
            var countries = await GetAllCountriesAsync();

            return countries.FirstOrDefault(x => x.Id == id);
        }

        public async Task InsertCountryDataAsync(JsonElement country)
        {
            using var connection = _factory.CreateConnection();

            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                var commonName = country.GetProperty("name")
                    .GetProperty("common")
                    .GetString();

                var officialName = country.GetProperty("name")
                    .GetProperty("official")
                    .GetString();

                var region = country.GetProperty("region").GetString();

                var area = country.GetProperty("area").GetDecimal();

                var population = country.GetProperty("population").GetInt64();

                // Insert Country
                var insertCountryQuery = @"
                                            INSERT INTO countries
                                            (common_name, official_name, region, area, population)
                                            OUTPUT INSERTED.id
                                            VALUES
                                            (@common_name, @official_name, @region, @area, @population)
                                            ";

                using var countryCommand =
                    new SqlCommand(insertCountryQuery, connection, transaction);

                countryCommand.Parameters.AddWithValue(
                    "@common_name",
                    commonName ?? ""
                );

                countryCommand.Parameters.AddWithValue(
                    "@official_name",
                    officialName ?? (object)DBNull.Value
                );

                countryCommand.Parameters.AddWithValue(
                    "@region",
                    region ?? (object)DBNull.Value
                );

                countryCommand.Parameters.AddWithValue("@area", area);

                countryCommand.Parameters.AddWithValue(
                    "@population",
                    population
                );

                var countryId =
                    (int)await countryCommand.ExecuteScalarAsync();

                // Insert Capitals
                if (country.TryGetProperty("capital", out var capitals))
                {
                    foreach (var capital in capitals.EnumerateArray())
                    {
                        var insertCapitalQuery = @"
                                                INSERT INTO capitals (country_id, capital_name)
                                                VALUES (@country_id, @capital_name)
                                                ";

                        using var capitalCommand =
                            new SqlCommand(
                                insertCapitalQuery,
                                connection,
                                transaction
                            );

                        capitalCommand.Parameters.AddWithValue(
                            "@country_id",
                            countryId
                        );

                        capitalCommand.Parameters.AddWithValue(
                            "@capital_name",
                            capital.GetString() ?? ""
                        );

                        await capitalCommand.ExecuteNonQueryAsync();
                    }
                }

                // Insert Languages
                if (country.TryGetProperty("languages", out var languages))
                {
                    foreach (var language in languages.EnumerateObject())
                    {
                        var languageCode = language.Name;

                        var languageName = language.Value.GetString();

                        var insertLanguageQuery = @"
                                                    IF NOT EXISTS (
                                                    SELECT * FROM languages WHERE code = @code
                                                    )
                                                    BEGIN
                                                    INSERT INTO languages (code, name)
                                                    VALUES (@code, @name)
                                                    END
                                                    ";

                        using var languageCommand =
                            new SqlCommand(
                                insertLanguageQuery,
                                connection,
                                transaction
                            );

                        languageCommand.Parameters.AddWithValue(
                            "@code",
                            languageCode
                        );

                        languageCommand.Parameters.AddWithValue(
                            "@name",
                            languageName ?? ""
                        );

                        await languageCommand.ExecuteNonQueryAsync();

                        var getLanguageIdQuery =
                            "SELECT id FROM languages WHERE code = @code";

                        using var getLanguageCommand =
                            new SqlCommand(
                                getLanguageIdQuery,
                                connection,
                                transaction
                            );

                        getLanguageCommand.Parameters.AddWithValue(
                            "@code",
                            languageCode
                        );

                        var languageId =(int)await getLanguageCommand.ExecuteScalarAsync();

                        var insertCountryLanguageQuery = @"
                                                        INSERT INTO country_languages
                                                        (country_id, language_id)
                                                        VALUES
                                                        (@country_id, @language_id)
                                                        ";

                        using var countryLanguageCommand =
                            new SqlCommand(
                                insertCountryLanguageQuery,
                                connection,
                                transaction
                            );

                        countryLanguageCommand.Parameters.AddWithValue(
                            "@country_id",
                            countryId
                        );

                        countryLanguageCommand.Parameters.AddWithValue(
                            "@language_id",
                            languageId
                        );

                        await countryLanguageCommand.ExecuteNonQueryAsync();
                    }
                }

                // Insert Currencies
                if (country.TryGetProperty("currencies", out var currencies))
                {
                    foreach (var currency in currencies.EnumerateObject())
                    {
                        var currencyCode = currency.Name;

                        var currencyData = currency.Value;

                        var currencyName =
                            currencyData.GetProperty("name").GetString();

                        string? currencySymbol = null;

                        if (currencyData.TryGetProperty(
                            "symbol",
                            out var symbolProperty))
                        {
                            currencySymbol = symbolProperty.GetString();
                        }

                        var insertCurrencyQuery = @"
                                                    IF NOT EXISTS (
                                                    SELECT * FROM currencies WHERE code = @code
                                                    )
                                                    BEGIN
                                                    INSERT INTO currencies (code, name, symbol)
                                                    VALUES (@code, @name, @symbol)
                                                    END
";

                        using var currencyCommand =
                            new SqlCommand(
                                insertCurrencyQuery,
                                connection,
                                transaction
                            );

                        currencyCommand.Parameters.AddWithValue(
                            "@code",
                            currencyCode
                        );

                        currencyCommand.Parameters.AddWithValue(
                            "@name",
                            currencyName ?? ""
                        );

                        currencyCommand.Parameters.AddWithValue(
                            "@symbol",
                            currencySymbol ?? (object)DBNull.Value
                        );

                        await currencyCommand.ExecuteNonQueryAsync();

                        var getCurrencyIdQuery =
                            "SELECT id FROM currencies WHERE code = @code";

                        using var getCurrencyCommand =
                            new SqlCommand(
                                getCurrencyIdQuery,
                                connection,
                                transaction
                            );

                        getCurrencyCommand.Parameters.AddWithValue(
                            "@code",
                            currencyCode
                        );

                        var currencyId =
                            (int)await getCurrencyCommand.ExecuteScalarAsync();

                        var insertCountryCurrencyQuery = @"
                                                            INSERT INTO country_currencies
                                                            (country_id, currency_id)
                                                            VALUES
                                                            (@country_id, @currency_id)
                                                            ";

                        using var countryCurrencyCommand =
                            new SqlCommand(
                                insertCountryCurrencyQuery,
                                connection,
                                transaction
                            );

                        countryCurrencyCommand.Parameters.AddWithValue(
                            "@country_id",
                            countryId
                        );

                        countryCurrencyCommand.Parameters.AddWithValue(
                            "@currency_id",
                            currencyId
                        );

                        await countryCurrencyCommand.ExecuteNonQueryAsync();
                    }
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
 }
