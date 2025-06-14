using es_catalogo.Constans;
using es_catalogo.Controller.contract;
using es_catalogo.Controller.impl;
using es_catalogo.exception;
using es_catalogo.middleware;
using es_catalogo.Repository.contract;
using es_catalogo.Repository.impl;
using es_catalogo.Services.contract;
using es_catalogo.Services.impl;
using es_catalogo.utils;
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
builder.Services.AddScoped<IController, ControllerImpl>(); // Capa de Controlador: Interfaz e implementacion del Controlador. Recepta parametros y procesa respuestas 
builder.Services.AddScoped<IService, ServiceImpl>(); // Capa de Servicio: Interfaz e implementacion del servicio se encarga de aplicar la logica del negocio
builder.Services.AddScoped<IRepository, RepositoryImpl>(); // Capa de Repository:  Interfaz e implementacion del Repositorio, es decir se comunica con la base de datos mediante acciones o queries
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
        Description = "API para la gestión de los catalogos"
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
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{ApiConstants.Routes.ControllerName} v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors("NUXT");
app.UseHttpsRedirection();

// Agregar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseHealthChecks("/health");

app.Run();
