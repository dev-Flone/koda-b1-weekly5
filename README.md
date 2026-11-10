# E-Wallet Application

A console-based e-wallet application evolved into a web application using **ASP.NET Core Web API, Razor MVC, and SQL Server**. This project demonstrates Object-Oriented Programming (OOP), layered architecture, database management, and transaction processing using ADO.NET.

## Tech Stack

- **Language:** C#
- **Framework:** .NET 10
- **Backend:** ASP.NET Core Web API
- **Frontend:** ASP.NET Core Razor MVC
- **Database:** Microsoft SQL Server
- **Data Access:** ADO.NET (`Microsoft.Data.SqlClient`)
- **API Documentation:** Swagger / Swashbuckle
- **Architecture:** Layered Architecture

## Project Structure

```text
weekly5/
├── src/
│   ├── EWallet.Api/
│   │   ├── Controllers/
│   │   ├── Handlers/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   ├── EWallet.Core/
│   │   ├── Data/
│   │   ├── database/
│   │   ├── DTOs/
│   │   ├── Enums/
│   │   ├── Exceptions/
│   │   ├── Interfaces/
│   │   ├── Models/
│   │   ├── Repositories/
│   │   └── Services/
│   │
│   └── EWallet.Mvc/
│       ├── Controllers/
│       ├── Models/
│       ├── ViewModels/
│       ├── Views/
│       ├── wwwroot/
│       └── Program.cs
│
├── EWallet.slnx
└── README.md
```

### Project Responsibilities

| Project | Responsibility |
|---|---|
| EWallet.Api | Provides REST API endpoints |
| EWallet.Mvc | Provides the web interface using Razor MVC |
| EWallet.Core | Contains models, business logic, repositories, interfaces, and database access |

Both API and MVC reference `EWallet.Core` directly and share the same business logic and database.

## Architecture

```text
              SQL Server
                   |
             EWallet.Core
          /                \
   EWallet.Api          EWallet.Mvc
       |                     |
   REST API              Razor Views
       |                     |
   API Clients          Web Browser
```

The application uses a layered approach:

1. **Models** — Represent application entities such as User, Wallet, and Transaction.
2. **DTOs** — Define request and response data structures for the API.
3. **Interfaces** — Define contracts for repositories and services.
4. **Repositories** — Handle database operations using ADO.NET.
5. **Services** — Handle business logic and transaction processing.
6. **Controllers** — Handle HTTP requests and MVC page interactions.

## Database Design

Database name: `EWalletDb`

### Entity Relationship

```text
Users
  |
  | 1 : 1
  |
Wallets
  |
  | 1 : N (Sender)
  |
Transactions
  |
  | N : 1 (Receiver)
  |
Wallets
```

### Tables

**Users**

| Column | Type | Description |
|---|---|---|
| Id | INT | Primary key |
| Name | NVARCHAR(100) | User name |
| Email | NVARCHAR(150) | Unique email |
| Phone | NVARCHAR(20) | Phone number |

**Wallets**

| Column | Type | Description |
|---|---|---|
| Id | INT | Primary key |
| UserId | INT | Unique foreign key to Users |
| WalletNumber | NVARCHAR(20) | Unique wallet number |
| Balance | DECIMAL(18,2) | Wallet balance |

**Transactions**

| Column | Type | Description |
|---|---|---|
| Id | INT | Primary key |
| SenderWalletId | INT | Nullable foreign key to Wallets |
| ReceiverWalletId | INT | Foreign key to Wallets |
| Type | INT | Transaction type |
| Amount | DECIMAL(18,2) | Transaction amount |
| Description | NVARCHAR(250) | Optional description |
| CreatedAt | DATETIME2 | Transaction timestamp |

Transaction types:

- `1` — Top Up
- `2` — Transfer

Top Up transactions have a NULL `SenderWalletId`, while Transfer transactions contain both sender and receiver wallet IDs.

## Features

### User Management
- View all registered users.
- View individual user information.
- Create users with an automatically generated wallet.
- Update user information.

### Wallet Management
- View wallet information.
- Check wallet balance.
- Top up wallet balance.
- Validate top-up amounts.

### Money Transfer
- Transfer funds between users.
- Validate sender and recipient wallets.
- Prevent transfers to the same wallet.
- Validate sufficient balance.
- Record transfer history.
- Use SQL transactions to ensure debit, credit, and transaction recording are committed or rolled back together.

### Transaction History
- View transaction history.
- Filter transactions by:
  - All
  - Income
  - Expense
- Display localized currency amounts.

## REST API Endpoints

Base URL: `http://localhost:5224`

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/users` | Get all users |
| GET | `/api/users/{id}` | Get user by ID |
| POST | `/api/users` | Create a user and wallet |
| PUT | `/api/users/{id}` | Update user information |
| GET | `/api/wallets/{userId}` | Get wallet by user ID |
| POST | `/api/wallets/{userId}/topups` | Top up wallet |
| POST | `/api/transfers` | Transfer funds |
| GET | `/api/users/{userId}/transactions` | Get user transaction history |

### Swagger

API documentation is available at:

`http://localhost:5224/swagger`

## Object-Oriented Programming

This project applies OOP concepts through:

- **Class and Object:** User, Wallet, Transaction, and service classes.
- **Constructor:** Used for initializing objects and injecting dependencies.
- **Encapsulation:** Models and business logic manage their respective data and operations.
- **Abstraction:** Interfaces define contracts for repositories and services.
- **Inheritance:** Classes reuse behavior through inheritance where appropriate.
- **Polymorphism:** Interface implementations are resolved through dependency injection.
- **Interface:** Repository and service contracts separate implementation from usage.

## Database Setup

Make sure Microsoft SQL Server is installed and running.

The database scripts are located in:

`src/EWallet.Core/database/`

Execute the scripts in the following order using SQL Server Management Studio (SSMS):

1. `01_create_database.sql`
2. `02_create_table.sql`
3. `03_seed_data.sql`

The `04_migrate_transactions.sql` script is intended for updating an older database schema when necessary. It is not required when setting up a fresh database using the updated schema.

The seed script inserts three dummy users:

- Andi
- Budi
- Citra

**Important:** Run the seed script only once for a fresh database to avoid duplicate data.

The `file.sql` file in `EWallet.Api` is a development workspace for additional database changes and maintenance.

## Configuration

Both API and MVC projects use the same database connection configuration.

Update the `ConnectionStrings:DefaultConnection` value in each project's `appsettings.json` if your SQL Server configuration differs.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EWalletDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Adjust the server name and authentication settings according to your environment.

## Running the Application

Open a terminal in the solution root directory.

### 1. Build the Solution

```powershell
dotnet build EWallet.slnx
```

### 2. Run the Web API

Open the first terminal:

```powershell
dotnet run --project src/EWallet.Api
```

API URL:

`http://localhost:5224`

Swagger URL:

`http://localhost:5224/swagger`

### 3. Run the MVC Application

Open a second terminal:

```powershell
dotnet run --project src/EWallet.Mvc
```

MVC URL:

`http://localhost:5096`

Keep both applications running while testing their respective features.

## Notes

- This project uses ADO.NET for database access.
- Entity Framework Core is not used.
- API and MVC applications share the same Core services and repositories.
- Database transactions are used to maintain consistency during money transfers.

---

**Project:** E-Wallet Application  
**Framework:** .NET 10  
**Database:** SQL Server  
**Learning Focus:** C#, OOP, ASP.NET Core Web API, Razor MVC, ADO.NET, and SQL Server.
