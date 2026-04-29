--Create Database 
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'countrydb')
BEGIN
    CREATE DATABASE countrydb;
END
GO

USE countrydb;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'countries')
BEGIN
    CREATE TABLE countries (
    id INT PRIMARY KEY IDENTITY(1,1),    
    common_name VARCHAR(255) NOT NULL,
    official_name VARCHAR(255),
    region VARCHAR(100),
    area DECIMAL(15,2),
    population BIGINT,
    created_at DATETIME DEFAULT GETDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'country_native_names')
BEGIN
    CREATE TABLE country_native_names (
    id INT PRIMARY KEY IDENTITY(1,1),

    country_id INT NOT NULL,

    language_code VARCHAR(10),

    official_name VARCHAR(255),

    common_name VARCHAR(255),

    FOREIGN KEY (country_id) REFERENCES countries(id)
        ON DELETE CASCADE
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'currencies')
BEGIN
    CREATE TABLE currencies (
    id INT PRIMARY KEY IDENTITY(1,1),

    code VARCHAR(10) UNIQUE NOT NULL,

    name VARCHAR(255),

    symbol VARCHAR(50)
    );
END
GO


IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'country_currencies')
BEGIN
    CREATE TABLE country_currencies (
    country_id INT NOT NULL,
    currency_id INT NOT NULL,

    PRIMARY KEY (country_id, currency_id),

    FOREIGN KEY (country_id) REFERENCES countries(id)
        ON DELETE CASCADE,

    FOREIGN KEY (currency_id) REFERENCES currencies(id)
        ON DELETE CASCADE
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'languages')
BEGIN
    CREATE TABLE languages (
    id INT PRIMARY KEY IDENTITY(1,1),

    code VARCHAR(10) UNIQUE NOT NULL,

    name VARCHAR(255) NOT NULL
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'country_languages')
BEGIN
    CREATE TABLE country_languages (
    country_id INT NOT NULL,
    language_id INT NOT NULL,

    PRIMARY KEY (country_id, language_id),

    FOREIGN KEY (country_id) REFERENCES countries(id)
        ON DELETE CASCADE,

    FOREIGN KEY (language_id) REFERENCES languages(id)
        ON DELETE CASCADE
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'capitals')
BEGIN
    CREATE TABLE capitals (
    id INT PRIMARY KEY IDENTITY(1,1),

    country_id INT NOT NULL,

    capital_name VARCHAR(255),

    FOREIGN KEY (country_id) REFERENCES countries(id)
        ON DELETE CASCADE
    );
END
GO




