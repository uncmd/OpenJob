[![build](https://github.com/uncmd/OpenJob/actions/workflows/build.yml/badge.svg)](https://github.com/uncmd/OpenJob/actions/workflows/build.yml)

## About this solution

This is a minimalist, non-layered startup solution with the ABP Framework. All the fundamental ABP modules are already installed.

## How to run

The application needs to connect to a database. Run the following command in the `OpenJob` directory:

````bash
dotnet run --migrate-database
````

This will create and seed the initial database. Then you can run the application with any IDE that supports .NET.

Happy coding..!

## Code First Migrations

create a new migration

````bash
dotnet ef migrations add Added_TodoItem
````

apply changes to the database using the following command

````bash
dotnet ef database update
````

OpenJob

