using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SanalPos.Application.Common.Interfaces;
using SanalPos.Application.Interfaces;
using SanalPos.Infrastructure.Authentication;
using SanalPos.Infrastructure.Persistence;
using SanalPos.Infrastructure.Repositories;
using SanalPos.Infrastructure.Services;
using SanalPos.Infrastructure.Settings;
using System.Text;

namespace SanalPos.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Settings
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            // DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => 
                    {
                        b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        b.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    }
                );
                
                // Command timeout
                options.EnableSensitiveDataLogging(true);
                options.EnableDetailedErrors();
            });

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services
            services.AddSingleton<IDateTime, DateTimeService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();
            services.AddHttpContextAccessor();

            // JWT Authentication
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();

            // Interface implementations
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddTransient<IDateTime, DateTimeService>();

            // Repository implementations
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();

            // Entity Framework veri depolama service'ini yapılandır
            services.AddTransient<IModelCustomizer, ModelCustomizer>();

            return services;
        }
    }
}