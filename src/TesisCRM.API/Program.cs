using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
//using QuestPDF.Infrastructure;
using TesisCRM.API.Data;
using TesisCRM.API.Middleware;
using TesisCRM.API.Repositories;
using TesisCRM.API.Services;

//QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// ==============================
// Logging básico
// ==============================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ==============================
// Controllers
// ==============================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ==============================
// Swagger
// ==============================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CRM API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Ingrese el token en este formato: Bearer {token}",
        Name = "Authorization",
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

// ==============================
// CORS
// ==============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ==============================
// Validación de configuración
// ==============================
var connectionString = builder.Configuration.GetConnectionString("Default");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception("No se encontró la cadena de conexión 'ConnectionStrings:Default' en appsettings.json.");
}

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new Exception("No se encontró la configuración 'Jwt:Key' en appsettings.json.");
}

if (string.IsNullOrWhiteSpace(jwtIssuer))
{
    throw new Exception("No se encontró la configuración 'Jwt:Issuer' en appsettings.json.");
}

if (string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new Exception("No se encontró la configuración 'Jwt:Audience' en appsettings.json.");
}

// ==============================
// Repositories
// ==============================
builder.Services.AddScoped<SqlConnectionFactory>();

builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ClienteRepository>();
builder.Services.AddScoped<ServicioRepository>();
builder.Services.AddScoped<ClienteServicioRepository>();
builder.Services.AddScoped<PlantillaContratoRepository>();
builder.Services.AddScoped<ContratoRepository>();
builder.Services.AddScoped<AgendaRepository>();
builder.Services.AddScoped<PagoRepository>();
builder.Services.AddScoped<DashboardRepository>();

// ==============================
// Services
// ==============================
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<FileStorageService>();
builder.Services.AddScoped<ContratoWordService>();
//builder.Services.AddScoped<ContratoPdfService>();

// ==============================
// JWT
// ==============================
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// ==============================
// Authorization
// ==============================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireRole("ADMIN");
    });
});

var app = builder.Build();

// ==============================
// Middleware
// ==============================
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRM API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();