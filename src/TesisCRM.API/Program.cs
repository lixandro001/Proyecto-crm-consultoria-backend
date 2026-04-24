using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using TesisCRM.API.Data;
using TesisCRM.API.Middleware;
using TesisCRM.API.Repositories;
using TesisCRM.API.Services;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CRM API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(o => o.AddPolicy("frontend", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

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

builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<FileStorageService>();
builder.Services.AddScoped<ContratoWordService>();
builder.Services.AddScoped<ContratoPdfService>();

var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("AdminOnly", p => p.RequireRole("ADMIN"));
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
