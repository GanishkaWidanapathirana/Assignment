using PublicApiCaller.Data;
using PublicApiCaller.Repositories;
using PublicApiCaller.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<DbConnectionFactory>();

builder.Services.AddScoped<ICountryRepository, CountryRepository>();

builder.Services.AddScoped<ICountryService, CountryService>();

builder.Services.AddHttpClient<IRestCountriesApiService, RestCountriesApiService>(client =>
{
    client.BaseAddress = new Uri("https://restcountries.com/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
