# Line Management System for Telecom Providers

## Overview
This project is a Windows desktop application (WPF) designed to manage mobile phone lines for Egyptian telecom providers (Vodafone, Etisalat, WE, Orange). Its primary purpose is to organize groups of lines, track automatic renewals, manage customer deliveries, and support barcode scanning for efficient data entry. The system aims to streamline line management operations for telecom agents, offering features like comprehensive search, flexible Excel import/export, and robust data management.

## User Preferences
Preferred communication style: Simple, everyday language.

## System Architecture

### UI/UX Decisions
- **Framework**: WPF (.NET desktop UI framework)
- **Language**: C# with XAML
- **UI Design**: Color-coded interfaces per telecom provider (Vodafone=Red, Etisalat=Green, WE=Purple, Orange=Orange). Utilizes MaterialDesignThemes for UI components and supports light/dark modes.
- **Input Methods**: Optimized data entry with auto-field navigation and auto-save on Enter key press.

### Technical Implementations
- **Core Business Logic**:
    - **Group Management**: Supports unlimited lines per group across four telecom providers. Lines have states (Active, Suspended, Blocked, With/Without Cash Wallet). Groups track responsible employee, customer, and expected delivery date. A "Business Group" type includes confirmation tracking.
    - **Line Entity**: Stores person name, national ID (14 digits), phone number, internal ID, cash wallet flag (with optional wallet number), line system field, and details field.
    - **Automatic Renewal**: Tracks 60-day renewal cycles for cash wallet groups with 7-day and expiration day alerts.
    - **Delivery Tracking**: Manages customer assignment and expected delivery dates with 3-day pre-delivery and overdue alerts.
    - **Backup & Restore**: Manual and automatic (24-hour) backup system with restore functionality and cleanup of old backups.
    - **Reporting**: Export functionality for lines and groups to Excel and PDF formats, including provider-specific branding and statistics.
    - **Excel Import**: Smart import from Excel files with intelligent column detection supporting Arabic/English headers (with or without headers). Supports flexible column mapping and validates national IDs (14 digits) and phone numbers (11 digits starting with 01).
    - **Search Functionality**: Comprehensive and advanced search filters for groups and lines, including real-time search, provider, status, and cash wallet filters. Search applies consistently to standalone lines, group-owned lines, and group rows themselves. Text predicates work across all search types (Name, National ID, Phone Number, Internal ID, Cash Wallet Number, Line System, Details).
- **System Design**:
    - **MVVM Pattern**: Utilizes Model-View-ViewModel for separation of concerns.
    - **Theming**: Comprehensive light and dark mode support across all windows with dynamic resource binding.
    - **Error Handling**: Improved delete confirmation and error messages.

### Feature Specifications
- **Line System Field**: Added "LineSystem" field to `PhoneLine` model for tracking system information, supported in forms, import, and export.
- **Unlimited Lines**: Removed previous limits on the number of lines per group. Database migration handles removal of MaxLines column with proper SQLite foreign key constraint management (disable FK → rebuild table in transaction → re-enable FK).

## External Dependencies

### Hardware
- **XB-2055 USB 1D Barcode Scanner**: For barcode/QR code scanning.

### Platform Requirements
- **Windows Operating System**: Windows 10 or newer.
- **.NET Framework/Runtime**: .NET 8.0 for WPF applications.

### Data Storage
- **Database**: SQLite (local `linemanagement.db` file).
- **ORM**: Entity Framework Core 8.0.
- **Entities**: LineGroup, PhoneLine, Alert.

### Third-Party Packages
- **Microsoft.EntityFrameworkCore.Sqlite (8.0.0)**: ORM for SQLite.
- **Newtonsoft.Json (13.0.3)**: JSON serialization.
- **MaterialDesignThemes (5.0.0)**: Material Design UI components.
- **ClosedXML (0.104.2)**: Excel import/export functionality.
- **QuestPDF (2025.1.0)**: PDF report generation.
- **LiveChartsCore.SkiaSharpView.WPF (2.0.0-rc4.5)**: Charts and data visualization for dashboard.

## Recent Changes (November 9, 2025)
- Successfully imported project to Replit environment
- Installed .NET 8.0 toolchain and all required NuGet packages
- Added missing LiveChartsCore.SkiaSharpView.WPF package dependency
- Fixed GroupStatus enum reference (changed `Blocked` to `Barred` in DashboardViewModel)
- Build validation workflow configured and passing successfully
- Note: This is a Windows WPF application and cannot run directly on Replit's Linux environment. The project is set up for code editing, building, and development work only.