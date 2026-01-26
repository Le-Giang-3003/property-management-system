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
git clone <repository-url>
cd property-management-system

### Step 2: Configure database connection

Open appsettings.json and update the connection string:
"ConnectionStrings": {
  "DefaultConnection": "Server=local;Database=PropertyManagementDB;User Id=sa;Password=12345;TrustServerCertificate=True;"
}

### Step 3: Apply database migrations

At navigation bar, open Tools -> NuGet Package Manager -> Package Manager Console
Enter add-migration <Name of migration>, choose default project is the project that have AppDbContext
Then enter update-database