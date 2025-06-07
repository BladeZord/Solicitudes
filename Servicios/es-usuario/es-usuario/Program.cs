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
        Description = "API para la gestión de los usuarios"
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
