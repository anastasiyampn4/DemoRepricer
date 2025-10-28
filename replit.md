# Wildberries Repricer - Information System

## Overview

This is a demonstration application built with C# (.NET 8.0) that automates price management (repricing) for coffee products on the Wildberries marketplace. The system implements multiple repricing strategies to help sellers maintain competitive pricing while protecting profit margins. It monitors competitor prices, applies configurable pricing rules, and maintains a complete audit trail of price changes.

The application is designed as a course project showcasing automated pricing strategies for e-commerce platforms, with features for product catalog management, competitor monitoring, automated repricing, price history tracking, and profitability analytics.

## User Preferences

Preferred communication style: Simple, everyday language.

## Recent Changes (October 22, 2025)

### Completed Implementation
- ✅ Full C# repricing system with SQLite database
- ✅ Five repricing strategies implemented and tested
- ✅ Console demo application running successfully  
- ✅ Windows Forms UI code created (requires Windows to run)
- ✅ Sample data with 5 coffee products and competitive pricing
- ✅ Price history tracking and analytics
- ✅ Fixed critical bugs in MaintainMargin strategy and rounding consistency

### Architecture Improvements
- Implemented repository pattern for clean data access
- Service layer encapsulates business logic
- Consistent 2-decimal price rounding across all strategies
- MaintainMargin strategy now correctly honors both config and product margin requirements

## System Architecture

### Application Framework
- **Technology Stack**: .NET 8.0 (C#)
- **Architecture Pattern**: Repository pattern with service layer
- **Rationale**: Separates data access logic from business logic, making the codebase more maintainable and testable

### Data Storage
- **Database**: SQLite with Microsoft.Data.Sqlite (v9.0.10)
- **Design Choice**: Embedded database for simplicity and portability
- **Rationale**: SQLite eliminates the need for separate database server setup, making the demo application self-contained and easy to distribute. Perfect for educational/demo purposes
- **Pros**: Zero configuration, cross-platform, file-based storage
- **Cons**: Limited for high-concurrency scenarios (not needed for demo)

### Core Components

#### Data Layer (`Data/`)
- **DatabaseContext**: Manages database connections and schema
- **ProductRepository**: CRUD operations for products
- **CompetitorRepository**: Manages competitor price data with query support
- **PriceHistoryRepository**: Tracks all price changes with audit trail
- **Purpose**: Abstracts database operations and provides clean data access interface

#### Business Logic Layer (`Services/`)
- **RepricingService**: Core pricing engine that applies repricing strategies
  - Validates competitor data availability
  - Calculates new prices based on selected strategy
  - Applies price constraints (min/max, margin requirements)
  - Records price changes in history
- **SampleDataService**: Initializes test/demo data for the application
- **Design**: Service classes encapsulate business rules and coordinate between repositories

#### Domain Models (`Models/`)
- **Product**: Represents items in the catalog with price ranges and constraints
- **Competitor**: Stores competitor pricing information
- **PriceHistory**: Audit trail of all price changes
- **Sale**: Transaction records for profitability analysis
- **RepricingStrategy**: Defines configurable pricing rules

#### Repricing Strategies Implemented
1. **Match Lowest**: Sets price equal to lowest competitor price
2. **Undercut by Percent**: Reduces price by percentage below competitor (e.g., 5% lower)
3. **Undercut by Amount**: Reduces price by fixed amount below competitor (e.g., 50₽ lower)
4. **Maintain Margin**: Ensures minimum profit margin is maintained, using the higher of config margin or product target margin
5. **Premium Pricing**: Adds premium markup above competitor prices

Each strategy addresses different competitive positioning requirements, from aggressive price competition to premium brand positioning.

**Important Implementation Details:**
- MaintainMargin uses `Math.Max(config.MinMarginPercent, product.TargetMarginPercent)` to honor both the strategy's requested margin and the product's own margin floor
- All strategies go through `ApplyPriceConstraints` which enforces min/max prices and margin requirements
- Prices are consistently rounded to 2 decimal places after all constraints are applied

### User Interface

#### Console Application (`Program.cs`)
- **Platform**: Cross-platform console application
- **Features**: 
  - Demonstrates all repricing strategies
  - Shows product catalog with margins
  - Displays competitor analysis
  - Provides pricing analytics
- **Use Case**: Demo and testing on Replit (Linux) environment

#### Windows Forms UI (`WindowsFormsUI/`)
- **Platform**: Windows-only (requires Windows to run)
- **Note**: Code is available in the project but cannot run on Linux/Replit
- **Key Forms**:
  - MainForm: Primary application interface
  - ProductManagementForm: Product catalog CRUD operations
  - RepricingConfigForm: Strategy configuration interface

### Data Serialization
- **Library**: Newtonsoft.Json (v13.0.4)
- **Purpose**: Handles JSON serialization for configuration and data exchange
- **Choice Rationale**: Industry-standard JSON library with robust feature set

### Design Principles
- **Separation of Concerns**: Clear boundaries between UI, business logic, and data access
- **Repository Pattern**: Isolates data persistence logic
- **Service Layer**: Encapsulates business rules and orchestrates operations
- **Domain-Driven Design**: Models reflect real-world business concepts (Products, Competitors, Sales)
- **Constraint-Based Pricing**: All strategies respect min/max prices and margin requirements

## External Dependencies

### NuGet Packages
- **Microsoft.Data.Sqlite** (v9.0.10): SQLite database provider for .NET
- **Newtonsoft.Json** (v13.0.4): JSON serialization/deserialization
- **SQLitePCLRaw** (v2.1.10): Low-level SQLite bindings (dependency of Microsoft.Data.Sqlite)

### Runtime Requirements
- **.NET 8.0 Runtime**: Required to execute the application
- **Platform Support**: Cross-platform for core logic, Windows-only for UI components

### External Services
- **Wildberries Marketplace**: Target platform for price management (integration would be implemented for production use)
- **Note**: Current implementation is a standalone demo without active external API integration

### Database
- **SQLite**: Embedded, file-based relational database
- **Location**: Local file system (`wildberries_repricer.db`)
- **Schema**: Managed through DatabaseContext with tables for Products, Competitors, PriceHistory, and Sales

## Project Structure

```
WildberriesRepricer/
├── Models/                      # Domain models
├── Data/                        # Data access layer (repositories)
├── Services/                    # Business logic layer
├── WindowsFormsUI/             # Windows Forms UI (Windows only)
├── Program.cs                   # Console demo application
├── WildberriesRepricer.csproj  # Project configuration
├── README.md                    # User documentation
└── wildberries_repricer.db     # SQLite database (created at runtime)
```

## Running the Application

The application runs automatically via the configured workflow. It will:
1. Initialize the SQLite database
2. Load sample coffee products and competitor data
3. Demonstrate all repricing strategies
4. Display analytics and pricing insights

## Future Enhancements (Suggested by Architect Review)

1. **Automated Testing**: Add unit tests for each strategy and constraint pipeline
2. **Enhanced Diagnostics**: Log which constraint was applied in RepricingResult
3. **Documentation**: Document MaintainMargin behavior for operators
4. **Real API Integration**: Connect to actual Wildberries API
5. **Scheduled Repricing**: Implement automatic repricing on schedule
6. **Export Functionality**: Generate Excel/PDF reports
