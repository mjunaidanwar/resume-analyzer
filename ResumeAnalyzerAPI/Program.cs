using Microsoft.EntityFrameworkCore;
using ResumeAnalyzerAPI.Data;
using ResumeAnalyzerAPI.Repositories;
using ResumeAnalyzerAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the database context with your connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register your repository for the repository pattern
builder.Services.AddScoped<IResumeAnalysisRepository, ResumeAnalysisRepository>();

// Register ResumeAnalyzerService
builder.Services.AddScoped<ResumeAnalyzerService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Required to route requests to controllers
app.UseRouting();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles(); // Serves index.html when accessing "/"
app.UseStaticFiles();  // Serves all other files (main.js, etc.)


app.MapControllers();

app.Run();
