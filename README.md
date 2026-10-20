# Order Management API

A RESTful Order Management API built with **ASP.NET Core Web API, C#, Dapper, and SQL Server**. The application manages products, orders, order items, inventory, and order status updates using a layered architecture.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Core Workflows](#core-workflows)
- [Authentication and Authorization](#authentication-and-authorization)
- [Database Design](#database-design)
- [API Endpoints](#api-endpoints)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Error Handling](#error-handling)
- [Testing](#testing)
- [Future Improvements](#future-improvements)

## Overview

The Order Management API provides endpoints for managing products and processing customer orders. It follows a layered architecture to separate HTTP handling, application business logic, domain entities, and database access.

The application uses **Dapper** for lightweight data access and SQL Server stored procedures for database operations.

Order creation is handled within a database transaction to maintain data consistency across order records, order items, and inventory updates.

## Features

### Product Management

- Retrieve product information.
- Manage product data through the product API.
- Validate product availability before order creation.
- Track inventory quantities.

### Order Management

- Create orders containing multiple items.
- Associate orders with authenticated users.
- Calculate order totals using database product prices.
- Store order items and their purchase-time unit prices.
- Retrieve orders and their associated items.
- Update order status and expected delivery dates.
- Validate order status transitions through the application service.

### Security

- JWT-based authentication.
- Role-based authorization.
- Admin-only access to administrative operations.
- Derive the customer identity from the authenticated user's claims rather than trusting a client-supplied user ID.

### Data Consistency

- Execute order creation inside a SQL Server transaction.
- Roll back database changes when an operation fails.
- Validate requested quantities and product stock.
- Update inventory using database-side stock checks.

## Technology Stack

| Technology                | Purpose                            |
| ------------------------- | ---------------------------------- |
| C#                        | Application programming language   |
| ASP.NET Core Web API      | REST API development               |
| .NET                      | Application runtime                |
| Dapper                    | Database access and object mapping |
| Microsoft SQL Server      | Relational database                |
| SQL Stored Procedures     | Database operations                |
| JWT Bearer Authentication | Authentication                     |
| Role-Based Authorization  | Access control                     |
| xUnit                     | Unit testing                       |

## Architecture

The project uses a layered architecture inspired by Clean Architecture principles.

```text
Client
  |
  v
OrderManagement.WebAPI
  |-- Controllers
  |-- Authentication and Authorization
  |-- Request and Response DTOs
  |
  v
OrderManagement.Application
  |-- Services
  |-- Service Contracts
  |-- Repository Contracts
  |-- Business Validation
  |
  v
OrderManagement.Infrastructure
  |-- Dapper Repositories
  |-- SQL Connection Factory
  |-- Stored Procedure Execution
  |
  v
SQL Server Database

OrderManagement.Domain
  |-- Entities
  |-- Core Business Concepts
```

### Layer Responsibilities

**WebAPI Layer**

Handles HTTP requests, model validation, authentication, authorization, and HTTP responses.

**Application Layer**

Coordinates use cases and enforces application business rules. For example, `OrderService` validates order quantities, product availability, and permitted order status transitions.

**Domain Layer**

Contains core entities such as `Order`, `OrderItem`, and `Product`.

**Infrastructure Layer**

Implements repository contracts and handles database access through Dapper, SQL Server connections, and stored procedures.

### Dependency Direction

The Application layer depends on abstractions such as `IOrderRepository`, rather than directly depending on a concrete SQL repository. Infrastructure implements those abstractions.

This separation makes business logic easier to test and reduces coupling between application rules and database implementation.

## Project Structure

The following is a representative structure. Adjust directory names to match the actual solution.

```text
OrderManagement/
|
|-- OrderManagement.WebAPI/
|   |-- Controllers/
|   |-- DTOs/
|   |-- Middleware/
|   |-- Program.cs
|   |-- appsettings.json
|
|-- OrderManagement.Application/
|   |-- Interface/
|   |   |-- RepositoryContracts/
|   |   |-- ServiceContracts/
|   |-- Services/
|
|-- OrderManagement.Domain/
|   |-- Entities/
|
|-- OrderManagement.Infrastructure/
|   |-- ConnectionFactory/
|   |-- Repository/
|   |-- StoredProcedures/
|
|-- OrderManagement.Tests/
|
|-- README.md
```

## Core Workflows

### 1. Create an Order

1. The authenticated customer submits an order request.
2. The API obtains the user's ID from the validated JWT claims.
3. The application service validates the request and quantities.
4. The service retrieves the requested products and checks their availability.
5. Product prices are read from the database, and the total is calculated on the server.
6. The repository begins a SQL Server transaction.
7. The repository inserts the order and its order items.
8. Inventory is reduced using database-side stock validation.
9. The transaction commits if all operations succeed; otherwise, it rolls back.

### 2. Retrieve Orders

1. The controller receives the request.
2. The repository retrieves order records.
3. Related order items are retrieved and associated with their respective orders.
4. The API returns the resulting data.

Dapper maps query results to objects but does not automatically populate navigation properties. Related records must be queried and mapped explicitly.

### 3. Update Order Status

1. An authorized administrator sends a `PUT` request containing the order ID, new status, and expected delivery date.
2. The API verifies that the user has the Admin role.
3. The application service checks whether the requested status transition is valid.
4. The repository calls the relevant stored procedure.
5. The API returns an appropriate HTTP response.

Example status-transition policy:

| Current Status | Permitted Next Status |
| -------------- | --------------------- |
| Pending        | Confirmed, Cancelled  |
| Confirmed      | Processing, Cancelled |
| Processing     | Shipped               |
| Shipped        | Delivered             |
| Delivered      | No further transition |
| Cancelled      | No further transition |

This is an example business policy and should match the application's actual requirements.

## Authentication and Authorization

Protected endpoints require a valid JWT bearer token.

Include the token in the HTTP request header:

```http
Authorization: Bearer <your-jwt-token>
```

Administrative endpoints should enforce role-based authorization:

```csharp
[Authorize(Roles = "Admin")]
```

The role claim in the JWT must match the role configuration used by ASP.NET Core authorization.

The application should not trust a user ID supplied in the request body when determining who owns an order.

## Database Design

The core entities are:

### Users

Stores registered user and authentication information.

Typical fields include:

- `UserId`
- `Email`
- `PasswordHash`
- `Role`

Actual fields depend on the implemented schema.

### Products

Stores product details and inventory information.

Typical fields include:

- `ProductId`
- `Name`
- `Description`
- `Price`
- `StockQuantity`
- `CreatedAt`
- `UpdatedAt`

### Orders

Stores the main order record.

Typical fields include:

- `OrderId`
- `UserID`
- `Status`
- `TotalAmount`
- `ShippingAddress`
- `ExpectedDeliveryDate`
- `CreatedAt`
- `UpdatedAt`

### OrderItems

Stores individual products belonging to an order.

Typical fields include:

- `OrderItemId`
- `OrderId`
- `ProductId`
- `Quantity`
- `UnitPrice`

Each order can contain multiple order items. Each order item references a product and its parent order.

**Important:** The final schema and column names should match the SQL Server database and stored procedures used by the application.

## API Endpoints

The following table describes the intended endpoint design. Confirm each route and HTTP method against the actual controllers before publishing the documentation.

| Method | Endpoint                       | Purpose                                        |
| ------ | ------------------------------ | ---------------------------------------------- |
| GET    | `/api/products`                | Retrieve products                              |
| GET    | `/api/products/{id}`           | Retrieve a product by ID                       |
| POST   | `/api/products`                | Create a product, if implemented               |
| PUT    | `/api/products/{id}`           | Update a product, if implemented               |
| DELETE | `/api/products/{id}`           | Delete a product, if implemented               |
| POST   | `/api/orders`                  | Create an order                                |
| GET    | `/api/orders`                  | Retrieve orders                                |
| PUT    | `/api/orders/{orderId}/status` | Update order status and expected delivery date |

Administrative access should be applied to the appropriate product-management and order-status endpoints according to the application's authorization policy.

## Getting Started

### Prerequisites

Install the following:

- .NET SDK compatible with the project.
- Microsoft SQL Server.
- SQL Server Management Studio or another SQL client.
- Visual Studio or Visual Studio Code.

### 1. Clone the Repository

```bash
git clone (https://github.com/Saqeeb-Khan-Repo/OrderManagement.WebAPI)
cd OrderManagement
```

### 2. Configure the Database

Create the required database in SQL Server.

Execute the project's SQL scripts to create the required tables and stored procedures, including the procedures used for product management, order creation, order item insertion, inventory updates, order retrieval, and order status updates.

Ensure the database schema matches the application's entity and repository mappings.

### 3. Configure the Connection String

Add the connection string to the API project's local configuration.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=OrderManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Replace `YOUR_SERVER` with the correct SQL Server instance name.

For SQL Server authentication, use an appropriate connection string and store credentials securely. Do not commit passwords, production credentials, or JWT signing keys to source control.

### 4. Configure JWT Authentication

Configure the JWT issuer, audience, signing key, and token lifetime according to the application's authentication implementation.

Keep signing keys in user secrets, environment variables, or another secure configuration provider.

Ensure the authentication middleware and authorization middleware are registered in the correct order.

### 5. Build the Solution

From the solution directory, run:

```bash
dotnet restore
dotnet build
```

### 6. Run the API

Navigate to the WebAPI project directory and execute:

```bash
dotnet run
```

Use the API URL displayed in the terminal. If Swagger is configured, open its URL to explore and test the available endpoints.

## Configuration

The application requires appropriate configuration for:

- SQL Server connection settings.
- JWT authentication and token validation.
- Dependency injection for services, repositories, and the connection factory.
- Logging and exception handling.
- Environment-specific application settings.

Do not commit production secrets or sensitive connection details.

## Error Handling

The API should return suitable HTTP status codes for common outcomes.

| Status Code               | Meaning                                                                      |
| ------------------------- | ---------------------------------------------------------------------------- |
| 200 OK                    | Request completed successfully                                               |
| 201 Created               | Resource created successfully                                                |
| 400 Bad Request           | Invalid request or validation failure                                        |
| 401 Unauthorized          | Missing or invalid authentication                                            |
| 403 Forbidden             | Authenticated user lacks the required permission                             |
| 404 Not Found             | Requested resource does not exist                                            |
| 409 Conflict              | Business conflict, such as insufficient stock or an invalid state transition |
| 500 Internal Server Error | Unexpected server-side failure                                               |

The API should use consistent error responses and avoid exposing internal exception details to clients.

## Testing

The application can be tested at different levels.

### Unit Testing

Use xUnit to test application business rules independently of SQL Server.

Suggested test cases:

- Reject an order containing no items.
- Reject a quantity less than one.
- Reject duplicate product IDs if duplicates are prohibited.
- Reject a product that does not exist.
- Reject an order when stock is insufficient.
- Calculate the total using database product prices.
- Reject invalid order status transitions.
- Reject updates for orders that do not exist.

### Integration Testing

Verify interactions between the API, SQL Server, Dapper repositories, and stored procedures.

Suggested scenarios:

- Create an order with multiple items.
- Confirm order items are saved correctly.
- Confirm inventory is reduced after successful order creation.
- Confirm all database changes roll back when order creation fails.
- Verify JWT authentication and Admin-only authorization.
- Verify the status and expected delivery date update correctly.

## Future Improvements

Potential enhancements include:

- Add pagination, filtering, and sorting to order retrieval.
- Introduce consistent API response models.
- Add structured logging and centralized exception handling.
- Expand unit and integration test coverage.
- Improve inventory concurrency handling.
- Add order history and status-change auditing.
- Add API versioning and production deployment configuration.

## License BY
AUTHOR : Saqeeb khan 
