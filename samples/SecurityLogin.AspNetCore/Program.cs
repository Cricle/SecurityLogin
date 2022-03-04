using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using SecurityLogin;
using SecurityLogin.AspNetCore;
using SecurityLogin.AspNetCore.Services;
using SecurityLogin.Store.Redis;
using SecurityLogin.Transfer.TextJson;
using StackExchange.Redis;
using System.Net;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(x=>x.UseSqlite("Data Source=app.db"))
    .AddIdentity<IdentityUser, IdentityRole>(x =>
    {
        x.Password.RequireDigit = false;
        x.Password.RequiredLength = 1;
        x.Password.RequiredUniqueChars = 0;
        x.Password.RequireLowercase = false;
        x.Password.RequireUppercase = false;
        x.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddStackExchangeRedisCache(x => x.Configuration = "127.0.0.1:6379");
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("127.0.0.1:6379"));
builder.Services.AddSingleton<IDatabase>(x=>x.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
builder.Services.AddSingleton<IDistributedLockFactory>(new RedLockFactory(new RedLockConfiguration(new RedLockEndPoint[]
{
    new RedLockEndPoint(new DnsEndPoint("127.0.0.1",6379))
})));
builder.Services.AddNormalSecurityService();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<StudentCacheFinder>();
builder.Services.AddScoped<StudentIdCacheFinder>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

