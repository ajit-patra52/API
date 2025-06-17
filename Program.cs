using Microsoft.EntityFrameworkCore;
using WebAPI6;
using WebAPI6.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connStr = builder.Configuration.GetConnectionString("Default");
if (!string.IsNullOrEmpty(connStr))
{
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connStr));
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();



// Register custom services
builder.Services.AddScopedServices();


var app = builder.Build();
app.UseExceptionMiddleware();
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();
app.Run();



