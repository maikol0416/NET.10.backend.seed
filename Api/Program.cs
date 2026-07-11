using Application;
using Api.Middlewares;
using Infraestructure.Entity;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddDependencyInjectionInfrastructureEf(builder.Configuration);
builder.Services.AddDependencyInjectionApplication();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// CORS — permite peticiones desde el frontend Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:4201")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options
    .AddPreferredSecuritySchemes("Bearer")
    .AddHttpAuthentication("Bearer", auth =>
    {
        auth.Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
    }));
}

// Middleware global de excepciones — debe ser el primero del pipeline
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

// Sirve wwwroot/uploads/* (imágenes guardadas por LocalImageStorageService) como archivos estáticos
app.UseStaticFiles();

// CORS debe ir antes de Authentication/Authorization
app.UseCors("AllowFrontend");

// ─── Authentication & Authorization pipeline ───
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
