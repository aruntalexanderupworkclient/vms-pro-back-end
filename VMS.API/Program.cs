using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VMS.API.Extensions;
using VMS.API.Middleware;
using VMS.Infrastructure.Data;
using VMS.Infrastructure.InMemory;
using VMS.Infrastructure.Repositories.InMemory;
using VMS.Infrastructure.Repositories.Interfaces;
using VMS.Infrastructure.Repositories.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);


// Add controllers
builder.Services.AddControllers();

// Infrastructure - switch between InMemory and PostgreSQL
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemory");

// Always register DbContext for EF Core migrations
builder.Services.AddDbContext<VmsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
    o =>
    {
        o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null);
    }));

// Register repositories based on configuration
if (useInMemory)
{
    builder.Services.AddSingleton<InMemoryDataStore>();
    builder.Services.AddScoped(typeof(IRepository<>), typeof(InMemoryRepository<>));
}
else
{
    builder.Services.AddScoped(typeof(IRepository<>), typeof(PostgreSqlRepository<>));
}

// Application services, AutoMapper, FluentValidation
builder.Services.AddApplicationServices();
builder.Services.AddAutoMapperProfiles();
builder.Services.AddFluentValidators();

// Authentication & Authorization
//builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("VmsPolicy", policy =>
        policy.WithOrigins("http://localhost:4300", "https://ashy-pebble-0441d4500.6.azurestaticapps.net")
              .AllowAnyHeader()
              .AllowAnyMethod());
});


builder.Services.AddAuthorization();

// Swagger
builder.Services.AddSwaggerDocumentation();

// CORS
builder.Services.AddCorsPolicy(builder.Configuration);

var app = builder.Build();

// Exception middleware
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "VMS Pro API v1"));
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<VmsDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Migration failed: " + ex.Message);
        throw; // optional (remove if you don’t want crash)
    }
}


app.UseCors("VmsPolicy");
app.Use(async (context, next) =>
{
    context.Response.Headers["Cross-Origin-Opener-Policy"] = "same-origin-allow-popups";
    await next();
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
