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

using Microsoft.OpenApi.Models;

using Newtonsoft.Json;

using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

#region Configuración de conexión local desde appsettings
// Obtener cadena de conexión directamente desde appsettings.json
string connectionString = configuration.GetConnectionString("SQLConnection")
    ?? throw new ServiceException("La cadena de conexión SQLConnection no está definida");

Console.WriteLine($"Cadena de conexión usada: {connectionString}");
#endregion

// Registrar servicios
builder.Services.AddSingleton(new Provider(connectionString));
builder.Services.AddSingleton<LogUtil>();
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
        Description = "API para la gestión de los catalogos"
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
app.UseAuthorization();

app.MapControllers();
app.UseHealthChecks("/health");

app.Run();
