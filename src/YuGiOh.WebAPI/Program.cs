
using YuGiOh.WebAPI.Middlewares;
using YuGiOh.Infrastructure;
using YuGiOh.Application;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// 1. Add Services
// =====================================================

// Add Controllers + System.Text.Json options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS (for React)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("http://localhost:3000") // React dev server
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddLogging();

// Infrastructure (Persistence + Identity + Email + CSC + Seeding + Caching)
builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.AddApplicationLayer();

var app = builder.Build();

// =====================================================
// 2. Configure Middleware
// =====================================================

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use HTTPS redirection (recommended)
app.UseHttpsRedirection();

// Use CORS for the React frontend
app.UseCors("AllowReactApp");

// Authentication / Authorization
app.UseAuthentication(); // <— Needed if you use Identity
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// =====================================================
// 3. (Optional) Seed Database
// =====================================================


app.Run();
