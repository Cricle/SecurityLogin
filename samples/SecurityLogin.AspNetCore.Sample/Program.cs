using Ao.Cache;
using Ao.Cache.Serizlier.TextJson;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using SecurityLogin.AccessSession;
using SecurityLogin.AspNetCore;
using SecurityLogin.AspNetCore.Services;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
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
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("127.0.0.1:6379"));
builder.Services.AddNormalSecurityService();
builder.Services.AddScoped<LoginService>();
builder.Services.AddSingleton<IEntityConvertor, TextJsonEntityConvertor>();
builder.Services.AddDistributedLockFactory().AddInRedisFinder();
builder.Services.AddSecurityLoginWithDefaultIdentity<string,UserSnapshot>(
    req => Task.FromResult(new UserSnapshot { Id = req.Input, Name = req.Key, Token = req.Token }),"SecurityLogin.Session");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();