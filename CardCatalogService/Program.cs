using CardCatalogService.API.Middleware;
using CardCatalogService.API.Middlewares;
using CardCatalogService.Application.Interfaces;
using CardCatalogService.Application.Mapping;
using CardCatalogService.Application.Services;
using CardCatalogService.Infrastructure.Authorization;
using CardCatalogService.Infrastructure.Cache;
using CardCatalogService.Infrastructure.Data;
using CardCatalogService.Infrastructure.External;
using CardCatalogService.Infrastructure.Interfaces;
using CardCatalogService.Infrastructure.Mapping;
using CardCatalogService.Infrastructure.Repositories;
using CardCatalogService.Infrastructure.Services;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(
    typeof(MappingProfile),              // Application katmanındaki profil
    typeof(ExternalMappingProfile)       // Infrastructure içindeki bu yeni profil
);

builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});

var connectionMultiplexer = ConnectionMultiplexer.Connect("localhost:6379");
// Add services to the container.

builder.Services.AddHangfireServer();
builder.Services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);
builder.Services.AddScoped<ICardCacheService, CardCacheService>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddHttpClient<IYgoProDeckClient, YgoProDeckClient>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseHangfireDashboard();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new AllowAllUsersFilter() }
});

app.MapControllers();

app.UseHangfireDashboard("/hangfire");

// Her 30 dakikada bir Sync işlemini çağır
RecurringJob.AddOrUpdate<ISyncService>(
    "sync-cards-job",
    service => service.SyncCardsAsync(),
    "*/30 * * * *"  // Cron format: her 30 dakikada bir
);


app.Run();
