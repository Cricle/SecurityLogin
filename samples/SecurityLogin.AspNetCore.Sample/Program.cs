using Ao.Cache;
using Ao.Cache.Serizlier.TextJson;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using SecurityLogin.AccessSession;
using SecurityLogin.AspNetCore;
using SecurityLogin.AspNetCore.Services;
using StackExchange.Redis;
using System;
using System.Net;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Authorization", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Authorization"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference=new OpenApiReference
                {
                    Type= ReferenceType.SecurityScheme,
                    Id="Authorization"
                },
                Scheme= "Authorization",
                In= ParameterLocation.Header
            },Array.Empty<string>()
        }
    });
});
builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlite("Data Source=app.db"))
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
builder.Services.AddSingleton(x => x.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
builder.Services.AddSingleton<IDistributedLockFactory>(new RedLockFactory(new RedLockConfiguration(new RedLockEndPoint[]
{
    new RedLockEndPoint(new DnsEndPoint("127.0.0.1",6379))
})));
builder.Services.AddNormalSecurityService();
builder.Services.AddScoped<LoginService>();
builder.Services.AddMemoryCache();
builder.Services.AddDefaultSecurityLoginHandler<string, UserSnapshot>();
builder.Services.AddScoped<IIdentityService<string,UserSnapshot>, MyIdentityService>();
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IEntityConvertor, TextJsonEntityConvertor>();
builder.Services.AddInRedisFinder();
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = SecurityLoginConsts.AuthenticationScheme;
    x.AddScheme<CrossAuthenticationHandler<UserSnapshot>>(SecurityLoginConsts.AuthenticationScheme, "se-default");
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public class MyIdentityService : IdentityService<string, UserSnapshot>
{
    public MyIdentityService(ICacheVisitor cacheVisitor) : base(cacheVisitor)
    {
    }

    protected override Task<UserSnapshot> AsTokenInfoAsync(string input, TimeSpan? cacheTime, string key, string token)
    {
        return Task.FromResult(new UserSnapshot { Token = token, Id = input, Name = key });
    }
    protected override string GetKey(string token)
    {
        return "Security.Login.Tokens." + token;
    }
}