using System.Net;
using System.Text.Json.Serialization;
using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Domain.Repositories;
using Infrastructure.Options;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.Options;
using Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Swagger/OpenAPI (placeholder)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// DI - Application interfaces and Infrastructure implementations
builder.Services.AddHttpClient();
builder.Services.Configure<PhpApiOptions>(builder.Configuration.GetSection("PhpApi"));
builder.Services.AddHttpClient("PhpApi", (sp, client) =>
{
    var opts = sp.GetRequiredService<IOptions<PhpApiOptions>>().Value;
    client.BaseAddress = new Uri(opts.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds);
    client.DefaultRequestVersion = System.Net.HttpVersion.Version11;
    client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
    client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
    if (!string.IsNullOrWhiteSpace(opts.ApiKey))
    {
        client.DefaultRequestHeaders.Add("X-API-KEY", opts.ApiKey);
    }
});

// OAuth token provider (local implementation)
builder.Services.AddScoped<IOAuthTokenProvider, OAuthTokenProvider>();
builder.Services.AddScoped<ITerpelClient, TerpelHttpService>();
builder.Services.AddScoped<IFileParser, FileParser>();
builder.Services.AddScoped<IProcessingService, ProcessingService>();

builder.Services.AddScoped<IConsolidadoJobRepository, ConsolidadoJobRepository>();
builder.Services.AddScoped<IConsolidadoArchivoRepository, ConsolidadoArchivoRepository>();
builder.Services.AddScoped<IVentaDetalladaRepository, VentaDetalladaRepository>();
builder.Services.AddScoped<ISincronizadorService, SincronizadorService>();

// AutoMapper profile (registered as requested)
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// FluentValidation is registered inside services where validators are used (validators are in Application)

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseCors();

// Endpoint raíz para verificar que el servidor está funcionando
app.MapGet("/", () => Results.Ok(new 
{ 
    mensaje = "🚀 API Terpel Integración",
    version = "1.0.0",
    estado = "Activo",
    endpoints = new[]
    {
        "GET  /health - Verificar salud del servidor",
        "POST /api/terpel/ventas/sync - Procesar ventas (síncrono)",
        "POST /api/terpel/ventas/async - Procesar ventas (asíncrono)"
    },
    documentacion = "/swagger"
}));

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new 
{ 
    estado = "Saludable",
    timestamp = DateTime.UtcNow,
    servicio = "Terpel Integration API"
}));

app.MapControllers();

app.Run();
