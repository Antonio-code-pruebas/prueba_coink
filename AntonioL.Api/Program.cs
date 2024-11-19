using AntonioL.Api.Mapping;
using AntonioL.Logic.Interfaces;
using AntonioL.Logic.Logic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AntonioL.Api.Middleware;
using Serilog.Events;
using Serilog;
using System.Text.Json.Serialization;
using AntonioL.Share.Dtos;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using AntonioL.Api.Filters;
using Microsoft.Extensions.DependencyInjection;
using AntonioL.Models.PruebaCoink;

var builder = WebApplication.CreateBuilder(args);

//agregar configuración desde appsetting.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json")
    .Build();

// Add services to the container.

// Configuración de Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Reemplaza el logger predeterminado con Serilog
builder.Host.UseSerilog();



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Prueba Coink - Antonio López Ch.",
        Version = "v1",
        Description = "Api para prueba de candidato Antonio López Ch. en Coink",
        Contact = new OpenApiContact
        {
            Name = "Antonio López Ch.",
            Email = "lopezchantonio@gmail.com"
        }
    });
    options.IncludeXmlComments(xmlCommentsFullPath);
});

// Leer los orígenes permitidos desde appsettings.json
var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<List<string>>();
if (allowedOrigins == null || !allowedOrigins.Any())
{
    throw new Exception("AllowedOrigins no está configurado correctamente en appsettings.json.");
}
Log.Information("Allowed Origins: {AllowedOrigins}", string.Join(", ", allowedOrigins));
// Registrar la lista de orígenes permitidos como un servicio
builder.Services.AddSingleton(allowedOrigins);

// Registrar el filtro de acción global
builder.Services.AddControllers(options =>
{
    // Inyectar el logger en el filtro
    options.Filters.Add<ValidateOriginFilter>();
});

// Política de CORS para Producción
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionCorsPolicy",
        builder =>
        {
            builder.WithOrigins(allowedOrigins.ToArray())
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

// Política de CORS para Desarrollo
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentCorsPolicy",
        builder =>
        {
            builder.WithOrigins(allowedOrigins.ToArray())
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});
// FIN CORS


builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Automapper
builder.Services.AddAutoMapper(typeof(MappingProfiles));

//Configurar el dbcontext de PRUEBA COINK
builder.Services.AddDbContext<AntonioLContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
});

// Repository 
builder.Services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));

// Inyectar el UnitOfWork
builder.Services.AddScoped<IUnitOfWork<AntonioLContext>, UnitOfWork<AntonioLContext>>();

// Support string to enum conversions
builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});


builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-ES");
});


// Configurar límite de tamaño de la solicitud
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
    //options.KeyLengthLimit = int.MaxValue;
    //options.ValueLengthLimit = int.MaxValue;
    //options.MultipartBodyLengthLimit = long.MaxValue; // Aumenta el límite del cuerpo multipart
    //options.MultipartHeadersLengthLimit = int.MaxValue;
});


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//CORS
//app.UseCors("CorsRule");
if (app.Environment.IsDevelopment())
{
    // Ambiente de Desarrollo
    app.UseCors("DevelopmentCorsPolicy");
}
else
{
    // Ambiente de Producción
    app.UseCors("ProductionCorsPolicy");
    app.UseHsts(); // Habilita HSTS para producción
}
// FIN CORS


app.UseMiddleware<ExceptionMiddleware>();

app.UseStatusCodePagesWithReExecute("/errors", "?code={0}");

app.UseRouting();

app.Use((context, next) =>
{
    var token = context.Request.Headers["Authorization"];
    // Registra el token o realiza cualquier otra verificación que desees.
    Console.WriteLine($"Token recibido: {token}");
    return next();
});

app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();


app.Run();
