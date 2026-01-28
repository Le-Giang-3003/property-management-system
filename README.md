# Property Management System

Property Management System is a web-based application developed for **PRN222**, designed to support **buying, selling, and renting real estate properties**.  
The system is built using **C#** with **ASP.NET MVC (.NET)** and follows the MVC architectural pattern.

---

## Table of Contents
- [Overview](#overview)
- [Features](#features)
- [System Roles](#system-roles)
- [Technologies Used](#technologies-used)
- [System Requirements](#system-requirements)
- [Installation Guide](#installation-guide)
- [Running the Application](#running-the-application)
- [Project Structure](#project-structure)
- [Future Improvements](#future-improvements)
- [License](#license)

---

## Overview

The **Property Management System** aims to provide a centralized platform where:
- Property owners can manage and publish their properties for rent or sale
- Users can search, view, and request to rent or buy properties
- Administrators can manage users, properties, and system data

This project focuses on applying **ASP.NET MVC**, **Entity Framework Core**, and **SQL Server** in a real-world business scenario.

---

## Features

- User registration and authentication
- Role-based authorization
- Property listing (rent / sale)
- Property search and filtering
- Property management (CRUD)
- Contract and document management
- Admin management dashboard

---

## System Roles

### Admin
- Manage users and roles
- Manage all properties
- Monitor system data

### Landlord / Seller
- Create and manage property listings
- Upload related documents
- Manage rental or sale information

### User (Tenant / Buyer)
- Browse and search properties
- View property details
- Send rental or purchase requests

---

## Technologies Used

- **Language**: C#
- **Framework**: ASP.NET MVC (.NET)
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Frontend**: Razor View, HTML, CSS, JavaScript
- **Authentication**: ASP.NET Identity / JWT

---

## System Requirements

Before running the project, make sure your environment meets the following requirements:

- .NET SDK 6.0 or later
- SQL Server / SQL Server Express
- Visual Studio 2022  
  (with **ASP.NET and web development** workload installed)
- Modern web browser (Chrome, Edge, Firefox)

---

## Installation Guide

### Step 1: Clone the repository

```bash
git clone https://github.com/Le-Giang-3003/property-management-system.git
cd property-management-system
```

### Step 2: Configure database connection

Open appsettings.json and update the connection string:
"ConnectionStrings": {
  "DefaultConnection": "Server=local;Database=PropertyManagementDB;User Id=sa;Password=12345;TrustServerCertificate=True;"
}

### Step 3: Apply database migrations
- At navigation bar, open Tools -> NuGet Package Manager -> Package Manager Console.
- Enter add-migration <Name of migration>, choose default project is the project that have AppDbContext (DataAccessLayer).
- Then enter update-database.
## Running the Application
Using Visual Studio
1. Open the solution file (.sln) in Visual Studio 2022
2. Set the Web project as the Startup Project
3. Press F5 or click Run
4. The application will be available at:
```bash
https://localhost:7206
```
Using .NET CLI (optional)
```bash
dotnet restore
dotnet build
dotnet run
```

### Admin Portal Login

- **Cùng trang đăng nhập**: Dùng trang **Auth/Login** (mặc định là trang chủ khi chưa đăng nhập).
- **Tài khoản Admin mặc định (lần chạy đầu)**  
  Nếu trong database chưa có user nào có role Admin, ứng dụng sẽ tự tạo một tài khoản Admin khi khởi động:
  - **Email**: `admin@localhost` (hoặc cấu hình trong `appsettings.json` → `SeedAdmin:Email`)
  - **Mật khẩu**: `Admin@123` (hoặc cấu hình trong `appsettings.json` → `SeedAdmin:Password`)
- Sau khi đăng nhập với tài khoản Admin, hệ thống sẽ chuyển hướng đến **Admin Dashboard** (`/Admin`). Từ menu bên trái có thể vào: Manage Users, Create User, Roles & Permissions, System Settings, Audit Logs.
- **Tạo thêm Admin**: Đăng nhập bằng một Admin hiện có → **Admin** → **Create User** → chọn Role **Admin** và tạo tài khoản mới.

## Project Structure
```bash
PropertyManagementSystem
│
├── PropertyManagementSystem.Web        (Presentation Layer - MVC)
│   │
│   ├── Controllers
│   │   ├── UserController.cs
│   │   ├── PropertyController.cs
│   │   ├── DashboardController.cs
│   │
│   ├── Views
│   │   ├── Shared
│   │   ├── User
│   │   ├── Property
│   │
│   ├── wwwroot
│   │   ├── css
│   │   ├── js
│   │   └── images
│   │
│   ├── Program.cs
│   ├── appsettings.json
│   └── PropertyManagementSystem.Web.csproj
│
├── PropertyManagementSystem.BLL        (Business Logic Layer)
│   │
│   ├── Services
│   │   ├── Interfaces
│   │   │   ├── IPropertyService.cs
│   │   │   ├── IUserService.cs
│   │   │   └── IDocumentService.cs
│   │   │
│   │   └── Implementations
│   │       ├── PropertyService.cs
│   │       ├── UserService.cs
│   │       └── DocumentService.cs
│   │
│   ├── DTOs
│   │   ├── PropertyDTO.cs
│   │   ├── UserDTO.cs
│   │   └── ContractDTO.cs
│   │
│   └── PropertyManagementSystem.BLL.csproj
│
├── PropertyManagementSystem.DAL        (Data Access Layer)
│   │
│   ├── Entities
│   │   ├── Property.cs
│   │   ├── User.cs
│   │   ├── Lease.cs
│   │   └── Document.cs
│   │
│   ├── DbContext
│   │   └── AppDbContext.cs
│   │
│   ├── Repositories
│   │   ├── Interfaces
│   │   │   ├── IPropertyRepository.cs
│   │   │   └── IUserRepository.cs
│   │   │
│   │   └── Implementations
│   │       ├── PropertyRepository.cs
│   │       └── UserRepository.cs
│   │
│   ├── Migrations
│   │
│   └── PropertyManagementSystem.DAL.csproj
│
├── PropertyManagementSystem.sln
└── README.md
```
## Key Design Principles
1. MVC Architecture
Clear separation of concerns between:
- Model (Business logic & data)
- View (UI)
- Controller (Request handling)
2. Layered Architecture
- Presentation Layer (Controllers, Views)
- Business Logic Layer (Services)
- Data Access Layer (EF Core, DbContext)
3. Dependency Injection
- Services are injected via interfaces
- Improves testability and maintainability

## Security Considerations
- Authentication using ASP.NET Identity / JWT
- Role-based authorization using [Authorize] attributes
- Secure password hashing
- Validation for user input
- Protection against common attacks (SQL Injection via EF Core)

## Future Improvements
- Online payment integration
- Advanced search (map-based, price range, location)
- Notification system (email / in-app)
- Report and analytics dashboard
- Mobile-friendly UI or mobile application
- Microservice architecture (optional extension)
## Limitations
- The project is developed for educational purposes
- Not optimized for large-scale production use
- Some business flows may be simplified
## License
This project is developed for academic use in the course PRN222.
All rights reserved © 2026.
## Author
Group: Group 3 - SE1815
Course: PRN222 – ASP.NET MVC
Institution: FPT University
