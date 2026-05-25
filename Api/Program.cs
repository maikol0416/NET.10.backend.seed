using Application;
using Api.Middlewares;
using Infraestructure.Entity;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDependencyInjectionInfrastructureEf(builder.Configuration);
builder.Services.AddDependencyInjectionApplication();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Middleware global de excepciones — debe ser el primero del pipeline
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
