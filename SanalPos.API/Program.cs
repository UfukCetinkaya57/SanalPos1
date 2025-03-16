using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SanalPos.API.Middleware;
using SanalPos.Application;
using SanalPos.Infrastructure;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SanalPos.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Update;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SanalPos.Application.Common.Models;
using SanalPos.Infrastructure.Services;
using SanalPos.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using SanalPos.Domain.Entities;
using SanalPos.Infrastructure.Authentication;
using SanalPos.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Sanal POS API", 
        Version = "v1",
        Description = "Sanal POS uygulaması için API",
        Contact = new OpenApiContact
        {
            Name = "Sanal POS Ekibi",
            Email = "info@sanalpos.com"
        }
    });
    
    // Benzersiz şema ID'leri oluştur
    c.CustomSchemaIds(type => type.FullName);
    
    // JWT için Swagger yapılandırması
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// JWT Ayarları
builder.Services.Configure<SanalPos.Infrastructure.Settings.JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Password Hasher
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

// Veritabanını otomatik oluştur
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        Console.WriteLine("Veritabanı oluşturma işlemi başlatılıyor...");
        
        context.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));
        
        // Veritabanı bağlantısını kontrol et
        bool canConnect = false;
        try
        {
            canConnect = context.Database.CanConnect();
            Console.WriteLine($"Veritabanına bağlantı durumu: {(canConnect ? "Başarılı" : "Başarısız")}");
        }
        catch (Exception connEx)
        {
            Console.WriteLine($"Veritabanına bağlanırken hata: {connEx.Message}");
            if (connEx.InnerException != null)
            {
                Console.WriteLine($"İç hata: {connEx.InnerException.Message}");
            }
            throw;
        }
        
        // Veritabanı yoksa oluştur
        if (!canConnect)
        {
            var dbCreated = context.Database.EnsureCreated();
            Console.WriteLine(dbCreated 
                ? "Veritabanı başarıyla oluşturuldu." 
                : "Veritabanı oluşturulamadı.");
        }
        
        Console.WriteLine("Veritabanı migrasyonları uygulanıyor...");
        context.Database.Migrate();
        Console.WriteLine("Veritabanı migrasyonları başarıyla uygulandı.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Veritabanı oluşturulurken bir hata oluştu: {ex.Message}");
        Console.WriteLine($"Hata türü: {ex.GetType().Name}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        
        if (ex.InnerException != null)
        {
            Console.WriteLine($"İç hata: {ex.InnerException.Message}");
            Console.WriteLine($"İç hata türü: {ex.InnerException.GetType().Name}");
            Console.WriteLine($"İç hata Stack Trace: {ex.InnerException.StackTrace}");
            
            if (ex.InnerException.InnerException != null)
            {
                Console.WriteLine($"En iç hata: {ex.InnerException.InnerException.Message}");
            }
        }
        
        // Loglama işlemi
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "Veritabanı oluşturulurken bir hata oluştu: {ErrorMessage}", ex.Message);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sanal POS API v1");
    });
    
    // Geliştirme ortamında detaylı hata mesajları göster
    app.UseDeveloperExceptionPage();
}

// Özel exception handling middleware'i kullan
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Helper method to extract validation errors
static Dictionary<string, string[]> ExtractValidationErrors(DbUpdateException dbEx)
{
    var errors = new Dictionary<string, string[]>();
    
    if (dbEx.InnerException != null)
    {
        errors.Add("DbError", new[] { dbEx.InnerException.Message });
    }
    
    // Try to extract SQL Server specific errors
    if (dbEx.InnerException is SqlException sqlEx)
    {
        foreach (SqlError err in sqlEx.Errors)
        {
            errors.Add($"SqlError_{err.Number}", new[] { err.Message });
        }
    }
    
    return errors;
}
