# CarParkManagement


## Assumptions
- simple API, no need to care much about decoupling, Repo pattern etc.
- app doesn't need to keep history of what car was parked where and when
- Vehicle type can be restricted to 3 string values (enum-like) in input dto
- No API authentication
- No API versioning
- HTTP as this is for local usage only
- No logging to AppInsights/no Serilog
- No libraries like FluentValidation & FluentAssertions for this take home task
- No healthchecks
- EF Core with migration to set-up, seed & manage the data

## Setup
- it is required to have MSSQL server setup with empty DB and provide connection string in CarParkManagement.API/appsettings.json (for user with "db_owner" permission)
- app can be run from Visual Studio
- on startup app will create tables and seed the data
- there will be swagger UI available at http://localhost:5000/swagger/index.html

## ToDos
- I left some TODOs in the code with possible enhancements & things to clarify as I often tend to do when starting to work on new functionality/project

## Questions I would ask
- "Allocating vehicles to the first available space" -> what exactly does it mean "first available space"?
- Should number of spaces be configurable somehow? If so, how?
- Can charging strategy change in the future? How should it be managed? Separate Endpoints? Db?
- Can list of vehicle types change and/or grow?
- Any requirements regarding vehicle reg? Max length etc.
- Why charge is double and not decimal?
- Details regarding charging. Are charges calculated per started minute or full minute, same for additional charge (per started 5 minutes or finished 5 minutes)?
- Is it planned to extend the API to provide history of what car was parked where and how much was charged?
