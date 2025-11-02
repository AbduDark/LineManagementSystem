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
- **Line Entity**: Stores person name, national ID (14 digits), phone number, internal ID, cash wallet flag (with optional wallet number), and details field.
- **Automatic Renewal**: Tracks 60-day renewal cycles for cash wallet groups with 7-day and expiration day alerts.
- **Delivery Tracking**: Manages customer assignment and expected delivery dates with 3-day pre-delivery and overdue alerts.
- **Backup & Restore**: Manual and automatic (24-hour) backup system with restore functionality and cleanup of old backups.
- **Reporting**: Export functionality for lines and groups to Excel and PDF formats, including provider-specific branding and statistics.

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
- **ClosedXML (0.104.2)**: Excel export functionality.
- **QuestPDF (2025.1.0)**: PDF report generation.