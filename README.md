# WebDevToCSharp API Documentation

## 📋 Table of Contents

- [Overview](#overview)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Architecture & Flow](#architecture--flow)
- [API Specification](#api-specification)
  - [Authentication Endpoints](#authentication-endpoints)
  - [User Endpoints](#user-endpoints)
  - [File Endpoints](#file-endpoints)
- [Configuration](#configuration)
- [Getting Started](#getting-started)
- [Error Handling](#error-handling)

---

## Overview

WebDevToCSharp is a RESTful API built with **ASP.NET Core** that provides user authentication, user management, and file upload functionality. The API uses JWT (JSON Web Tokens) for secure authentication and BCrypt for password hashing.

### Key Features

- ✅ User Registration & Login
- ✅ JWT-based Authentication
- ✅ Password Hashing with BCrypt
- ✅ File Upload with Validation
- ✅ In-Memory User Repository
- ✅ Minimal API Architecture

---

## Technology Stack

| Component | Technology | Version |
|-----------|------------|---------|
| Framework | ASP.NET Core | .NET 10.0 |
| Authentication | JWT Bearer | 10.0.9 |
| Password Hashing | BCrypt.Net-Next | 4.2.0 |
| Token Library | System.IdentityModel.Tokens.Jwt | 8.19.1 |
| Language | C# | Latest |

---

## Project Structure

```
/workspace
├── Program.cs                 # Application entry point & configuration
├── WebDevCSharp.csproj        # Project file with dependencies
├── appsettings.json           # Configuration (JWT settings)
├── appsettings.Development.json
│
├── Endpoints/                 # API Endpoint definitions
│   ├── AuthEndpoints.cs       # Register & Login endpoints
│   ├── UserEndpoints.cs       # User management endpoints
│   └── FileEndpoints.cs       # File upload endpoints
│
├── Models/                    # Data models
│   ├── User.cs                # User entity
│   └── Dtos/                  # Data Transfer Objects
│       ├── AuthDtos.cs        # RegisterDto, LoginDto, AuthResponseDto
│       └── UserDto.cs         # UserDto
│
├── Services/                  # Business logic layer
│   ├── IAuthService.cs        # Auth service interface
│   ├── AuthService.cs         # JWT token generation
│   ├── IUserRepository.cs     # Repository interface
│   └── InMemoryUserRepository.cs  # In-memory implementation
│
└── Properties/                # Project properties
    └── launchSettings.json
```

---

## Architecture & Flow

### Authentication Flow

```
┌─────────────┐      ┌─────────────┐      ┌─────────────┐      ┌─────────────┐
│   Client    │      │     API     │      │ AuthService │      │ UserRepository│
└──────┬──────┘      └──────┬──────┘      └──────┬──────┘      └──────┬──────┘
       │                    │                    │                    │
       │ POST /register     │                    │                    │
       │───────────────────>│                    │                    │
       │                    │ Check email exists │                    │
       │                    │───────────────────>│                    │
       │                    │                    │                    │
       │                    │ Hash password      │                    │
       │                    │ (BCrypt)           │                    │
       │                    │                    │                    │
       │                    │ Create user        │                    │
       │                    │────────────────────────────────────────>│
       │                    │                    │                    │
       │  200 OK            │                    │                    │
       │<───────────────────│                    │                    │
       │                    │                    │                    │
       │ POST /login        │                    │                    │
       │───────────────────>│                    │                    │
       │                    │ Get user by email  │                    │
       │                    │────────────────────────────────────────>│
       │                    │<────────────────────────────────────────│
       │                    │ Verify password    │                    │
       │                    │ (BCrypt)           │                    │
       │                    │                    │                    │
       │                    │ Generate JWT token │                    │
       │                    │───────────────────>│                    │
       │                    │<───────────────────│                    │
       │  Token + User info │                    │                    │
       │<───────────────────│                    │                    │
       │                    │                    │                    │
       │ GET /users         │                    │                    │
       │ (with JWT)         │                    │                    │
       │───────────────────>│                    │                    │
       │                    │ Validate JWT       │                    │
       │                    │                    │                    │
       │                    │ Get all users      │                    │
       │                    │────────────────────────────────────────>│
       │                    │<────────────────────────────────────────│
       │  Users list        │                    │                    │
       │<───────────────────│                    │                    │
       │                    │                    │                    │
```

### Request Processing Pipeline

1. **Authentication Middleware** - Validates JWT token if present
2. **Authorization Middleware** - Checks user permissions
3. **Endpoint Routing** - Maps request to appropriate handler
4. **Business Logic** - Executes service layer operations
5. **Response** - Returns formatted JSON response

---

## API Specification

Base URL: `http://localhost:5000` (default)

### Authentication Endpoints

#### 1. Register User

Creates a new user account.

| Detail | Value |
|--------|-------|
| **Endpoint** | `/api/auth/register` |
| **Method** | `POST` |
| **Auth Required** | ❌ No |
| **Content-Type** | `application/json` |

**Request Body:**
```json
{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

**CURL Request:**
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "johndoe",
    "email": "john@example.com",
    "password": "SecurePass123!"
  }'
```

**Success Response (200 OK):**
```json
{
  "message": "User registered successfully"
}
```

**Error Responses:**

| Status Code | Response | Description |
|-------------|----------|-------------|
| 400 Bad Request | `{"message": "Email and password required"}` | Missing required fields |
| 409 Conflict | `{"message": "Email already registered"}` | Email already exists |

---

#### 2. Login User

Authenticates user and returns JWT token.

| Detail | Value |
|--------|-------|
| **Endpoint** | `/api/auth/login` |
| **Method** | `POST` |
| **Auth Required** | ❌ No |
| **Content-Type** | `application/json` |

**Request Body:**
```json
{
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

**CURL Request:**
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "SecurePass123!"
  }'
```

**Success Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiZW1haWwiOiJqb2huQGV4YW1wbGUuY29tIiwidW5pcXVlbmFtZSI6ImpvaG5kb2UiLCJqdGkiOiJhYmMxMjMiLCJleHAiOjE2OTk5OTk5OTksImlzcyI6IkFQSVdlYmRldiIsImF1ZCI6IkFQSVdlYmRldlVzZXIifQ.signature",
  "user": {
    "id": 1,
    "username": "johndoe",
    "email": "john@example.com",
    "createdAt": "2024-01-15T10:30:00Z"
  }
}
```

**Error Responses:**

| Status Code | Response | Description |
|-------------|----------|-------------|
| 401 Unauthorized | `null` | Invalid email or password |

---

### User Endpoints

All user endpoints require JWT authentication.

#### 3. Get All Users

Retrieves a list of all registered users.

| Detail | Value |
|--------|-------|
| **Endpoint** | `/api/users` |
| **Method** | `GET` |
| **Auth Required** | ✅ Yes (Bearer Token) |

**CURL Request:**
```bash
curl -X GET http://localhost:5000/api/users \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Success Response (200 OK):**
```json
[
  {
    "id": 1,
    "username": "johndoe",
    "email": "john@example.com",
    "createdAt": "2024-01-15T10:30:00Z"
  },
  {
    "id": 2,
    "username": "janedoe",
    "email": "jane@example.com",
    "createdAt": "2024-01-16T14:20:00Z"
  }
]
```

**Error Responses:**

| Status Code | Response | Description |
|-------------|----------|-------------|
| 401 Unauthorized | `null` | Missing or invalid token |
| 403 Forbidden | `null` | Token expired or insufficient permissions |

---

### File Endpoints

All file endpoints require JWT authentication.

#### 4. Upload File

Uploads a file to the server with validation.

| Detail | Value |
|--------|-------|
| **Endpoint** | `/api/files/upload` |
| **Method** | `POST` |
| **Auth Required** | ✅ Yes (Bearer Token) |
| **Content-Type** | `multipart/form-data` |

**File Constraints:**

| Constraint | Value |
|------------|-------|
| Max Size | 5 MB |
| Allowed Extensions | `.jpg`, `.jpeg`, `.png`, `.pdf` |

**CURL Request:**
```bash
curl -X POST http://localhost:5000/api/files/upload \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -F "file=@/path/to/your/file.jpg"
```

**Success Response (200 OK):**
```json
{
  "message": "File uploaded successfully",
  "fileName": "a1b2c3d4-e5f6-7890-abcd-ef1234567890.jpg",
  "size": 245678,
  "url": "/uploads/a1b2c3d4-e5f6-7890-abcd-ef1234567890.jpg"
}
```

**Error Responses:**

| Status Code | Response | Description |
|-------------|----------|-------------|
| 400 Bad Request | `{"message": "No file uploaded"}` | No file provided |
| 400 Bad Request | `{"message": "File too large (max 5MB)"}` | File exceeds size limit |
| 400 Bad Request | `{"message": "Invalid file type"}` | Unsupported file extension |
| 401 Unauthorized | `null` | Missing or invalid token |

---

## Complete Endpoint Summary Table

| # | Method | Endpoint | Auth Required | Description |
|---|--------|----------|---------------|-------------|
| 1 | POST | `/api/auth/register` | ❌ No | Register new user |
| 2 | POST | `/api/auth/login` | ❌ No | Login and get JWT token |
| 3 | GET | `/api/users` | ✅ Yes | Get all users |
| 4 | POST | `/api/files/upload` | ✅ Yes | Upload file |

---

## Configuration

### JWT Settings (appsettings.json)

```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyHere",
    "Issuer": "APIWebdev",
    "Audience": "APIWebdevUser",
    "ExpirationInMinutes": 60
  }
}
```

| Setting | Description | Default |
|---------|-------------|---------|
| `SecretKey` | Secret key for signing JWT tokens | `SecretKey` |
| `Issuer` | Token issuer identifier | `APIWebdev` |
| `Audience` | Token audience identifier | `APIWebdevUser` |
| `ExpirationInMinutes` | Token validity duration | `60` |

> ⚠️ **Security Warning**: Change the `SecretKey` in production to a strong, randomly generated string.

---

## Getting Started

### Prerequisites

- .NET 10.0 SDK or later
- cURL (for testing)

### Installation

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd WebDevToCSharp
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Configure JWT settings:**
   Edit `appsettings.json` and update the `SecretKey` with a secure value.

4. **Run the application:**
   ```bash
   dotnet run
   ```

5. **Test the API:**
   Use the CURL examples above to test the endpoints.

---

## Error Handling

The API follows standard HTTP status code conventions:

| Status Code | Meaning | When Returned |
|-------------|---------|---------------|
| 200 | OK | Successful request |
| 400 | Bad Request | Invalid input or validation failure |
| 401 | Unauthorized | Missing or invalid authentication |
| 403 | Forbidden | Valid auth but insufficient permissions |
| 409 | Conflict | Resource conflict (e.g., duplicate email) |
| 500 | Internal Server Error | Server-side error |

### Error Response Format

```json
{
  "message": "Description of the error"
}
```

---

## Security Considerations

1. **Password Storage**: All passwords are hashed using BCrypt before storage
2. **JWT Tokens**: Tokens expire after 60 minutes by default
3. **HTTPS**: Always use HTTPS in production environments
4. **Rate Limiting**: Consider implementing rate limiting for production
5. **Input Validation**: All inputs are validated before processing

---


## License

This project is for educational purposes.

---
