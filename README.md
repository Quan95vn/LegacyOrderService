# LegacyOrderService (Refactored)

A modernized .NET 8 Console Application simulating an Order Processing System.
This project demonstrates the transformation of a legacy, monolithic codebase into a scalable, testable, and robust N-Layer architecture.

## 🚀 Features

* **Modern Architecture:** N-Layer Design (Service, Repository, Models).
* **Tech Stack:** .NET 8, Entity Framework Core (SQLite), Serilog.
* **Asynchronous I/O:** Fully async pipeline using `Task` and `CancellationToken`.
* **Robustness:**
    * Graceful Shutdown (Handles `Ctrl+C` safely).
    * Input Validation & Retry loop.
    * Result Pattern for flow control (instead of Exceptions).
* **Quality Assurance:**
    * Unit Tests (xUnit, Moq, Bogus) covering Services and Repositories.
    * CI Pipeline (GitHub Actions).
    * Containerized with Docker.

## 🛠 Prerequisites

* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Docker Desktop](https://www.docker.com/products/docker-desktop) (Optional)

## 🏃 Getting Started

### 1. Run Locally

```bash
# Clone the repository
git clone [https://github.com/Quan95vn/LegacyOrderService.git](https://github.com/Quan95vn/LegacyOrderService.git)
cd LegacyOrderService

# Restore dependencies
dotnet restore

# Run Database Migrations (Initialize SQLite DB)
cd src/LegacyOrderService
dotnet ef database update

# Run the Application
dotnet run
```

### 2. Run with Docker

You can run the application in an isolated container without installing the .NET SDK.

```bash
# Build the Docker Image
docker build -t orderservice:latest .

# Run the Container
# -it: Interactive mode (required for console input)
# -v: Mount local directory to persist database data
docker run -it --rm -v ${PWD}/db-data:/app/data orderservice:latest
```

For Linux / macOS (Bash):
```bash
docker run -it --rm -v ${PWD}/src/LegacyOrderService/LocalDb:/app/LocalDb orderservice:latest
```

For Windows (PowerShell):
```bash
docker run -it --rm -v ${PWD}\src\LegacyOrderService\LocalDb:/app/LocalDb orderservice:latest
```

For Windows (Command Prompt - CMD):
```bash
docker run -it --rm -v %cd%\src\LegacyOrderService\LocalDb:/app/LocalDb orderservice:latest
```

### 3. Run Tests

```bash
Ensure you are at the **solution root** (if you followed the steps above, you need to navigate back up two levels):

```bash
# Return to root directory
cd ../..

# Run all unit tests in the solution
dotnet test
```

### 4. Database Scalability
Current State: The application uses SQLite for portability and ease of review (zero-configuration).

Future Roadmap: Thanks to EF Core abstraction, migrating to a robust RDBMS (SQL Server/PostgreSQL) for production requires minimal changes:

Change the NuGet package (e.g., Microsoft.EntityFrameworkCore.SqlServer).
Update Program.cs to use .UseSqlServer().

Update Connection String in appsettings.json.

Regenerate Migrations (due to syntax differences in raw SQL scripts).

## 📂 Project Structure

```
├── .github/workflows   # CI Pipeline configuration
├── src/
│   └── LegacyOrderService      # Main Application
├── test/
│   └── LegacyOrderService.Tests # Unit Tests
├── Dockerfile          # Docker build instructions
└── OrderServiceSolution.sln
```