# Arabia Maintenance Services API

A comprehensive RESTful API built with ASP.NET Core 9.0 for managing maintenance services, businesses, employees, and service requests. This application provides a complete backend solution for a maintenance service management system with JWT-based authentication and role-based authorization.

## Table of Contents

- [Overview](#overview)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Setup Instructions](#setup-instructions)
- [Configuration](#configuration)
- [API Documentation](#api-documentation)
- [Database Schema](#database-schema)
- [Authentication & Authorization](#authentication--authorization)
- [Architecture](#architecture)
- [Testing](#testing)
- [Development](#development)

## Overview

Arabia Maintenance Services API is a backend application designed to manage:
- **Businesses**: Client businesses that request maintenance services
- **Employees**: Service technicians with specializations
- **Services**: Available maintenance service types (Electric, Plumbing, Cleaning, etc.)
- **Service Requests**: Work orders connecting businesses, services, and employees

The API implements a clean architecture pattern with separation of concerns, using repositories for data access, services for business logic, and controllers for HTTP handling.

## Technology Stack

- **.NET 9.0**: Latest .NET framework
- **ASP.NET Core Web API**: RESTful API framework
- **Entity Framework Core 9.0**: ORM for database operations
- **PostgreSQL**: Relational database (via Npgsql)
- **ASP.NET Core Identity**: User authentication and authorization
- **JWT Bearer Authentication**: Token-based authentication
- **AutoMapper 12.0**: Object-to-object mapping
- **Swagger/OpenAPI**: API documentation
- **FluentValidation**: Input validation (configured but not actively used)

## Project Structure

```
arabia/
├── Controllers/              # API Controllers (HTTP endpoints)
│   ├── AuthController.cs
│   ├── BusinessesController.cs
│   ├── EmployeesController.cs
│   ├── ServicesController.cs
│   └── ServiceRequestsController.cs
│
├── DTOs/                    # Data Transfer Objects
│   ├── Requests/            # Request DTOs for API inputs
│   │   ├── CreateBusinessRequest.cs
│   │   ├── CreateEmployeeRequest.cs
│   │   ├── CreateServiceRequest.cs
│   │   ├── CreateServiceRequestRequest.cs
│   │   ├── LoginRequest.cs
│   │   ├── RegisterRequest.cs
│   │   ├── UpdateBusinessRequest.cs
│   │   ├── UpdateEmployeeRequest.cs
│   │   ├── UpdateServiceRequest.cs
│   │   └── UpdateServiceRequestRequest.cs
│   └── Responses/           # Response DTOs for API outputs
│       ├── AuthResponse.cs
│       ├── BusinessResponse.cs
│       ├── EmployeeResponse.cs
│       ├── ServiceRequestResponse.cs
│       ├── ServiceResponse.cs
│       └── UserResponse.cs
│
├── Infrastructure/           # Infrastructure layer
│   └── Persistence/         # Data access layer
│       ├── ApplicationDbContext.cs
│       └── Repositories/    # Repository pattern implementation
│           ├── IRepository.cs
│           ├── Repository.cs
│           ├── IServiceRequestRepository.cs
│           └── ServiceRequestRepository.cs
│
├── Mappings/                # AutoMapper profiles
│   └── MappingProfile.cs
│
├── Middleware/              # Custom middleware
│   ├── AppException.cs
│   ├── ErrorResponse.cs
│   └── ExceptionMiddleware.cs
│
├── Models/                  # Domain models/entities
│   ├── Business.cs
│   ├── Employee.cs
│   ├── Role.cs
│   ├── Service.cs
│   ├── ServiceRequest.cs
│   └── User.cs
│
├── Services/                # Business logic layer
│   ├── Interfaces/         # Service interfaces
│   │   ├── IAuthService.cs
│   │   ├── IBusinessService.cs
│   │   ├── IEmployeeService.cs
│   │   ├── IServiceRequestService.cs
│   │   ├── IServiceService.cs
│   │   └── IUserRepository.cs
│   ├── AuthService.cs
│   ├── BusinessService.cs
│   ├── EmployeeService.cs
│   ├── ServiceRequestService.cs
│   ├── ServiceService.cs
│   └── UserRepository.cs
│
├── Migrations/              # Entity Framework migrations
│   └── [Migration files]
│
├── Tests/                   # Unit tests
│   └── Services/
│       ├── AuthServiceTests.cs
│       ├── BusinessServiceTests.cs
│       ├── EmployeeServiceTests.cs
│       ├── ServiceRequestServiceTests.cs
│       └── ServiceServiceTests.cs
│
├── Properties/
│   └── launchSettings.json
│
├── appsettings.json         # Production configuration
├── appsettings.Development.json  # Development configuration
├── Program.cs               # Application entry point
└── arabia.csproj            # Project file
```

## Features

### Core Features

1. **User Authentication & Authorization**
   - User registration and login
   - JWT token-based authentication
   - Refresh token mechanism
   - Role-based access control (Admin, Manager, Employee, User)

2. **Business Management**
   - Create, read, update, delete businesses
   - Track business contact information and location
   - Filter active/inactive businesses

3. **Employee Management**
   - Manage service technicians
   - Track employee specializations
   - Filter employees by specialization
   - Employee assignment to service requests

4. **Service Management**
   - Manage available service types
   - Set base pricing for services
   - Track service descriptions

5. **Service Request Management**
   - Create service requests
   - Assign employees to requests
   - Track request status (Pending, Assigned, InProgress, Completed, Cancelled)
   - Filter by business, employee, or status
   - Track estimated and actual costs
   - Schedule and completion date tracking

### Security Features

- Password hashing using ASP.NET Core Identity
- JWT token expiration (1 hour)
- Refresh token expiration (7 days)
- CORS configuration for frontend integration
- Role-based authorization policies
- Exception handling middleware

## Prerequisites

- **.NET 9.0 SDK** or later
- **PostgreSQL 12+** (or compatible version)
- **IDE**: Visual Studio 2022, Visual Studio Code, or Rider
- **Git** (for version control)

## Setup Instructions

### 1. Clone the Repository

```bash
git clone <repository-url>
cd arabia
```

### 2. Configure Database

1. Install PostgreSQL if not already installed
2. Create a new database:
   ```sql
   CREATE DATABASE arabia_db;
   ```

3. Update connection string in `appsettings.json` or `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=arabia_db;Username=your_user;Password=your_password;Port=5432"
     }
   }
   ```

### 3. Run Database Migrations

```bash
dotnet ef database update
```

Or using the Package Manager Console in Visual Studio:
```powershell
Update-Database
```

### 4. Configure JWT Settings

Update JWT configuration in `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "ArabiaAPI",
    "Audience": "ArabiaAPIUsers",
    "ExpirationHours": 1
  }
}
```

**Important**: Change the JWT Key to a secure random string in production!

### 5. Run the Application

```bash
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5124`
- HTTPS: `https://localhost:7270` (if configured)

### 6. Access Swagger Documentation

Navigate to: `http://localhost:5124/swagger`

## Configuration

### Connection Strings

The application supports different connection strings for different environments:

- **Development**: `appsettings.Development.json`
- **Production**: `appsettings.json`

### CORS Configuration

Configure allowed origins in `appsettings.json`:

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5173",
      "http://localhost:4200"
    ]
  }
}
```

### Logging

Logging levels can be configured:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

## API Documentation

### Base URL

```
http://localhost:5124/api
```

### Authentication Endpoints

#### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "password": "SecurePass123!",
  "role": "User"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "password": "SecurePass123!"
}
```

#### Refresh Token
```http
POST /api/auth/refresh
Content-Type: application/json

"refresh_token_string"
```

#### Logout
```http
POST /api/auth/logout
Authorization: Bearer {token}
Content-Type: application/json

"refresh_token_string"
```

### Business Endpoints

| Method | Endpoint | Authorization | Description |
|--------|----------|---------------|-------------|
| GET | `/api/businesses` | Anonymous | Get all businesses |
| GET | `/api/businesses/active` | Anonymous | Get active businesses |
| GET | `/api/businesses/{id}` | Anonymous | Get business by ID |
| POST | `/api/businesses` | Manager+ | Create business |
| PUT | `/api/businesses/{id}` | Manager+ | Update business |
| DELETE | `/api/businesses/{id}` | Manager+ | Delete business |

### Employee Endpoints

| Method | Endpoint | Authorization | Description |
|--------|----------|---------------|-------------|
| GET | `/api/employees` | Employee+ | Get all employees |
| GET | `/api/employees/{id}` | Employee+ | Get employee by ID |
| GET | `/api/employees/specialization/{specialization}` | Employee+ | Get employees by specialization |
| POST | `/api/employees` | Manager+ | Create employee |
| PUT | `/api/employees/{id}` | Manager+ | Update employee |
| DELETE | `/api/employees/{id}` | Manager+ | Delete employee |

### Service Endpoints

| Method | Endpoint | Authorization | Description |
|--------|----------|---------------|-------------|
| GET | `/api/services` | Anonymous | Get all services |
| GET | `/api/services/active` | Anonymous | Get active services |
| GET | `/api/services/{id}` | Anonymous | Get service by ID |
| POST | `/api/services` | Manager+ | Create service |
| PUT | `/api/services/{id}` | Manager+ | Update service |
| DELETE | `/api/services/{id}` | Manager+ | Delete service |

### Service Request Endpoints

| Method | Endpoint | Authorization | Description |
|--------|----------|---------------|-------------|
| GET | `/api/servicerequests` | User+ | Get all service requests |
| GET | `/api/servicerequests/{id}` | User+ | Get service request by ID |
| GET | `/api/servicerequests/business/{businessId}` | User+ | Get requests by business |
| GET | `/api/servicerequests/employee/{employeeId}` | User+ | Get requests by employee |
| GET | `/api/servicerequests/status/{status}` | User+ | Get requests by status |
| POST | `/api/servicerequests` | Anonymous | Create service request |
| PUT | `/api/servicerequests/{id}` | User+ | Update service request |
| DELETE | `/api/servicerequests/{id}` | User+ | Delete service request |

## Database Schema

### User (Identity)
- Inherits from `IdentityUser<int>`
- Fields: `Id`, `Email`, `UserName`, `FirstName`, `LastName`, `IsActive`, `CreatedAt`, `UpdatedAt`, `RefreshToken`, `RefreshTokenExpiryTime`
- Relationships: Many-to-many with Roles

### Role (Identity)
- Inherits from `IdentityRole<int>`
- Fields: `Id`, `Name`, `Description`, `CreatedAt`
- Available roles: Admin, Manager, Employee, User

### Business
- Fields: `Id`, `Name`, `Address`, `City`, `State`, `ZipCode`, `ContactEmail`, `ContactPhone`, `ContactPerson`, `IsActive`, `CreatedAt`, `UpdatedAt`
- Relationships: One-to-many with ServiceRequests

### Employee
- Fields: `Id`, `FirstName`, `LastName`, `Email`, `Phone`, `Specialization`, `HireDate`, `IsActive`, `CreatedAt`, `UpdatedAt`
- Relationships: One-to-many with ServiceRequests (nullable)

### Service
- Fields: `Id`, `Name`, `Description`, `BasePrice`, `IsActive`, `CreatedAt`, `UpdatedAt`
- Relationships: One-to-many with ServiceRequests

### ServiceRequest
- Fields: `Id`, `BusinessId`, `ServiceId`, `EmployeeId` (nullable), `Status`, `RequestedDate`, `ScheduledDate`, `CompletedDate`, `Description`, `Notes`, `EstimatedCost`, `ActualCost`, `CreatedAt`, `UpdatedAt`
- Relationships:
  - Many-to-one with Business
  - Many-to-one with Service
  - Many-to-one with Employee (nullable)
- Status values: `Pending`, `Assigned`, `InProgress`, `Completed`, `Cancelled`

## Authentication & Authorization

### Authentication Flow

1. **Registration/Login**: User provides credentials
2. **Token Generation**: Server generates JWT access token (1 hour expiry) and refresh token (7 days expiry)
3. **Token Usage**: Client includes JWT token in `Authorization: Bearer {token}` header
4. **Token Refresh**: Client uses refresh token to obtain new access token
5. **Logout**: Client invalidates refresh token

### Authorization Policies

The application defines four authorization policies:

- **Admin**: Requires Admin role
- **Manager**: Requires Admin or Manager role
- **Employee**: Requires Admin, Manager, or Employee role
- **User**: Requires any authenticated user (Admin, Manager, Employee, or User)

### Role Hierarchy

```
Admin (highest privileges)
  └── Manager
      └── Employee
          └── User (lowest privileges)
```

## Architecture

### Design Patterns

1. **Repository Pattern**: Abstracts data access logic
   - `IRepository<T>`: Generic repository interface
   - `Repository<T>`: Generic repository implementation
   - `IServiceRequestRepository`: Specialized repository for ServiceRequests

2. **Service Layer Pattern**: Encapsulates business logic
   - Services handle business rules and orchestration
   - Controllers delegate to services

3. **DTO Pattern**: Separates API contracts from domain models
   - Request DTOs for input validation
   - Response DTOs for output formatting

4. **Dependency Injection**: All services and repositories are registered in `Program.cs`

### Data Flow

```
HTTP Request
    ↓
Controller (handles HTTP concerns)
    ↓
Service (business logic)
    ↓
Repository (data access)
    ↓
DbContext (Entity Framework)
    ↓
PostgreSQL Database
```

### Middleware Pipeline

1. Exception Middleware (global error handling)
2. HTTPS Redirection (if configured)
3. CORS
4. Authentication
5. Authorization
6. Controllers

## Testing

The project includes a test suite in the `Tests/` directory:

- **AuthServiceTests.cs**: Tests for authentication service
- **BusinessServiceTests.cs**: Tests for business service
- **EmployeeServiceTests.cs**: Tests for employee service
- **ServiceRequestServiceTests.cs**: Tests for service request service
- **ServiceServiceTests.cs**: Tests for service service

### Running Tests

```bash
dotnet test
```

## Development

### Adding a New Migration

```bash
dotnet ef migrations add MigrationName
```

### Updating Database

```bash
dotnet ef database update
```

### Code Style

- Follow C# naming conventions
- Use nullable reference types (enabled)
- Use async/await for all I/O operations
- Use dependency injection for all dependencies

### Key Files to Modify

- **Adding a new entity**: Create model in `Models/`, add DbSet in `ApplicationDbContext.cs`, create migration
- **Adding a new endpoint**: Create controller in `Controllers/`, add service interface and implementation
- **Changing mapping**: Update `MappingProfile.cs`
- **Changing authorization**: Update policies in `Program.cs` or add `[Authorize]` attributes

## Support

For issues and questions, please contact: support@arabia.com

