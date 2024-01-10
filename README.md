# Finance Web API (Under Development)

## Welcome to the Finance Web API – your solution for managing expenses, incomes, goals, budgets, and categories! This API is designed to simplify financial tracking and management.


## Key points

* Expense Management: Add, retrieve, update, and delete expenses.
* Income Tracking: Record and manage different sources of income.
* Goal Setting: Set financial goals and track progress.
* Budget Planning: Create and manage budgets for better financial planning.
* Category Organization: Categorize expenses and incomes for easy analysis.


## Installation
To contribute, test or use the API, clone this repository and install the necessary dependencies:
 
* Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.0)
* Microsoft.EntityFrameworkCore (8.0.0)
* Microsoft.EntityFrameworkCore.Design (8.0.0)
* Microsoft.EntityFrameworkCore.SqlServer (8.0.0)
* Microsoft.EntityFrameworkCore.Tools (8.0.0)
* Swashbuckle.AspNetCore (6.4.0)
* Swashbuckle.AspNetCore.Filters (7.0.12)


## Configuration Files

### 1. `appsettings.json`

The `appsettings.json` file is the main configuration file for the Finance Web API. It contains general settings and configurations applicable across all environments. Key features include:

- Database connection strings
- API key configurations
- Logging settings

Please ensure to update this file with the appropriate values for your environment.

**Example `appsettings.json`:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your_database_connection_string"
  },
  "ApiKeys": {
    "SomeServiceApiKey": "your_api_key_here"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "System": "Error"
    }
  }
  // Other configurations...
}
```


### 2. `appsettings.Development.json`

The `appsettings.Development.json` file is used for development specific configurations. It overrides or extends settings in the main `appsettings.json` file when the application is running in the development environment.

**Example `appsettings.Development.json`:**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  },
  // Other development-specific configurations...
}
```


### 3. `launchSettings.json` (Visual Studio Only)

The `launchSettings.json` file is used to configure the behavior of the Visual Studio debugger and other runtime behaviors during development. 

**Example (For swagger) `launchSettings.json`:**

```json
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:7190;http://localhost:5015",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
```


## Api documentation expectation 2024-01-02 - 2024-29-02