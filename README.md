# E-Wallet Application

A simple E-Wallet application built with **ASP.NET Core Web API**, **ASP.NET Core MVC (Razor Views)**, and **SQL Server** using **ADO.NET**.

This project was developed as part of the Weekly 5 assignment to practice Object-Oriented Programming, layered architecture, database relationships, REST API development, and MVC web application development.

## Features

### User Management
- View all registered users.
- View user details.
- Create a new user with an automatically generated wallet.
- Update user information.

### Wallet Management
- View wallet information and current balance.
- Top up wallet balance.
- Transfer funds between users.
- Validate transaction amounts and wallet balances.

### Transaction History
- View transaction history.
- Filter transactions by:
  - All transactions
  - Income
  - Expense
- Display transaction amounts in Indonesian Rupiah format.

## Technology Stack

- **Language:** C#
- **Framework:** .NET 10
- **Backend:** ASP.NET Core Web API
- **Frontend:** ASP.NET Core MVC with Razor Views
- **Database:** Microsoft SQL Server
- **Data Access:** ADO.NET
- **API Documentation:** Swagger / Swashbuckle
- **Database Driver:** Microsoft.Data.SqlClient

## Project Architecture

The solution uses a layered architecture with three main projects:

```text
weekly5/
├── src/
│   ├── EWallet.Api/
│   │   ├── Controllers/
│   │   ├── Handlers/
│   │   └── Program.cs
│   │
│   ├── EWallet.Core/
│   │   ├── Data/
│   │   ├── DTOs/
│   │   ├── Enums/
│   │   ├── Exceptions/
│   │   ├── Interfaces/
│   │   ├── Models/
│   │   ├── Repositories/
│   │   ├── Services/
│   │   └── database/
│   │
│   └── EWallet.Mvc/
│       ├── Controllers/
│       ├── Models/
│       ├── ViewModels/
│       ├── Views/
│       └── wwwroot/
│
└── EWallet.slnx
```

### Architecture Overview

```text
              ┌─────────────────┐
              │   EWallet.Api   │
              │   REST API      │
              └────────┬────────┘
                       │
                       ▼
              ┌─────────────────┐
              │   EWallet.Core  │
              │                 │
              │    Services     │
              │    Repositories │
              │    Models       │
              │    Interfaces   │
              └────────┬────────┘
                       │
                       ▼
              ┌─────────────────┐
              │   SQL Server    │
              │    EWalletDb    │
              └─────────────────┘

              ┌─────────────────┐
              │   EWallet.Mvc   │
              │  Razor Views    │
              └────────┬────────┘
                       │
                       └──────► EWallet.Core
```

Both API and MVC applications use the same Core project for business logic and database operations. They connect directly to the shared Core layer rather than MVC calling the API.

## Database Design

The application uses three main tables:

### Users
Stores user information.

- Id (Primary Key)
- Name
- Email (Unique)
- Phone

### Wallets
Stores wallet information.

- Id (Primary Key)
- UserId (Foreign Key, Unique)
- WalletNumber (Unique)
- Balance

Relationship: One User has one Wallet (1:1).

### Transactions
Stores wallet transaction records.

- Id (Primary Key)
- SenderWalletId (Foreign Key, Nullable)
- ReceiverWalletId (Foreign Key)
- Type
- Amount
- Description
- CreatedAt

Relationship: A Wallet can participate in many Transactions, either as sender or receiver (1:N).

### Transaction Types

| Value | Type | Description |
|---|---|---|
| 1 | TopUp | Wallet balance top up |
| 2 | Transfer | Transfer between wallets |

For top-up transactions, `SenderWalletId` is NULL because the funds originate externally.

## Getting Started

### Prerequisites

Make sure you have installed:

- .NET 10 SDK
- Microsoft SQL Server
- SQL Server Management Studio (SSMS) or another SQL client
- Visual Studio or Visual Studio Code (optional)

### 1. Clone the Repository

```bash
git clone <repository-url>
cd weekly5
```

### 2. Create the Database

Open SQL Server Management Studio and execute the following scripts in order:

1. `src/EWallet.Core/database/01_create_database.sql`
2. `src/EWallet.Core/database/02_create_table.sql`
3. `src/EWallet.Core/database/03_seed_data.sql`

The seed script inserts three sample users with their wallets and initial transaction records.

**Note:** Execute the seed script only once for a newly created database.

### 3. Configure the Connection String

Update the `DefaultConnection` value in:

- `src/EWallet.Api/appsettings.json`
- `src/EWallet.Mvc/appsettings.json`

Example using Windows Authentication:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EWalletDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Change the server name and authentication settings according to your SQL Server configuration.

### 4. Run the REST API

Open a terminal at the solution root:

```bash
dotnet run --project src/EWallet.Api
```

API base URL:

`http://localhost:5224`

Swagger UI:

`http://localhost:5224/swagger`

### 5. Run the MVC Application

Open another terminal at the solution root:

```bash
dotnet run --project src/EWallet.Mvc
```

MVC application:

`http://localhost:5096`

The API and MVC applications run independently but share the same database and Core business logic.

## API Endpoints

Base URL: `http://localhost:5224`

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/users` | Get all users |
| GET | `/api/users/{id}` | Get user by ID |
| POST | `/api/users` | Create user and wallet |
| PUT | `/api/users/{id}` | Update user |
| GET | `/api/wallets/{userId}` | Get wallet by user ID |
| POST | `/api/wallets/{userId}/topups` | Top up wallet |
| POST | `/api/transfers` | Transfer funds |
| GET | `/api/users/{userId}/transactions` | Get transaction history |

For request and response details, run the API and open Swagger UI.

## Business Logic and Validation

- Top-up and transfer amounts must be greater than zero.
- Users cannot transfer funds to their own wallet.
- Transfers require sufficient sender balance.
- Sender balance, receiver balance, and transaction records are updated within a database transaction.
- Database transactions are committed on success and rolled back on failure.
- User creation and wallet creation are handled within one database transaction.
- Transaction descriptions are optional.

## Sample Users

The database seed script provides the following sample accounts:

| Name | Email | Wallet Number | Initial Balance |
|---|---|---|---:|
| Andi | andi@gmail.com | WLT001 | Rp400,000 |
| Budi | budi@gmail.com | WLT002 | Rp400,000 |
| Citra | citra@gmail.com | WLT003 | Rp750,000 |

## Learning Objectives

This project demonstrates practical implementation of:

- Object-Oriented Programming (OOP)
- Classes, Objects, and Constructors
- Encapsulation
- Inheritance, Abstraction, and Polymorphism
- Interfaces
- Collections and Generics
- Layered Architecture
- Repository and Service patterns
- RESTful API development
- ADO.NET database operations
- SQL Server relationships and constraints
- Database transactions and rollback handling
- ASP.NET Core MVC and Razor Views

## License

This project was created for educational purposes.