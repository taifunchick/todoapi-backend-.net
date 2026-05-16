# 📝 Todo API - Minimal API

<div align="center">

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![SQLite](https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger)

**Simple todo API with filters and statistics**

</div>

---

## 📖 About

Lightweight Todo API built with ASP.NET Core Minimal API. Features filtering, priority levels, categories, due dates, and statistics.

---

## ✨ Features

| Feature | Description |
|---------|-------------|
| 📋 CRUD operations | Create, read, update, delete tasks |
| 🔍 Filters | By category, priority, status, search |
| 📊 Statistics | Completion rate, overdue tasks |
| 🏷️ Categories | Auto-generated category list |
| ⚡ Priority | High / Medium / Low |
| ✅ Toggle complete | One-click status change |

---

## 🚀 Quick Start

```bash
# Create project
dotnet new web -n TodoApi
cd TodoApi

# Install packages
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Swashbuckle.AspNetCore

# Run
dotnet run
```

Open http://localhost:5000/swagger

---

## 📡 API Endpoints

| Method | URL                 | Description          |
|--------|---------------------|----------------------|
| GET    | `/todos`            | All tasks (+ filters)|
| GET    | `/todos/{id}`       | Single task          |
| POST   | `/todos`            | Create task          |
| PUT    | `/todos/{id}`       | Update task          |
| PATCH  | `/todos/{id}/toggle`| Toggle complete      |
| DELETE | `/todos/{id}`       | Delete task          |
| GET    | `/stats`            | Statistics           |
| GET    | `/categories`       | Category list        |
