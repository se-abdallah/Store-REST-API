E-Commerce Store API `https://github.com/se-abdallah/Store-REST-API`
====================

A modular **.NET 8 Web API** implementing a simple E-Commerce backend. This project demonstrates a clean, layered architecture with a focus on separation of concerns and testability.

✨ Features
----------

-   **JWT Authentication**: Secure access for users.

-   **Product Management**: Includes soft delete and multi-language translations (EN/AR).

-   **Invoice Generation**: Automated stock handling and validation.

-   **Seed Data**: Pre-loaded with 20+ products and Admin/Visitor users.

-   **Comprehensive Testing**: Unit tests covering Domain, Application, and Infrastructure layers.

-   **Postman Ready**: Includes collections for quick setup and testing.

<br>

🏗️ Architecture & Project Structure
------------------------------------

The project follows a **Layered / Clean Architecture** style to ensure testable business logic and easier maintenance.

### Directory Tree

```
E-Commerce Store
│  Store.sln
│  README.md
│
├─ src
│  ├─ Store.API             # Presentation: controllers, middleware, DI, AutoMapper profile
│  ├─ Store.Application     # Use cases, DTOs, interfaces, pagination models
│  ├─ Store.Domain          # Entities and core domain models (no infrastructure deps)
│  └─ Store.Infrastructure  # EF Core, repositories, UoW, services, seeding, token service
│
├─ tests
│  ├─ Store.Domain.Tests          # Domain-level tests
│  ├─ Store.Application.Tests     # Application-level tests
│  └─ Store.Infrastructure.Tests  # Infrastructure/service tests (e.g. invoice flow)
│
└─ Docs
   └─ Postman Collections
      ├─ ECommerceStore.postman_API Auth.json
      ├─ ECommerceStore.postman_API Product.json
      └─ ECommerceStore.postman_API Invoice.json

```

### Layer summary

-   **Domain:** `Product`, `ProductTranslation`, `Invoice`, `InvoiceDetail`, `AppUser`, `BaseEntity`.

-   **Application:** DTOs, `IAuthService`, `IProductService`, `IInvoiceService`, `ITokenService`, `IUnitOfWork`, repository interfaces, `BaseParams`, `PagedList<T>`.

-   **Infrastructure:** `AppDbContext`, seeding (`StoreSeed`, `IdentitySeed`), repositories, UoW, concrete services, JWT `TokenService`, pagination extensions.

-   **API:** Controllers (`Account`, `Products`, `Invoices`), error types, `ExceptionMiddleware`, DI extensions, AutoMapper profiles.



<br>


🚀 How to Run the API
---------------------

Follow these steps to get the project running locally.

### 1\. Build the Project

Restore dependencies and build the solution:

```
dotnet restore
dotnet build Store.sln

```

### 2\. Database Setup

Apply migrations and seed the database with initial data:

```
cd src/Store.API
dotnet ef database update

```

### 3\. Run the Application

Start the API:

```
dotnet run

```

> **Note:** On the first run, the app will automatically apply migrations and seed the database with 20+ products and stock.

<br>

### Default Credentials

Use the following accounts to test the system:

| Role        | Email               | Username  | Password      |
| ----------- | ------------------- | --------- | ------------- |
| **Admin**   | `admin@store.com`   | `admin`   | `Admin#123`   |
| **Visitor** | `visitor@store.com` | `visitor` | `Visitor#123` |

<br>

🛠️ How to Test with Postman
---------------------------

The repository includes pre-configured Postman collections located in `Docs/Postman Collections/`.

### Setup Steps

1.  **Import Collections**: Open Postman and import the three JSON files found in the `Docs` folder.

2.  **Configure Environment**: Create a new environment in Postman and set the `baseUrl` variable:

    -   `{{baseUrl}}` → `"http://localhost:5000` `https://localhost:5001"` (Check your console output for the exact port).
      
      <br>
    

### Testing Workflows

#### 1\. Authentication (Auth Collection)

-   Use the **Register/Login** requests.

-   **Login as Admin** or **Visitor** using the credentials above.

-   The collection automatically stores the received JWT in a global `{{token}}` variable for use in other requests.

#### 2\. Products (Product Collection)

-   **Visitor**: List products (supports pagination, search, language filtering).

-   **Admin**: Create, update, soft-delete products, and view admin-specific details.

#### 3\. Invoices (Invoice Collection)

-   **Visitor**: Create invoices, list own invoices (filter/order/paginate).

-   **Admin**: List all system invoices and view details.

🧪 Running Tests
----------------

To run the automated test suite across all layers:

```
dotnet test Store.sln

```

### Coverage Highlights

-   **Store.Domain.Tests**: Verifies basic domain behavior for core entities.

-   **Store.Application.Tests**: Checks mapping and simple application-level logic.

-   **Store.Infrastructure.Tests**: Validates critical flows, such as:

    -   Decreasing product stock upon invoice creation.

    -   Ensuring invoice creation fails if requested quantity exceeds stock.














    
