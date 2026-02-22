# 🎤 EchoWarehouse

**AI-Powered Voice-Controlled Warehouse Management System**

Modern, multiplatform warehouse management system with voice control powered by artificial intelligence. Speak to the system naturally: *"Add 50 kilograms of cement to shelf 12"* - and it's done!

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18-61DAFB?logo=react)](https://reactjs.org/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15+-336791?logo=postgresql)](https://www.postgresql.org/)
[![Azure](https://img.shields.io/badge/Azure-Speech%20Services-0078D4?logo=microsoft-azure)](https://azure.microsoft.com/en-us/services/cognitive-services/speech-services/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

---

## 📋 Table of Contents

- [Features](#-features)
- [Technology Stack](#-technology-stack)
- [Architecture](#-architecture)
- [Database Schema](#-database-schema)
- [Installation](#-installation)
- [Configuration](#-configuration)
- [Authentication](#-authentication)
- [Bootstrap API](#-bootstrap-api)
- [Usage](#-usage)
- [API Documentation](#-api-documentation)
- [Voice Commands](#-voice-commands)
- [Internationalization](#-internationalization)
- [Project Structure](#-project-structure)
- [Deployment](#-deployment)
- [License](#-license)

---

## ✨ Features

### 🔐 Authentication & Security
- **Secure user authentication** with JWT tokens
- **Protected API endpoints** - authentication required for all operations
- **Registration with secret code** - prevents unauthorized registrations
- **Role-based access** (extendable for future admin features)

### 🚀 Bootstrap System
- **Single API call on startup** - `/api/bootstrap` loads all configuration
- **Centralized configuration** - all app settings in one database table
- **Version tracking** - database version and backend version included
- **One-time load** - all settings cached in frontend context

### 🎙️ Voice Control (AI-powered)
- **Multi-language speech recognition** (English, Hungarian) with Azure Speech Services
- **Natural language processing** with Google Gemini API
- **Real-time voice-to-action conversion**
- Supported operations:
  - 📦 Add items via voice command
  - 📤 Issue items
  - ✏️ Modify products
  - 🔍 Search
  - 🗑️ Delete

### 📊 Warehouse Management
- **Full CRUD operations** for products
- **Multiple units of measurement** (pieces, kg, g, l, ml, etc.)
- **Automatic conversions** between weight ⟷ volume
- **Density-based calculations**
- **Dynamic VAT calculation** - VAT rate from app config
- **Net ⟷ Gross price conversion** - automatic calculation in frontend
- **VAT Number management** - company tax identification
- **Location tracking** (shelf, warehouse)
- **Serial number & article number** management
- **Stock movement tracking**
- **History & audit trail**

### 🌐 Internationalization (i18n)
- **Database-driven localization** - all UI text stored as resource keys in PostgreSQL
- **One-time load on startup** - all translations fetched once via bootstrap
- **LocalStorage persistence** - selected locale saved locally
- **Default language** - configured in app settings
- **useLocalization hook** - simple key resolution in components
- **Instant language switching** - no API calls needed after initial load

### ⚙️ Application Settings
- **Company Information** - Company name, VAT number
- **Tax Configuration** - Default VAT rate (%)
- **Currency Settings** - Default currency (e.g., HUF, EUR, USD)
- **Language Settings** - Default application language
- **Version Information** - Database version, Backend version

### 🔍 Search & Filter
- Quick search by name, article number, serial number
- Filter by location
- Stock level alerts
- Full-text search

### 📈 Reporting & Analytics
- Stock movement reports
- Voice command analytics
- Real-time inventory status
- Export functionality (CSV, Excel)

---

## 🛠️ Technology Stack

### Backend
```
✅ .NET 8.0 (multiplatform: Windows, Linux, macOS)
✅ ASP.NET Core Web API
✅ ASP.NET Core Identity (authentication)
✅ JWT Bearer Authentication
✅ Entity Framework Core 8.0 (Code-First)
✅ Npgsql (PostgreSQL provider)
✅ Dapper (raw SQL queries)
✅ Azure.CognitiveServices.Speech
✅ Google.Cloud.AIPlatform.V1 (Gemini API)
✅ Swagger/OpenAPI (API documentation)
✅ AutoMapper
✅ FluentValidation
✅ Serilog (structured logging)
```

### Design Patterns
- **Repository Pattern** - data access layer abstraction
- **Dependency Injection** - ASP.NET Core built-in DI container
- **Unit of Work Pattern** - transaction management
- **CQRS-lite** - separation of commands and queries

### Frontend
```
✅ React 18 + TypeScript
✅ Vite (build tool)
✅ Tailwind CSS
✅ Axios (HTTP client)
✅ Context API (state management + configuration)
✅ LocalStorage (locale + auth token persistence)
✅ Custom useLocalization hook
✅ Custom useAuth hook
✅ Custom useAppConfig hook
✅ Protected Routes
✅ Web Speech API
```

### Database
```
✅ PostgreSQL 15+
✅ Entity Framework Core Migrations
✅ Raw SQL support for complex queries
✅ Full-text search (PostgreSQL native)
✅ JSONB support (voice command logs)
✅ Resource key storage for i18n
✅ User authentication storage
✅ Centralized app configuration
```

### DevOps & Hosting
```
✅ Azure App Service (F1 Free Tier)
✅ Firebase Hosting (frontend)
✅ Supabase (PostgreSQL hosting)
✅ GitHub Actions (CI/CD)
✅ Docker support
✅ Environment variable management (.env files)
```

---

## 🏗️ Architecture

### High-Level Architecture

```
┌──────────────────────────────────────────────────────┐
│                 React Frontend                        │
│  ┌────────────────────────────────────────────────┐  │
│  │  Authentication Check (Login/Register)         │  │
│  │  → JWT token in LocalStorage                   │  │
│  └────────────────────────────────────────────────┘  │
│  ┌────────────────────────────────────────────────┐  │
│  │  App Initialization (if authenticated):        │  │
│  │  → Call /api/bootstrap (ONE TIME)              │  │
│  │    Returns:                                     │  │
│  │    • App Settings (VAT, Company, Currency...)  │  │
│  │    • Localization (all resource keys)          │  │
│  │    • Version info (DB, Backend)                │  │
│  │  → Store everything in Context                 │  │
│  └────────────────────────────────────────────────┘  │
│  ┌────────────────────────────────────────────────┐  │
│  │  Voice Input  │  Product CRUD  │  Dashboard   │  │
│  │  All requests include JWT token               │  │
│  │  Prices calculated with cached VAT rate       │  │
│  │  UI text from cached localization             │  │
│  └────────────────────────────────────────────────┘  │
└───────────────────────┬──────────────────────────────┘
                        │ HTTPS/REST API + JWT
                        ▼
┌────────────────────────────────���─────────────────────┐
│            ASP.NET Core Web API (.NET 8)             │
│  ┌────────────────────────────────────────────────┐  │
│  │  JWT Authentication Middleware                 │  │
│  │  → Validates token on every request            │  │
│  └────────────────────────────────────────────────┘  │
│  ┌────────────────────────────────────────────────┐  │
│  │           Controllers Layer                    │  │
│  │  BootstrapController (single load endpoint)    │  │
│  │  AuthController (login/register)               │  │
│  │  VoiceController │ ProductsController │ ...    │  │
│  │  SettingsController (admin settings mgmt)     │  │
│  │  Scalar API at /scalar                        │  │
│  └────────────────┬───────────────────────────────┘  │
│                   │                                   │
│  ┌────────────────▼───────────────────────────────┐  │
│  │           Services Layer                       │  │
│  │  BootstrapService │ AuthService │ AIService    │  │
│  │  AppConfigService │ LocalizationService        │  │
│  └────────────────┬───────────────────────────────┘  │
│                   │                                   │
│  ┌────────────────▼───────────────────────────────┐  │
│  │      Repository Pattern + UnitOfWork           │  │
│  │  IUserRepo │ IProductRepo │ IAppConfigRepo    │  │
│  └────────────────┬───────────────────────────────┘  │
│                   │                                   │
│  ┌────────────────▼───────────────────────────────┐  │
│  │         Data Access Layer                      │  │
│  │  EF Core Context │ Dapper (Raw SQL)            │  │
│  └────────────────┬───────────────────────────────┘  │
└────────────────────┼──────────────────────────────────┘
                     │
                     ▼
      ┌──────────────────────────────────┐
      │   PostgreSQL Database            │
      │  Users (authentication)          │
      │  Products │ History │ VoiceLog   │
      │  ResourceKeys (i18n)             │
      │  AppConfig (all settings)        │
      │    - VAT rate, VAT number        │
      │    - Company name, Currency      │
      │    - Default language            │
      └──────────────────────────────────┘

      ┌──────────────────────────────────┐
      │   External Services              │
      │  Azure Speech │ Google Gemini    │
      └──────────────────────────────────┘

      ┌──────────────────────────────────┐
      │   Environment Variables          │
      │  REGISTRATION_SECRET (not in git)│
      └──────────────────────────────────┘
```

### Backend Architecture Layers

```
EchoWarehouse.API/
├── Controllers/          // API endpoints
├── Services/            // Business logic
│   ├── Interfaces/
│   └── Implementations/
├── Repositories/        // Data access (Repository Pattern)
│   ├── Interfaces/
│   └── Implementations/
├── Data/               // EF Core Context, Migrations
├── Models/             // Domain entities
│   ├── Entities/
│   ├── DTOs/
│   └── ViewModels/
├── Middleware/         // Custom middleware + JWT Auth
├── Validators/         // FluentValidation
└── Extensions/         // Service extensions, helpers
```

### Bootstrap Flow

```
1. User opens app
   ↓
2. Check LocalStorage for JWT token
   ↓
3a. No token → Show Login/Register screen
3b. Has token → Validate with backend
   ↓
4. If authenticated:
   Call GET /api/bootstrap (ONE TIME, ONE REQUEST)
   ↓
5. Bootstrap Response includes:
   {
     "appSettings": {
       "defaultVatRate": 27.00,
       "vatNumber": "HU12345678",
       "companyName": "Your Company",
       "currency": "HUF",
       "defaultLanguage": "hu-HU"
     },
     "localization": {
       "UI_Product_Name": "Termék neve",
       "UI_Button_Save": "Mentés",
       ...500+ keys
     },
     "version": {
       "backend": "1.0.0",
       "database": "20260207001"
     }
   }
   ↓
6. Store everything in React Context:
   - AppConfigContext (settings + versions)
   - LocalizationContext (translations)
   ↓
7. Main app ready - NO MORE BOOTSTRAP CALLS
```

---

## 💾 Database Schema

### Entity Relationship Diagram

```
┌─────────────────┐
│      Users      │
└─────────────────┘
        (no direct relations - authentication only)

┌─────────────────┐
│    Products     │
└────────┬────────┘
         │
         │ 1:N
         │
         ├─────────────────┐
         │                 │
         ▼                 ▼
┌─────────────────┐  ┌─────────────────┐
│     History     │  │ Voice Commands  │
└──────────────���──┘  └─────────────────┘

┌─────────────────┐
│  Resource Keys  │  (independent - i18n)
└─────────────────┘

┌─────────────────┐
│   App Config    │  (independent - application settings)
└─────────────────┘
```

### Tables Overview

| Table Name | Purpose | Key Relations |
|------------|---------|---------------|
| `users` | Store user accounts for authentication | Independent |
| `app_config` | Store all application settings (VAT, company, etc.) | Independent |
| `products` | Store all warehouse products and inventory | Parent to `history` and `voice_commands` |
| `history` | Audit trail of all product changes | Foreign key to `products` |
| `voice_commands` | Log of all voice interactions and AI parsing | Foreign key to `products` (nullable) |
| `resource_keys` | Localization strings for UI | Independent |

---

### Table: `users`

**Purpose:** User authentication and authorization

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | UUID | Primary Key, Default: auto-generated | Unique user identifier |
| `username` | VARCHAR(50) | NOT NULL, UNIQUE | Username for login |
| `email` | VARCHAR(255) | NOT NULL, UNIQUE | User email address |
| `password_hash` | VARCHAR(255) | NOT NULL | Hashed password (bcrypt) |
| `role` | VARCHAR(20) | NOT NULL, Default: 'user' | User role (user, admin) |
| `is_active` | BOOLEAN | Default: true | Account active status |
| `created_at` | TIMESTAMP | Default: NOW() | Account creation timestamp |
| `updated_at` | TIMESTAMP | Default: NOW() | Last update timestamp |
| `last_login` | TIMESTAMP | NULL | Last login timestamp |

**Indexes:**
- Primary Key: `id`
- Unique Index on: `username`, `email`
- Index on: `role`, `is_active`

---

### Table: `app_config`

**Purpose:** Store all application configuration settings (single row table)

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | UUID | Primary Key, Default: auto-generated | Unique config identifier |
| `default_vat_rate` | DECIMAL(5,2) | NOT NULL | Default VAT rate percentage (e.g., 27.00) |
| `vat_number` | VARCHAR(50) | NULL | Company VAT/Tax identification number |
| `company_name` | VARCHAR(255) | NOT NULL | Company name |
| `currency` | VARCHAR(3) | NOT NULL | ISO 4217 currency code (HUF, EUR, USD) |
| `default_language` | VARCHAR(10) | NOT NULL | Default language code (hu-HU, en-US) |
| `database_version` | VARCHAR(20) | NOT NULL | Database schema version (e.g., 20260207001) |
| `created_at` | TIMESTAMP | Default: NOW() | Record creation timestamp |
| `updated_at` | TIMESTAMP | Default: NOW() | Last update timestamp |

**Indexes:**
- Primary Key: `id`

**Business Rules:**
- This is effectively a single-row table (only one active configuration)
- All settings loaded via `/api/bootstrap` on app startup
- Changes require admin privileges

**Example record:**

| default_vat_rate | vat_number | company_name | currency | default_language | database_version |
|------------------|------------|--------------|----------|------------------|------------------|
| 27.00 | HU12345678 | Your Company | HUF | hu-HU | 20260207001 |

---

### Table: `products`

**Purpose:** Main inventory table storing all product information

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | UUID | Primary Key, Default: auto-generated | Unique product identifier |
| `name` | VARCHAR(255) | NOT NULL | Product name |
| `description` | TEXT | NULL | Detailed product description |
| `quantity` | DECIMAL(10,2) | NOT NULL, Default: 0 | Current stock quantity |
| `unit` | VARCHAR(20) | NULL | Unit of measurement (kg, l, pieces, etc.) |
| `density` | DECIMAL(10,4) | NULL | Product density for weight/volume conversion |
| `density_unit` | VARCHAR(20) | NULL | Density unit (g/cm³, kg/l, etc.) |
| `warehouse_entry_date` | TIMESTAMP | NOT NULL | Date when product entered warehouse |
| `serial_number` | VARCHAR(100) | NULL | Product serial number |
| `article_number` | VARCHAR(100) | NULL | Product article/SKU number |
| `location` | VARCHAR(100) | NULL | Storage location (shelf, zone, etc.) |
| `net_price` | DECIMAL(10,2) | NULL | Net price (without tax) |
| `gross_price` | DECIMAL(10,2) | NULL | Gross price (with tax) |
| `voice_created` | BOOLEAN | Default: false | Flag indicating if created via voice command |
| `created_at` | TIMESTAMP | Default: NOW() | Record creation timestamp |
| `updated_at` | TIMESTAMP | Default: NOW() | Last update timestamp |

**Indexes:**
- Primary Key: `id`
- Index on: `name`, `article_number`, `serial_number`, `location`, `created_at`

**Notes:**
- Price calculation (net ↔ gross) is done on frontend using VAT rate from AppConfig
- Both net_price and gross_price are stored for data integrity

---

### Table: `history`

**Purpose:** Audit trail tracking all product changes and stock movements

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | UUID | Primary Key, Default: auto-generated | Unique history record identifier |
| `product_id` | UUID | NOT NULL, Foreign Key | Reference to products table |
| `action_type` | VARCHAR(50) | NOT NULL | Action type: created, updated, added, removed |
| `quantity_change` | DECIMAL(10,2) | NULL | Amount of quantity changed |
| `unit` | VARCHAR(20) | NULL | Unit of measurement for the change |
| `quantity_before` | DECIMAL(10,2) | NULL | Quantity before the action |
| `quantity_after` | DECIMAL(10,2) | NULL | Quantity after the action |
| `description` | TEXT | NULL | Additional description of the change |
| `created_at` | TIMESTAMP | Default: NOW() | When the action occurred |

**Foreign Keys:**
- `product_id` → `products.id` (ON DELETE CASCADE)

**Indexes:**
- Primary Key: `id`
- Index on: `product_id`, `created_at`, `action_type`

---

### Table: `voice_commands`

**Purpose:** Log all voice command interactions and AI parsing results

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | UUID | Primary Key, Default: auto-generated | Unique command identifier |
| `raw_transcript` | TEXT | NOT NULL | Original speech-to-text output |
| `parsed_intent` | JSONB | NULL | AI-parsed intent (JSON structure) |
| `action_taken` | VARCHAR(50) | NULL | Action executed: add, remove, update, search, delete |
| `product_id` | UUID | NULL, Foreign Key | Reference to affected product (if any) |
| `locale` | VARCHAR(10) | NULL | Language code used (en-US, hu-HU) |
| `success` | BOOLEAN | Default: true | Whether command executed successfully |
| `error_message` | TEXT | NULL | Error details if command failed |
| `processing_time_ms` | INTEGER | NULL | Time taken to process command (milliseconds) |
| `created_at` | TIMESTAMP | Default: NOW() | Command timestamp |

**Foreign Keys:**
- `product_id` → `products.id` (ON DELETE SET NULL)

**Indexes:**
- Primary Key: `id`
- Index on: `created_at`, `locale`, `action_taken`, `success`
- GIN Index on: `parsed_intent` (for JSONB queries)

---

### Table: `resource_keys`

**Purpose:** Store all UI text translations for internationalization

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | UUID | Primary Key, Default: auto-generated | Unique resource key identifier |
| `key` | VARCHAR(255) | NOT NULL | Resource key (e.g., UI_Product_Name) |
| `locale` | VARCHAR(10) | NOT NULL | Language code (en-US, hu-HU) |
| `value` | TEXT | NOT NULL | Translated text value |
| `category` | VARCHAR(50) | NULL | Category: product, voice, common, menu, etc. |
| `description` | TEXT | NULL | Description for translators |
| `created_at` | TIMESTAMP | Default: NOW() | Record creation timestamp |
| `updated_at` | TIMESTAMP | Default: NOW() | Last update timestamp |

**Unique Constraints:**
- Unique combination of (`key`, `locale`)

**Indexes:**
- Primary Key: `id`
- Unique Index on: (`key`, `locale`)
- Index on: `locale`, `category`

---

### Database Views

#### View: `product_statistics`

**Purpose:** Aggregate product transaction statistics

**Columns:**
- `id` - Product ID
- `name` - Product name
- `quantity` - Current quantity
- `unit` - Unit of measurement
- `location` - Storage location
- `transaction_count` - Total number of transactions
- `total_added` - Total quantity added
- `total_removed` - Total quantity removed
- `last_transaction_date` - Most recent transaction

---

#### View: `voice_analytics`

**Purpose:** Voice command usage and performance metrics

**Columns:**
- `locale` - Language code
- `action_taken` - Command action type
- `command_count` - Total commands
- `avg_processing_time` - Average processing time
- `success_count` - Successful commands
- `error_count` - Failed commands
- `success_rate` - Percentage of successful commands

---

### Data Relationships

```
users (independent - authentication only)

app_config (independent - single row configuration)

products (1) ──────< (N) history
   │
   │ (optional)
   │
   └──────< (N) voice_commands

resource_keys (independent - no foreign keys)
```

---

## 🚀 Installation

### Prerequisites

- **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+** and **npm** - [Download](https://nodejs.org/)
- **PostgreSQL 15+** - [Download](https://www.postgresql.org/download/) OR [Supabase](https://supabase.com) account
- **Azure Speech Services** - [Free tier](https://azure.microsoft.com/en-us/services/cognitive-services/speech-services/)
- **Google Cloud Account** with Gemini API access - [Google AI Studio](https://ai.google.dev/)
- **Git** - [Download](https://git-scm.com/)

### 1️⃣ Clone Repository

```bash
git clone https://github.com/OmniStruo/EchoWarehouse.git
cd EchoWarehouse
```

### 2️⃣ Backend Setup

```bash
cd backend/EchoWarehouse.API

# Install NuGet packages
dotnet restore

# Configure environment variables
# Create appsettings.Development.json (NOT in git)
```

**Create `appsettings.Development.json`:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=echowarehouse;Username=postgres;Password=yourpassword"
  },
  "Authentication": {
    "RegistrationSecret": "your-super-secret-registration-code-here",
    "JwtSecret": "your-jwt-secret-key-minimum-32-characters",
    "JwtIssuer": "EchoWarehouse",
    "JwtAudience": "EchoWarehouse",
    "JwtExpirationHours": 24
  },
  "AzureSpeech": {
    "SubscriptionKey": "YOUR_AZURE_SPEECH_KEY",
    "Region": "westeurope"
  },
  "GoogleGemini": {
    "ApiKey": "YOUR_GOOGLE_GEMINI_API_KEY",
    "ProjectId": "your-project-id",
    "Location": "us-central1",
    "Model": "gemini-1.5-flash"
  },
  "Application": {
    "Version": "1.0.0"
  }
}
```

**Add to `.gitignore`:**
```
appsettings.Development.json
.env
*.env
!.env.example
```

```bash
# Run migrations
dotnet ef database update

# Seed initial data (creates default AppConfig + localization)
dotnet run --seed-data

# Run application
dotnet run
```

Backend available at: `https://localhost:5001`

**Scalar API documentation available at:** `https://localhost:5001/scalar`

### 3️⃣ Frontend Setup

```bash
cd ../../frontend

# Install dependencies
npm install

# Environment variables
cp .env.example .env
```

**Edit `.env`:**
```env
VITE_API_URL=https://localhost:5001
VITE_ENABLE_VOICE=true
```

```bash
# Start development server
npm run dev
```

Frontend available at: `http://localhost:5173`

### 4️⃣ Create First User

**Using Scalar API:**

1. Navigate to `https://localhost:5001/scalar`
2. Find `POST /api/auth/register`
3. Click "Try it out"
4. Fill in the request body with registration secret
5. Execute

### 5️⃣ Login and Bootstrap

After creating a user:
1. Open frontend at `http://localhost:5173`
2. Login with your credentials
3. App automatically calls `/api/bootstrap`
4. All settings and translations loaded in one request

---

## ⚙️ Configuration

### Backend Configuration (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=echowarehouse;Username=postgres;Password=yourpassword;SSL Mode=Prefer"
  },
  "Authentication": {
    "RegistrationSecret": "SET_IN_ENVIRONMENT_VARIABLE",
    "JwtSecret": "SET_IN_ENVIRONMENT_VARIABLE",
    "JwtIssuer": "EchoWarehouse",
    "JwtAudience": "EchoWarehouse",
    "JwtExpirationHours": 24
  },
  "AzureSpeech": {
    "SubscriptionKey": "YOUR_AZURE_SPEECH_KEY",
    "Region": "westeurope"
  },
  "GoogleGemini": {
    "ApiKey": "YOUR_GOOGLE_GEMINI_API_KEY",
    "ProjectId": "your-project-id",
    "Location": "us-central1",
    "Model": "gemini-1.5-flash"
  },
  "Application": {
    "Version": "1.0.0"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",
      "https://yourapp.web.app"
    ]
  },
  "Swagger": {
    "Enabled": true,
    "Title": "EchoWarehouse API",
    "Version": "v1",
    "Description": "AI-Powered Voice-Controlled Warehouse Management System API"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  }
}
```

### Environment Variables (Production - REQUIRED)

**⚠️ NEVER commit these to git!**

```bash
# Database
ConnectionStrings__DefaultConnection="Host=your-db-host;Database=echowarehouse;Username=user;Password=pass"

# Authentication (CRITICAL - KEEP SECRET)
Authentication__RegistrationSecret="your-super-secret-code"
Authentication__JwtSecret="your-jwt-secret-minimum-32-chars"

# External Services
AzureSpeech__SubscriptionKey="..."
GoogleGemini__ApiKey="..."

# Application
Application__Version="1.0.0"
```

### Frontend Configuration (`.env`)

```env
VITE_API_URL=https://localhost:5001
VITE_ENABLE_VOICE=true
```

---

## 🔐 Authentication

### Registration Flow

**Endpoint:** `POST /api/auth/register`

**Request:**
```json
{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecurePassword123!",
  "registrationSecret": "your-super-secret-registration-code"
}
```

**Response (Success):**
```json
{
  "success": true,
  "message": "User registered successfully",
  "userId": "550e8400-e29b-41d4-a716-446655440000"
}
```

---

### Login Flow

**Endpoint:** `POST /api/auth/login`

**Request:**
```json
{
  "username": "johndoe",
  "password": "SecurePassword123!"
}
```

**Response (Success):**
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2026-02-08T10:30:00Z",
  "user": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "username": "johndoe",
    "email": "john@example.com",
    "role": "user"
  }
}
```

**Frontend Flow:**
1. User enters credentials
2. POST to `/api/auth/login`
3. Receive JWT token
4. Store token in LocalStorage
5. Call `/api/bootstrap` (includes token)
6. Load all configuration and translations
7. Show main app

---

## 🚀 Bootstrap API

### Overview

The `/api/bootstrap` endpoint is the **single source of truth** for application initialization. It returns all necessary configuration, translations, and version information in one API call.

### Endpoint

**`GET /api/bootstrap?locale={locale}`**

**Authentication:** Required (JWT Bearer token)

**Query Parameters:**
- `locale` (optional) - Language code (e.g., `en-US`, `hu-HU`). Defaults to app config default language.

### Response Structure

```json
{
  "appSettings": {
    "defaultVatRate": 27.00,
    "vatNumber": "HU12345678",
    "companyName": "Your Company",
    "currency": "HUF",
    "defaultLanguage": "hu-HU"
  },
  "localization": {
    "UI_App_Title": "EchoWarehouse",
    "UI_Auth_Login": "Bejelentkezés",
    "UI_Product_Name": "Termék neve",
    "UI_Product_Quantity": "Mennyiség",
    "UI_Product_NetPrice": "Nettó ár",
    "UI_Product_GrossPrice": "Bruttó ár",
    "UI_Voice_Listening": "Hallgatlak...",
    "UI_Button_Save": "Mentés",
    "UI_Message_Success": "Sikeres művelet"
  },
  "version": {
    "backend": "1.0.0",
    "database": "20260207001"
  },
  "supportedLocales": [
    {
      "code": "en-US",
      "name": "English",
      "nativeName": "English"
    },
    {
      "code": "hu-HU",
      "name": "Hungarian",
      "nativeName": "Magyar"
    }
  ],
  "loadedAt": "2026-02-07T10:30:00Z"
}
```

### Example Request

```http
GET /api/bootstrap?locale=hu-HU
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## 📖 Usage

### First Time Setup

1. **Backend running** with database migrated and seeded
2. **Open frontend** at `http://localhost:5173`
3. **Register** with registration secret
4. **Login** with your credentials
5. **App calls `/api/bootstrap`** automatically
6. **All settings loaded** - ready to use!

### Daily Usage

1. **Open app** at `http://localhost:5173`
2. **Login** if not already authenticated
3. **Bootstrap loads:**
   - App settings (VAT, company info, currency)
   - All translations for selected language
   - Version information
4. **Use the app:**
   - Voice commands or manual entry
   - Prices auto-calculate with VAT rate from config
   - UI in your selected language
5. **Change language** - reloads translations from cached bootstrap data

### VAT Calculation

Frontend uses VAT rate from AppConfig (loaded via bootstrap):

**Formula:**
```typescript
// From useVat hook
const { defaultVatRate } = useAppConfig();

// Net to Gross
const calculateGross = (net: number) => {
  return net * (1 + defaultVatRate / 100);
};

// Gross to Net
const calculateNet = (gross: number) => {
  return gross / (1 + defaultVatRate / 100);
};
```

**Example:**
- VAT Rate: 27%
- Net Price: 1000
- Gross Price: 1000 × 1.27 = 1270

---

## 🔌 API Documentation

### Base URL
```
Development: https://localhost:5001/api
Production: https://your-api.azurewebsites.net/api
```

For full interactive API documentation, visit **[Scalar API](/scalar)**.

### API Endpoints Overview

#### **Bootstrap (Protected - Called ONCE on app startup)**

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/bootstrap?locale={locale}` | Get all app config, translations, and version info |

#### **Authentication (Public)**

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user (requires secret) |
| POST | `/api/auth/login` | Login and get JWT token |

#### **Authentication (Protected)**

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/logout` | Logout current user |
| GET | `/api/auth/me` | Get current user info |
| PUT | `/api/auth/change-password` | Change password |

#### **Settings (Protected - Admin Only)**

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/settings/config` | Get current app configuration |
| PUT | `/api/settings/config` | Update app configuration |

#### **Products (Protected)**

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | Get all products (paginated) |
| GET | `/api/products/{id}` | Get single product by ID |
| POST | `/api/products` | Create new product |
| PUT | `/api/products/{id}` | Update product |
| DELETE | `/api/products/{id}` | Delete product |
| GET | `/api/products/search?q={query}` | Search products |
| POST | `/api/products/{id}/stock` | Stock operation (add/remove) |

#### **Voice (Protected)**

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/voice/process` | Process voice command |
| GET | `/api/voice/history` | Get voice command history |
| GET | `/api/voice/analytics` | Get voice analytics |

#### **History (Protected)**

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/history` | Get all operations |
| GET | `/api/history/product/{id}` | Get product history |

#### **Health (Public)**

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/health` | Health check endpoint |

---

## 🎤 Voice Commands

### Supported Command Patterns

#### Add Item (English)
```
"Add [quantity] [unit] [product name] to [location]"
"Add 100 pieces of screws"
"New product: 5 liters of paint"
```

#### Add Item (Hungarian)
```
"Vegyél fel [mennyiség] [egység] [termék név] a [lokáció] polcra"
"Adj hozzá 100 darab csavart"
"Új termék: 5 liter festék"
```

---

## 🌐 Internationalization

### How It Works

1. **Database Storage**: All UI text stored as resource keys
2. **Bootstrap Load**: Translations loaded via `/api/bootstrap`
3. **Context Storage**: Stored in LocalizationContext
4. **Simple Hook**: `useLocalization()` provides `t()` function
5. **Language Switching**: Fetches new locale via bootstrap

### Supported Locales

- **en-US** - English (United States)
- **hu-HU** - Hungarian (Hungary)

---

## 📁 Project Structure

### Backend

```
EchoWarehouse/
├── backend/
│   ├── EchoWarehouse.API/
│   │   ├── Controllers/
│   │   │   ├── BootstrapController.cs      # Bootstrap endpoint
│   │   │   ├── AuthController.cs
│   │   │   ├── ProductsController.cs
│   │   │   ├── VoiceController.cs
│   │   │   ├── HistoryController.cs
│   │   │   ├── SettingsController.cs       # Admin settings
│   │   │   └── HealthController.cs
│   │   ├── Services/
│   │   │   ├── Interfaces/
│   │   │   │   ├── IBootstrapService.cs
│   │   │   │   ├── IAuthService.cs
│   │   │   │   ├── IAppConfigService.cs
│   │   │   │   ├── ILocalizationService.cs
│   │   │   │   └── ...
│   │   │   └── Implementations/
│   │   │       ├── BootstrapService.cs
│   │   │       ├── AuthService.cs
│   │   │       ├── AppConfigService.cs
│   │   │       ├── LocalizationService.cs
│   │   │       └── ...
│   │   ├── Repositories/
│   │   │   ├── Interfaces/
│   │   │   │   ├── IAppConfigRepository.cs
│   │   │   │   ├── IUserRepository.cs
│   │   │   │   └── ...
│   │   │   └── Implementations/
│   │   │       ├── AppConfigRepository.cs
│   │   │       ├── UserRepository.cs
│   │   │       └── ...
│   │   ├── Models/
│   │   │   ├── Entities/
│   │   │   │   ├── AppConfig.cs
│   │   │   │   ├── User.cs
│   │   │   │   ├── Product.cs
│   │   │   │   └── ...
│   │   │   ├── DTOs/
│   │   │   │   ├── BootstrapResponseDto.cs
│   │   │   │   ├── AppSettingsDto.cs
│   │   │   │   └── ...
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json    # NOT in git
│   │   ├── .gitignore
│   │   └── Program.cs
```

### Frontend

```
frontend/
├── src/
│   ├── components/
│   │   ├── auth/
│   │   ├── voice/
│   │   ├── products/
│   │   ├── settings/                        # Admin settings UI
│   │   │   └── AppConfigForm.tsx
│   │   └── common/
│   ├── services/
│   │   ├── bootstrapService.ts              # Bootstrap API call
│   │   ├── authService.ts
│   │   └── ...
│   ├── hooks/
│   │   ├── useAuth.ts
│   │   ├── useAppConfig.ts                  # Access app settings
│   │   ├── useLocalization.ts
│   │   ├── useVat.ts                        # VAT calculations
│   │   └── ...
│   ├── context/
│   │   ├── AuthContext.tsx
│   │   ├── AppConfigContext.tsx             # App settings + version
│   │   ├── LocalizationContext.tsx
│   │   └── ...
│   ├── utils/
│   │   ├── vatCalculator.ts
│   │   └── ...
│   ├── App.tsx
│   └── main.tsx
```

---

## 🚢 Deployment

### Backend - Azure App Service

```bash
# Configure environment variables including version
az webapp config appsettings set \
  --name echowarehouse-api \
  --resource-group EchoWarehouse-RG \
  --settings \
    ConnectionStrings__DefaultConnection="Host=..." \
    Authentication__RegistrationSecret="..." \
    Authentication__JwtSecret="..." \
    Application__Version="1.0.0"
```

### Docker Deployment

**.env file:**
```env
POSTGRES_PASSWORD=your-password
REGISTRATION_SECRET=your-secret
JWT_SECRET=your-jwt-secret
AZURE_SPEECH_KEY=your-key
GOOGLE_GEMINI_KEY=your-key
APPLICATION_VERSION=1.0.0
```

---

## 📄 License

This project is licensed under the MIT License.

---

## 👤 Author

**OmniStruo Team**

- GitHub: [@OmniStruo](https://github.com/OmniStruo)
- Project: [EchoWarehouse](https://github.com/OmniStruo/EchoWarehouse)

