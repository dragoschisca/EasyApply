using EasyApply.Api;
using EasyApply.BusinessLayer.Interfaces.Services;
using EasyApply.BusinessLayer.Core;
using EasyApply.Domain.Interfaces.Repositories;
using EasyApply.DataAccess.Data;
using EasyApply.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region DATABASE_SETUP

var password = Environment.GetEnvironmentVariable("EasyApplyDB_PASSWORD");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    .Replace("PLACEHOLDER", password);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            // Allow local dev AND deployed Vercel frontend
            var allowedOrigins = new List<string>
            {
                "http://localhost:4200",
                "https://localhost:4200"
            };

            // Read Vercel URL from env (set this on Render)
            var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL");
            if (!string.IsNullOrEmpty(frontendUrl))
            {
                allowedOrigins.Add(frontendUrl);
            }

            policy.WithOrigins(allowedOrigins.ToArray())
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.MapControllers();
app.Run();