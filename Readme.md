# Asset Management System

A Blazor-based .NET application to manage assets, assignments, and employees.

## Features
- Asset tracking
- Employee assignment
- Return management
- History with sorting

## Tech Stack
- .NET 8
- Blazor Server
- SQL Server
- Dapper ORM (for some queries)
- Entity Framework Core (for migrations / schema management)

---

## 🚀 Setup Instructions
dotnet restore
dotnet tool install --global dotnet-ef
dotnet ef migrations add <MigrationName>
dotnet ef database update

Login with the credentials:
set up in the appsettings.json file

### 1. Clone the repository
```bash
git clone https://github.com/yourusername/AssetManagementSystem.git
cd AssetManagementSystem
