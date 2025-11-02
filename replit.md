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
- Business Group type: Special group type with confirmation tracking (1-3 checkmarks per line)
- Additional details field (optional, up to 1000 characters) for extra group information

**Line Entity Structure**
- Person name
- National ID (14 digits)
- Phone number
- Internal ID
- Cash wallet flag (yes/no)
- Wallet number (optional)
- Details field (optional, up to 500 characters)
- Confirmation level (0-3) for business groups

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

## Recent Changes

### November 2, 2025 (Evening Update - Latest)
- ✅ **Enhanced details display in tables**: 
  - Added "التفاصيل الإضافية" column (250px width) in Groups table to show AdditionalDetails field
  - Expanded "التفاصيل" column (250px width) in Lines table for better visibility
  - Added text wrapping and tooltips for easy reading
  - Increased row height (Groups: 60→80px, Lines: 40→60px) for multi-line text display
- ✅ **Added Enter key shortcut in Groups Window**: Press Enter on selected group → Opens group details window automatically
- ✅ **Auto-focus on Phone Number field**: When opening line entry form, cursor automatically goes to Phone Number field (instead of Name)
- ✅ **Fixed Entity Framework tracking bug**: Added ChangeTracker.Clear() in UpdateLine to prevent duplicate entity tracking errors
- ✅ **Improved Enter key behavior for QR scanner workflow**:
  - Name field → Enter → National ID field
  - National ID field → Enter → Phone Number field
  - Phone Number field → Enter → Internal ID field
  - Internal ID field → Enter → **Auto-save line** (perfect for rapid data entry)
  - Cash Wallet Number field → Enter → Back to **Phone Number field** (for next line)

### November 2, 2025 (Earlier)
- ✅ Added automatic post-build copy of images to output directory (solves icon visibility issue when running exe)
- ✅ Improved alert display with warning icons (⚠) instead of checkmarks
- ✅ Enhanced alert UI with proper hide button and better visibility
- ✅ Completely redesigned DataGrid in ProviderGroupsWindow with modern styling
- ✅ Added color-coded cells and badges for better data visualization
- ✅ Improved row height and spacing for better readability
- ✅ Fixed resource pack URIs to ensure icons load correctly in all windows
- ✅ Added proper Link elements in csproj to maintain correct resource logical names
- ✅ **Fixed WPF Binding errors**: Converted all computed methods to properties for proper data binding
  - LineGroup: GetLineCount, GetRemainingDaysForRenewal, CanAddMoreLines, etc.
  - Alert: GetArabicMessage, GetColorBrush, GetColorHex
- ✅ Updated all service and ViewModel code to use properties instead of methods

### November 1, 2025
- ✅ Implemented complete WPF application with MVVM pattern
- ✅ Added SQLite database with Entity Framework Core
- ✅ Created alert service with automatic 5-minute checks
- ✅ Built color-coded UI for 4 telecom providers
- ✅ Implemented QR scanner support with Enter key navigation
- ✅ Added 60-day renewal tracking system
- ✅ Implemented delivery tracking with alerts
- ✅ Added build validation workflow (compiles on Linux via EnableWindowsTargeting)
- ✅ Documented Windows-only execution requirements
- ✅ Added Details field for phone lines (up to 500 characters)
- ✅ Added AdditionalDetails field for groups (up to 1000 characters)
- ✅ Implemented Business Group feature with confirmation levels (0-3 checkmarks)
- ✅ Added confirmation tracking UI with color-coded buttons
- ✅ Fixed duplicate assembly attribute errors
- ✅ Fixed image resource paths (removed /src/ prefix)
- ✅ Added ProviderToColorConverter for dynamic provider colors
- ✅ Implemented colored headers in all provider windows (Vodafone=Red, Etisalat=Green, WE=Purple, Orange=Orange)
- ✅ Improved text contrast with white text on colored backgrounds
- ✅ Enhanced visual design with shadows and borders

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