using JobAggregator.Application.Options;
using JobAggregator.Domain.Interfaces;
using JobAggregator.Infrastructure.BackgroundJobs;
using JobAggregator.Infrastructure.Persistence;
using JobAggregator.Infrastructure.Scrapers;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IJobRepository, JobRepository>();

// Opcje Adzuna
builder.Services.Configure<AdzunaOptions>(
    builder.Configuration.GetSection(AdzunaOptions.SectionName));

// HttpClient dla Adzuna
builder.Services.AddHttpClient<AdzunaJobSource>();

// Rejestracja scraperów
builder.Services.AddScoped<IJobSource, AdzunaJobSource>();
builder.Services.AddScoped<FetchOrchestrator>();
builder.Services.AddHostedService<JobFetcherService>();
builder.Services.AddHttpClient<JustJoinJobSource>();
builder.Services.AddScoped<IJobSource, JustJoinJobSource>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowReact");
//app.UseHttpsRedirection();
app.MapControllers();
app.Run();