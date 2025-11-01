# Line Management System for Telecom Providers

## Overview

A Windows desktop application (WPF) for managing mobile phone lines for Egyptian telecom providers (Vodafone, Etisalat, WE, Orange). The system manages groups of lines with automatic renewal tracking, customer delivery management, and barcode scanning support.

**Critical Note**: This is a WPF application that runs exclusively on Windows. It cannot be executed on Replit or Linux-based environments.

## User Preferences

Preferred communication style: Simple, everyday language.

## System Architecture

### Frontend Architecture
- **Framework**: WPF (Windows Presentation Foundation) - .NET desktop UI framework
- **Platform**: Windows-only (Windows 10 or newer)
- **Language**: C# with XAML for UI markup
- **UI Design**: Color-coded interfaces for different telecom providers (Vodafone=Red, Etisalat=Green, WE=Purple, Orange=Orange)

### Core Business Logic

**Group Management System**
- Maximum capacity: 50 lines per group
- Multi-provider support: 4 Egyptian telecom companies
- Line states: Active, Suspended, Blocked, Without Cash Wallet, With Cash Wallet
- Each group tracks: responsible employee, customer assignment, expected delivery date

**Line Entity Structure**
- Person name
- National ID (14 digits)
- Phone number
- Internal ID
- Cash wallet flag (yes/no)
- Wallet number (optional)

**Automatic Renewal System**
- 60-day renewal cycle for groups with cash wallets
- Notification system:
  - Alert 7 days before renewal due date
  - Alert on expiration date
  
**Delivery Tracking System**
- Customer assignment per group
- Expected delivery date tracking
- Notification system:
  - Alert 3 days before delivery date
  - Alert on overdue deliveries

### Input Method Support
- **Hardware Integration**: USB barcode scanner support (XB-2055 USB 1D Barcode Scanner)
- Auto-field navigation on Enter key press
- Rapid line data entry workflow

### Data Architecture
- Entity relationships: Groups (1:Many) Lines
- Group capacity constraint: 50 lines maximum
- National ID validation: 14-digit format
- Optional wallet number field (conditional on cash wallet flag)

## External Dependencies

### Hardware
- **XB-2055 USB 1D Barcode Scanner** - For rapid line data entry via barcode/QR code scanning

### Platform Requirements
- Windows operating system (Windows 10 or newer)
- .NET Framework/Runtime for WPF applications

### Data Storage
- **Database**: SQLite (local database file: linemanagement.db)
- **ORM**: Entity Framework Core 8.0
- **Entities**: LineGroup, PhoneLine, Alert
- **Relationships**: Groups (1:Many) Lines, Groups (1:Many) Alerts

### Third-Party Packages
- Microsoft.EntityFrameworkCore.Sqlite (8.0.0) - Database ORM
- Newtonsoft.Json (13.0.3) - JSON serialization
- MaterialDesignThemes (5.0.0) - Material Design UI components

## Project Structure

```
LineManagementSystem/
├── src/
│   ├── Models/              # Domain models (TelecomProvider, LineGroup, PhoneLine, Alert)
│   ├── Services/            # Business logic services (DatabaseContext, AlertService, GroupService)
│   ├── ViewModels/          # MVVM ViewModels with INPC
│   ├── Views/               # XAML views (MainWindow, ProviderGroupsWindow, GroupDetailsWindow)
│   ├── Resources/           # Images and styles
│   └── Converters/          # Value converters for data binding
├── App.xaml                 # Application entry point
├── LineManagementSystem.csproj
└── README.md
```

## Recent Changes (November 1, 2025)

- ✅ Implemented complete WPF application with MVVM pattern
- ✅ Added SQLite database with Entity Framework Core
- ✅ Created alert service with automatic 5-minute checks
- ✅ Built color-coded UI for 4 telecom providers
- ✅ Implemented QR scanner support with Enter key navigation
- ✅ Added 60-day renewal tracking system
- ✅ Implemented delivery tracking with alerts
- ✅ Added build validation workflow (compiles on Linux via EnableWindowsTargeting)
- ✅ Documented Windows-only execution requirements

## Build and Deployment

### On Replit (Build Validation Only)
- A workflow validates the code compiles successfully
- `dotnet build` runs successfully with EnableWindowsTargeting flag
- **Note**: The application cannot RUN on Replit (Linux), only build validation

### On Windows (Actual Execution)
1. Install .NET 8.0 SDK or Runtime
2. Clone/download the project
3. Run: `dotnet restore && dotnet build && dotnet run`
4. Application will launch with WPF UI

## Known Limitations
- Windows-only (WPF dependency)
- Cannot run on Replit or any Linux/Mac environment
- Requires Windows 10 or newer
- Barcode scanner (XB-2055) only works on Windows with proper drivers