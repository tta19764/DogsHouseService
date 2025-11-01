🐶 DogsHouseService
Overview

DogsHouseService is a sample REST API built with ASP.NET Core Web API and Entity Framework Core (Code-First) for managing a simple dog database.
It was developed as part of a Junior .NET Developer test task for Codebridge Technology.

The application exposes endpoints to:

Retrieve API version information (/ping).

Query a list of dogs with sorting and pagination.

Add new dogs to the database.

Handle rate limiting and error handling globally.

🧩 Technologies Used

.NET 8 / ASP.NET Core Web API

Entity Framework Core (Code First)

MS SQL Server (LocalDB for development)

xUnit – unit testing

Moq – mocking dependencies in tests

Swagger / OpenAPI – API documentation

Rate Limiting Middleware (built-in ASP.NET Core)

Custom Exception Handling Middleware

Dependency Injection / Repository-Service Pattern

⚙️ Project Structure
```
DogsHouseService/
├── DogsHouseService.WebApi/              # ASP.NET Core Web API project (entry point)
│   ├── Controllers/                      # API controllers (Ping, Dogs)
│   ├── Middlewares/                      # Custom exception and rate-limiting middleware
│   ├── Extensions/                       # Service and middleware registration extensions
│   ├── appsettings.json                  # Application and database configuration
│   ├── Helpers/                          # Mapping helpers (Model ↔ DTO)
│   ├── Logging/                          # Logging delegates
│   ├── Migrations/                       # Database migrations
│   ├── Options/                          # Classes for configuration setting extraction
│   └── Program.cs                        # Application startup entry
│
├── DogsHouseService.WebApi.Models/
│   └── Dtos/                             # Request and response DTOs
│
├── DogsHouseService.Services/            # Business logic layer
│   ├── Interfaces/                       # Service interfaces
│   ├── Enums/                            # Service enums
│   └── Models/                           # Business logic layer models
│
├── DogsHouseService.Services.Database/   # Data access layer (EF Core)
│   ├── Data/                             # DbContext (DogsHouseServiceDbContext)
│   ├── Entities/                         # EF Core entities (Dog)
│   ├── Interfaces/                       # Repository interfaces
│   ├── Helpers/                          # Mapping helpers (Model ↔ Entity)
│   ├── Services/                         # Service implementations
│   └── Repositories/                     # Repository implementations (e.g. DogRepository)
│
└── DogsHouseService.Tests/               # Unit test project (xUnit + Moq)
    ├── DogsControllerTests               # DogsController tests
    ├── DogServiceTests                   # DogService tests
    ├── DogRepositoryTests                # DogRepository tests
    ├── PingControllerTests               # PingController tests
    └── MiddlewareTests                   # Middleware tests
```


🗄️ Database Configuration

The project uses EF Core Code First with migrations.

Default connection string (in appsettings.json):

"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=DogsHouseServiceDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}

Running Migrations
dotnet ef migrations add Initial
dotnet ef database update Initial

🚀 Running the Application
1. Clone and open the project
git clone https://github.com/<your-username>/DogsHouseService.git
cd DogsHouseService

2. Run the API
dotnet run --project DogsHouseService.WebApi


The API will be available at:
👉 https://localhost:7089 or http://localhost:5192

3. Open Swagger UI

Visit
👉 https://localhost:7089/swagger

📡 API Endpoints
🧠 Health Check

GET /ping
Returns the application name and version.

Example
curl -X GET http://localhost:5192/ping


Response

Dogshouseservice.Version1.0.1

🐕 Get All Dogs

GET /dogs

Supports:

Sorting (attribute and order)

Pagination (pageNumber and pageSize)

Example 1 – Get all dogs
curl -X GET http://localhost:5192/dogs

Example 2 – Sorted by weight descending
curl -X GET "http://localhost:5192/dogs?attribute=weight&order=desc"

Example 3 – Paginated
curl -X GET "http://localhost:5192/dogs?pageNumber=2&pageSize=10"


Response

[
  {
    "name": "Neo",
    "color": "red&amber",
    "tail_length": 22,
    "weight": 32
  },
  {
    "name": "Jessy",
    "color": "black&white",
    "tail_length": 7,
    "weight": 14
  }
]

🐾 Create a Dog

POST /dog

Example
curl -X POST http://localhost:5192/dog \
  -H "Content-Type: application/json" \
  -d "{\"name\": \"Doggy\", \"color\":\"red\", \"tail_length\":173, \"weight\":33}"


Validation Rules

Dog name must be unique.

TailLength and Weight must be positive integers.

Invalid JSON or missing fields return 400 Bad Request.

⏱️ Rate Limiting

To prevent overloading the service, requests are limited based on configuration in appsettings.json:

"RateLimiting": {
  "PermitLimit": 10,
  "WindowSeconds": 1,
  "QueueLimit": 0
}


If exceeded, the server responds with:

HTTP 429 Too Many Requests
Too many requests. Please try again later.

🧰 Exception Handling

All unhandled exceptions are processed by ExceptionHandlingMiddleware, which:

Logs the error using ILogger

Returns a structured JSON response:

{
  "error": "Tail length cannot be negative.",
  "statusCode": 400
}

🧪 Unit Tests

The project includes comprehensive xUnit tests covering:

Repositories (in-memory EF Core)

Services (with Moq)

Controllers (mocked dependencies)

Middlewares (exception handling)

Run tests using:

dotnet test

📖 Configuration

All settings are stored in appsettings.json:

"AppSettings": {
  "ApplicationName": "Dogshouseservice",
  "Version": "1.0.1"
},
"RateLimiting": {
  "PermitLimit": 5,
  "WindowSeconds": 1,
  "QueueLimit": 0
}

🧠 Design Patterns Used

Repository Pattern – data access abstraction

Service Layer Pattern – business logic encapsulation

Dependency Injection – clean separation of concerns

DTO Mapping – using ModelToDto helper

Middleware Pattern – for centralized error handling and rate limiting
