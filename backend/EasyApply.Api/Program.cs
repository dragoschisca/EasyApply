using EasyApply.BusinessLayer.Interfaces.Services;
using EasyApply.BusinessLayer.Core;
using EasyApply.BusinessLayer.Core.Storage;
using EasyApply.Domain.Interfaces.Repositories;
using EasyApply.DataAccess.Data;
using EasyApply.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using System.Text.Json.Serialization;
using EasyApply.BusinessLayer.Core.AI;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

#region DATABASE_SETUP

var connectionString =
    Environment.GetEnvironmentVariable("SUPABASE_CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception("Database connection string is missing.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
        npgsqlOptions.CommandTimeout(60);
    }));

var jwtSecret =
    Environment.GetEnvironmentVariable("Jwt__Secret")
    ?? builder.Configuration["Jwt:Secret"];

if (string.IsNullOrWhiteSpace(jwtSecret))
{
    throw new Exception("JWT secret is missing.");
}

#endregion

#region SERVICES_SETUP

builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();

builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();

builder.Services.AddScoped<ICVService, CVService>();
builder.Services.AddScoped<ICVRepository, CVRepository>();

builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();

builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IJobRepository, JobRepository>();

builder.Services.AddScoped<ISavedJobService, SavedJobService>();
builder.Services.AddScoped<ISavedJobRepository, SavedJobRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGeminiService, GeminiService>();
builder.Services.AddScoped<ISupabaseStorageService, SupabaseStorageService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();

// FluentEmail Setup NEED CHANGES
// var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "localhost";
// var smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "25");
// var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER");
// var smtpPass = Environment.GetEnvironmentVariable("SMTP_PASS");
// var fromEmail = Environment.GetEnvironmentVariable("SMTP_FROM_EMAIL") ?? "no-reply@easyapply.com";
// var fromName = Environment.GetEnvironmentVariable("SMTP_FROM_NAME") ?? "EasyApply";
//
// builder.Services
//     .AddFluentEmail(fromEmail, fromName)
//     .AddRazorRenderer()
//     .AddMailKitSender(new FluentEmail.Mailkit.Smtp.SmtpClientOptions
//     {
//         Server = smtpHost,
//         Port = smtpPort,
//         User = smtpUser,
//         Password = smtpPass,
//         UseSsl = false, // Set to true if using SSL/TLS port like 465
//         RequiresAuthentication = !string.IsNullOrEmpty(smtpUser)
//     });

// Geocoding — named HttpClient with required Nominatim User-Agent header.
builder.Services.AddHttpClient("Nominatim", client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "EasyApply/1.0 (job-portal)");
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddScoped<IGeocodingService, GeocodingService>();

builder.Services.AddHttpClient();

#endregion

#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = new List<string>
        {
            "http://localhost:4200",
            "https://localhost:4200",
            "http://localhost:5077",
            "https://localhost:5077"
        };

        // Add from Environment Variable (Comma-separated)
        var envFrontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL");
        if (!string.IsNullOrEmpty(envFrontendUrl))
        {
            allowedOrigins.AddRange(envFrontendUrl.Split(',', StringSplitOptions.RemoveEmptyEntries));
        }

        // Add from Configuration
        var configFrontendUrl = builder.Configuration["Cors:FrontendUrl"];
        if (!string.IsNullOrEmpty(configFrontendUrl))
        {
            allowedOrigins.Add(configFrontendUrl);
        }

        policy.WithOrigins(allowedOrigins.Distinct().ToArray())
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

#endregion

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "EasyApply API V1");
    options.RoutePrefix = "swagger"; // Standard: Accessible at /swagger
});

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.MapControllers();
app.Run();