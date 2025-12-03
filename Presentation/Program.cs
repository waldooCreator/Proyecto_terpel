using System.Text.Json.Serialization;
using Application.Interfaces;
using Application.Mappings;
using Infrastructure.Services;
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
builder.Services.AddScoped<ITerpelClient, TerpelHttpService>();
builder.Services.AddScoped<IFileParser, FileParser>();
builder.Services.AddScoped<IProcessingService, Application.Services.ProcessingService>();

// AutoMapper profile (registered as requested)
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// Register required services in DI (ensure implementations are available)
builder.Services.AddScoped<ITerpelClient, TerpelHttpService>();
builder.Services.AddScoped<IProcessingService, Application.Services.ProcessingService>();
builder.Services.AddScoped<IFileParser, FileParser>();

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

app.MapControllers();

app.Run();
