using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using VMS.API.Services;
using VMS.Application.Interfaces;
using VMS.Application.Mappings;
using VMS.Application.Services;
using VMS.Application.Validators;
using VMS.Domain.Entities;
using VMS.Infrastructure.Repositories.UnitOfWork;

namespace VMS.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // ✅ NEW: Register Unit of Work Factory
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserProvider, HttpCurrentUserProvider>();
        services.AddScoped<IUnitOfWorkFactory, PostgreSqlUnitOfWorkFactory>();

        // ✅ Register User Context for accessing logged-in user details in services
        services.AddScoped<IUserContext, UserContext>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IVisitorService, VisitorService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IHostService, HostService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IMdmService, MdmService>();

        return services;
    }

    public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        return services;
    }

    public static IServiceCollection AddFluentValidators(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();
        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "VMS-Default-Secret-Key-For-Development-Only-256-Bits!!";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"] ?? "VMS.API",
                ValidAudience = jwtSettings["Audience"] ?? "VMS.Client",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "VMS Pro API",
                Version = "v1",
                Description = "Visitor Management System API - VMS Pro",
                Contact = new OpenApiContact
                {
                    Name = "VMS Pro Team",
                    Email = "admin@vms.com"
                }
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:4200" };

        services.AddCors(options =>
        {
            options.AddPolicy("VmsPolicy", builder =>
            {
                builder.WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        return services;
    }
}


