using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebAPI6;
using WebAPI6.Extensions;
using WebAPI6.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connStr = builder.Configuration.GetConnectionString("Default");
if (!string.IsNullOrEmpty(connStr))
{
    _ = builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connStr));
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        _ = policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddHttpLogging(option =>
{
    option.LoggingFields = HttpLoggingFields.RequestMethod |
        //HttpLoggingFields.RequestPath |
        //HttpLoggingFields.RequestQuery |
        //HttpLoggingFields.RequestHeaders |
        HttpLoggingFields.ResponseStatusCode |
        HttpLoggingFields.Duration;
    option.RequestBodyLogLimit = 4096;
    option.ResponseBodyLogLimit = 4096;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddLogging();

// Remove default logging providers
builder.Logging.ClearProviders();

// Add Serilog
builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
);

// Add response compression services
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
    // Optional: restrict to certain MIME types
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
});


// Register custom services
builder.Services.AddScopedServices();
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

app.UseCorrelationIdMiddleware();
app.UseHttpLogging();
app.UseExceptionMiddleware();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.UseCors("AllowAll");
app.UseResponseCompression();

app.MapControllers();
app.Run();



