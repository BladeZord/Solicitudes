using es_usuario.Constans;
using es_usuario.Controller.contract;
using es_usuario.Controller.impl;
using es_usuario.exception;
using es_usuario.middleware;
using es_usuario.Repository.contract;
using es_usuario.Repository.impl;
using es_usuario.Services.contract;
using es_usuario.Services.impl;
using es_usuario.utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

#region Configuración de conexión local desde appsettings
// Obtener cadena de conexión directamente desde appsettings.json
string connectionString = configuration.GetConnectionString("SQLConnection")
    ?? throw new ServiceException("La cadena de conexión SQLConnection no está definida");

Console.WriteLine($"Cadena de conexión usada: {connectionString}");
#endregion

// Configuración de JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
        };
    });

// Registrar servicios
builder.Services.AddSingleton(new Provider(connectionString));
builder.Services.AddControllers();
builder.Services.AddScoped<IController, ControllerImpl>();
builder.Services.AddScoped<IService, ServiceImpl>();
builder.Services.AddScoped<IRepository, RepositoryImpl>();
builder.Services.AddScoped<DbConnectionManager>();
builder.Services.AddHealthChecks();

// Configuración de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = ApiConstants.Routes.ControllerName,
        Version = "v1",
        Description = "API para la gestión de los usuarios"
    });

    // Configuración de autenticación en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
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

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("NUXT", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configuración JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

var app = builder.Build();

// Pipeline HTTP
//if (!app.Environment.IsProduction())
//{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{ApiConstants.Routes.ControllerName} v1");
        c.RoutePrefix = "swagger";
    });
//}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors("NUXT");

// Agregar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseHealthChecks("/health");

app.Run();
