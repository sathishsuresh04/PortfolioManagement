# Stock Portfolio API

**Implemented features:** 
-  Developed endpoints for fetching specific portfolios, computing total value of stocks in a chosen currency, and soft-deleting portfolios.
- Integrated CurrencyLayer API for applicable exchange rates, set to refresh every 24 hours.

**Refactoring & Testing:**
- Code refactoring executed to improve both maintenance and readability.
- Created unit tests to confirm feature functionality, focusing on creating a concise and meaningful test suite.

**Database:**
- MongoDB was kept as the primary database for data persistence, leveraging its document-oriented format and scalability advantages.
- 
**Execution:** 
- To run the solution, update the following in your configuration file(appsettings.json):
     "MongoDbOptions": {"ConnectionString": ""}
     "ExchangeRateApiOptions": { "Token": ""}

**Tech Stack:**
- Vertical Slice Architecture
- Domain-Driven Design (DDD)
- Repository Pattern
- Command Query Responsibility Segregation (CQRS) Pattern with MediatR library
- Minimal APIs
- Fluent Validation with a Validation Pipeline Behaviour on top of MediatR
- Azure Cosmos MongoDB
- Structured Logging with Serilog
- Unit Testing with NSubstitute
- .Net 7
- Centralized package management and build managment

**Third-Party Libraries:**
- Ardalis.GuardClauses
- Mapster
- MediatR
- FluentValidation.AspNetCore
- Scrutor
- Serilog
- Humanizer
- Bogus
- EasyCaching.Core**

Note: All changes have adhered to best coding practices, and third-party open source libraries were utilized for task efficiency.
