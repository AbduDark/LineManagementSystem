# Line Management System for Telecom Providers

## Overview
This project is a Windows desktop application (WPF) designed to manage mobile phone lines for Egyptian telecom providers (Vodafone, Etisalat, WE, Orange). Its primary purpose is to organize groups of lines, track automatic renewals, manage customer deliveries, and support barcode scanning for efficient data entry. The system aims to streamline line management operations for telecom agents.

## User Preferences
Preferred communication style: Simple, everyday language.

## System Architecture

### Frontend
- **Framework**: WPF (.NET desktop UI framework)
- **Platform**: Windows-only (Windows 10 or newer)
- **Language**: C# with XAML
- **UI Design**: Color-coded interfaces per telecom provider (Vodafone=Red, Etisalat=Green, WE=Purple, Orange=Orange). Utilizes MaterialDesignThemes for UI components and supports light/dark modes.

### Core Business Logic
- **Group Management**: Supports up to 50 lines per group across four telecom providers. Lines have states (Active, Suspended, Blocked, With/Without Cash Wallet). Groups track responsible employee, customer, and expected delivery date. A "Business Group" type includes confirmation tracking (0-3 levels per line).
- **Line Entity**: Stores person name, national ID (14 digits), phone number, internal ID, cash wallet flag (with optional wallet number), line system field, and details field.
- **Automatic Renewal**: Tracks 60-day renewal cycles for cash wallet groups with 7-day and expiration day alerts.
- **Delivery Tracking**: Manages customer assignment and expected delivery dates with 3-day pre-delivery and overdue alerts.
- **Backup & Restore**: Manual and automatic (24-hour) backup system with restore functionality and cleanup of old backups.
- **Reporting**: Export functionality for lines and groups to Excel and PDF formats, including provider-specific branding and statistics.
- **Excel Import**: Smart import from Excel files with intelligent column detection supporting Arabic/English headers (with or without headers). Auto-validates national IDs (14 digits) and phone numbers (11 digits starting with 01). Internal ID generated from row number.

### Data Architecture
- **Relationships**: Groups can have many Lines (1:Many).
- **Validation**: National ID (14-digit format) and conditional wallet number.

### Input Methods
- **Hardware Integration**: Supports USB 1D barcode scanners (e.g., XB-2055) for rapid line data entry.
- **Workflow**: Optimized data entry with auto-field navigation and auto-save on Enter key press.

### System Design
- **MVVM Pattern**: Utilizes Model-View-ViewModel for separation of concerns.
- **Theming**: Comprehensive light and dark mode support across all windows with dynamic resource binding.
- **Error Handling**: Improved delete confirmation and error messages.

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

## Replit Environment Status

⚠️ **Important**: This is a Windows-only WPF application that **cannot run on Replit** (Linux environment). However, the project has been successfully imported and builds without errors.

### Development Environment Setup
- **.NET 8.0 SDK**: Installed and configured
- **Build Status**: ✅ Builds successfully with 0 errors, 0 warnings
- **NuGet Packages**: All dependencies restored successfully

### Known Limitations on Replit
- The application cannot be executed on Replit (requires Windows)
- LSP errors appear for WPF types (expected on Linux, does not affect build)
- No workflow/preview available (Windows desktop app)

### To Run This Application
1. Download the project from Replit
2. Open on a Windows machine (Windows 10 or newer)
3. Open in Visual Studio 2022 or VS Code
4. Run with `dotnet run` or press F5 in Visual Studio

## Recent Changes (November 2025)

### Latest Fixes (November 5, 2025 - Replit Import)
- **MaterialDesign Resource Fix**: Replaced MaterialDesign styles in ImportSettingsDialog.xaml with custom ModernButton styles to match the rest of the application
  - Removed `materialDesign:Card` component, replaced with standard Border with ModernCard style
  - Removed MaterialDesign button styles, using ModernButton from Styles.xaml instead
  - Removed unused MaterialDesign namespace references
  - Build now succeeds with 0 errors and 0 warnings

### New Features (November 5, 2025)
- **Line System Field**: Added new "LineSystem" field to PhoneLine model to track line system information
  - Added LineSystem property to PhoneLine model (max 100 characters)
  - Added "نظام الخط" column in GroupDetailsWindow DataGrid
  - Added "نظام الخط" input field in add/edit line form
  - Added LineSystem support in Excel import (ImportSettingsDialog and ImportService)
  - Added LineSystem column in Excel export (ExportService)
  - Field is optional and can be left empty

### Bug Fixes
- **Line Deletion Fix**: Fixed issue where phone lines were not being deleted properly. Added ChangeTracker.Clear() before ExecuteDelete in GroupService.DeleteLine() to ensure clean deletion without tracking conflicts.
- **NullReferenceException Fix**: Fixed crash when deleting groups or lines by storing group name before LoadGroups() call.
- **Build Fix**: Fixed compilation error in ExportService.cs (exportLines variable) and SearchWindow.xaml.cs (nullability issue)

### Previous Features
- **Custom Excel Import Settings Dialog**:
  - Created ImportSettingsDialog.xaml with comprehensive column selection interface
  - Users can now customize which columns contain specific data using checkboxes
  - Supports both required fields (Name, National ID, Phone Number) and optional fields (Internal ID, Cash Wallet status, Wallet Number)
  - Shows column previews from the Excel file for easier selection
  - Validates all required selections before import
  - Added ImportService.ImportFromExcelWithCustomSettings() method for custom column mapping
  - Supports custom boolean parsing for cash wallet (نعم/لا, yes/no, 1/0, true/false)

- **Excel Import System**: 
  - Added ImportService with smart column detection algorithm
  - Supports files with or without headers
  - Detects Arabic and English column names (اسم, Name, الرقم القومي, National ID, رقم, Phone, Mobile, etc.)
  - Auto-validates national IDs (14 digits) and phone numbers (11 digits starting with 01)
  - Normalizes phone numbers (removes spaces, dashes, +2 prefix)
  - Generates Internal ID from row number
  - Provides detailed import results with error reporting
  - Added "📥 استيراد من Excel" button in GroupDetailsWindow
  - GroupService.ImportLines() validates max lines limit before importing