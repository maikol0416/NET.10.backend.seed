dotnet ef migrations add InitialCreate --project Infraestructure --startup-project Api
dotnet ef database update --project Infraestructure --startup-project Api