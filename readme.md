# Country API — ASP.NET Core Web API

# External API Used

REST Countries API  
https://restcountries.com/v3.1/all

---

# How to Run

## 1. Clone
``` bash
git clone https://github.com/GanishkaWidanapathirana/Assignment.git
cd Assignment
```

## 2. Setup DB
``` bash
Run:
Database/schema.sql
```

## 3. Configure Connection String
Update appsettings.json

#### Configure Connection String

Update appsettings.json:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=countrydb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```
If using SQL Login:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=countrydb;User Id=sa;Password=your_password;TrustServerCertificate=True;"
  }
}
```

## 4. Run
``` bash
dotnet restore
dotnet build
dotnet run
```

---

# API

GET /api/countries  
GET /api/countries/{id}

---

### GET /api/countries

Returns a list of all countries.

First checks the database
If data exists → returns from SQL Server
If database is empty → fetches from external API, saves to DB, then returns data

### GET /api/countries/{id}

Returns a single country by its ID.

Searches in the database
If found → returns the country
If not found → returns 404 Not Found