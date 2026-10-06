# 🚗 CarShare - Peer-to-Peer Car Rental Platform (Backend API)

![.NET](https://img.shields.io/badge/.NET-9.0%20%7C%208.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?style=for-the-badge&logo=dotnet)
![Entity Framework](https://img.shields.io/badge/EF%20Core-SQL%20Server-512BD4?style=for-the-badge&logo=nuget)
![JWT](https://img.shields.io/badge/JWT-Authentication-black?style=for-the-badge&logo=jsonwebtokens)
![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)

A robust, enterprise-ready RESTful Web API for a peer-to-peer car rental platform. Built using **ASP.NET Core**, **Entity Framework Core**, and Clean 3-Tier Layered Architecture.

---

## 📌 Table of Contents
- [✨ Key Features](#-key-features)
- [🏗️ System Architecture](#️-system-architecture)
- [🛠️ Tech Stack](#️-tech-stack)
- [🔑 Roles & Permissions](#-roles--permissions)
- [📡 API Endpoints Overview](#-api-endpoints-overview)
- [🚀 Getting Started](#-getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation & Configuration](#installation--configuration)
  - [Database Migration](#database-migration)
  - [Running the API](#running-the-api)
- [📄 Database Diagram & Relationships](#-database-diagram--relationships)
- [🌐 Frontend Repository & Live Demo](#-frontend-repository--live-demo)

---

## ✨ Key Features

* 🔐 **Authentication & Authorization**:
  * Secure user registration and login with encrypted passwords.
  * Role-Based Access Control (RBAC) via JSON Web Tokens (JWT) with customized claims.
* 🚘 **Car Management**:
  * Car owners can publish vehicle listings with full specifications, rental rates, and location.
  * Admin review & approval pipeline before listings become publicly searchable.
  * Query available approved cars with comprehensive filtering.
* 📝 **Rental Proposals & Bookings**:
  * Renters submit rental proposals with customized date ranges, messages, and uploaded verification documents.
  * Multi-part form data support for vehicle licenses and identity document uploads (`/uploads`).
  * Car owners review, approve, or reject rental requests.
* ⭐ **Ratings & Reviews**:
  * Verified renters can review vehicles and owners post-rental.

---

## 🏗️ System Architecture

The solution follows a structured **3-Tier / Layered Architecture**:

```text
├── CarShare.API (Presentation Layer)
│   ├── Controllers (AuthController, CarsController, RentalsController, UsersController)
│   ├── Middleware & Program.cs (CORS, JWT Authentication, Swagger OpenAPI)
│   └── wwwroot/uploads (Uploaded document storage)
│
├── CarShare.BLL (Business Logic Layer)
│   ├── DTOs (Car, Rental, User, Review)
│   ├── Interfaces (ICarService, IRentalService, IUserService)
│   ├── Services (CarService, RentalService, UserService)
│   └── Mappings (AutoMapper Profiles)
│
└── CarShare.DAL (Data Access Layer)
    ├── Entities / Models (User, Car, RentalProposal, Rental, Review)
    ├── Data / DbContext (ApplicationDbContext)
    └── Repositories (CarRepository, RentalRepository, UserRepository)
```

---

## 🛠️ Tech Stack

* **Language**: C# 12 / 13
* **Framework**: ASP.NET Core Web API (.NET 9 / .NET 8)
* **ORM**: Entity Framework Core
* **Database**: Microsoft SQL Server
* **Authentication**: JWT Bearer Tokens (`System.IdentityModel.Tokens.Jwt`)
* **Object Mapping**: AutoMapper
* **API Documentation**: Swagger / OpenAPI (Swashbuckle)

---

## 🔑 Roles & Permissions

| Role | Permissions |
| :--- | :--- |
| **Admin** | Approve/reject car listings, manage all users, system oversight |
| **CarOwner** | Add & manage owned vehicles, approve/reject rental proposals |
| **Renter** | Browse approved cars, submit rental proposals with license verification files, leave reviews |

---

## 📡 API Endpoints Overview

### 🔐 Authentication (`/api/Auth`)
* `POST /api/Auth/register` - Register a new user (`CarOwner` or `Renter`)
* `POST /api/Auth/login` - Authenticate user and receive a JWT token

### 👤 Users (`/api/Users`)
* `GET /api/Users/profile` - Get current authenticated user's profile

### 🚗 Cars (`/api/Cars`)
* `GET /api/Cars` - List all available and approved cars
* `GET /api/Cars/{id}` - Get car details by ID
* `POST /api/Cars` - Add a new car listing *(CarOwner only)*
* `PATCH /api/Cars/{id}/approve` - Approve a car listing *(Admin only)*

### 📑 Rentals & Proposals (`/api/Rentals`)
* `POST /api/Rentals/proposals` - Submit a new rental proposal with uploaded documents (`multipart/form-data`) *(Renter only)*
* `PATCH /api/Rentals/proposals/{id}/approve` - Approve a rental proposal *(CarOwner only)*
* `GET /api/Rentals/proposals/{id}` - View rental proposal details

---

## 🚀 Getting Started

### Prerequisites
* [.NET SDK 8.0 or 9.0](https://dotnet.microsoft.com/download)
* [SQL Server](https://www.microsoft.com/sql-server) or SQL Server Express / LocalDB
* [Visual Studio 2022](https://visualstudio.microsoft.com/) / [VS Code](https://code.visualstudio.com/) / [Rider](https://www.jetbrains.com/rider/)

### Installation & Configuration

1. **Clone the repository:**
   ```bash
   git clone https://github.com/MaryamElghazaly/CarShareBackend.git
   cd CarShareBackend
   ```

2. **Configure Database Connection & JWT:**
   Open `CarShare.API/appsettings.json` and adjust the connection string and JWT key as needed:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=CarShareDB;Trusted_Connection=True;TrustServerCertificate=True;"
     },
     "Jwt": {
       "Key": "YourSuperSecretJWTKeyHereWithAtLeast32Characters!",
       "Issuer": "CarShareAPI",
       "Audience": "CarShareClients"
     }
   }
   ```

3. **Apply Database Migrations:**
   ```bash
   dotnet ef database update --project CarShare.DAL --startup-project CarShare.API
   ```

4. **Run the API:**
   ```bash
   dotnet run --project CarShare.API
   ```

5. **Explore Swagger UI:**
   Navigate to `https://localhost:7023/swagger` or `http://localhost:5023/swagger` in your browser.

---

## 🌐 Frontend Repository & Live Demo

* **Frontend Repository**: [CarShare (React + Vite)](https://github.com/MaryamElghazaly/CarShare)
* **Live Demo**: [CarShare Web App](https://maryamelghazaly.github.io/CarShare/)

---

## 👩‍💻 Author
**Maryam Elghazaly**
* GitHub: [@MaryamElghazaly](https://github.com/MaryamElghazaly)
