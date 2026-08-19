using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Configure EF Core - use connection string from configuration, default to SQLite file
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=careers.db";
builder.Services.AddDbContext<CareersWebApi.Data.AppDbContext>(options =>
    options.UseSqlite(connectionString));

// By default use the Greenhouse-backed repository which fetches jobs from the external boards API.
builder.Services.AddMemoryCache();
// Register GreenhouseJobRepository as the IJobRepository and configure its HttpClient
builder.Services.AddHttpClient<CareersWebApi.Repositories.IJobRepository, CareersWebApi.Repositories.GreenhouseJobRepository>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Greenhouse:BaseUrl"] ?? "https://boards-api.greenhouse.io");
});

// allow simple CORS for the frontend during development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    // include XML comments (for method summaries)
    var xmlPath = Path.ChangeExtension(System.Reflection.Assembly.GetExecutingAssembly().Location, ".xml");
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

// Ensure database is created and seed default data if empty
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CareersWebApi.Data.AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Jobs.Any())
    {
        // seed sample jobs
        var jobs = Enumerable.Range(1, 42).Select(i => new CareersWebApi.Entities.JobEntity
        {
            Title = $"Software Engineer {i}",
            Location = i % 3 == 0 ? "New York, NY" : "Remote",
            Department = i % 2 == 0 ? "Engineering" : "Product",
            PublishedAt = DateTime.UtcNow.AddDays(-i),
            AbsoluteUrl = $"https://boards.example.com/jobs/{i}",
            Content = $"<p>This is the job description for job {i}.</p>"
        });

        db.Jobs.AddRange(jobs);
        db.SaveChanges();
    }
}

app.UseAuthorization();

app.MapControllers();

app.Run();
