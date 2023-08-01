using Ao.Cache;
using Ao.Cache.Serizlier.TextJson;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SecurityLogin.AccessSession;
using SecurityLogin.AspNetCore;
using SecurityLogin.AspNetCore.Services;
using StackExchange.Redis;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("asd", new OpenApiSecurityScheme
    {
        Name = "asd",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "asd"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference=new OpenApiReference
                {
                    Type= ReferenceType.SecurityScheme,
                    Id="asd"
                },
                Scheme= "asd",
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
builder.Services.AddScoped<MyCross>();
builder.Services.AddSecurityLoginWithDefaultIdentity<UserSnapshot, UserSnapshot, MyCross>(
    req => Task.FromResult(req.Input.Set(x => x.Token = req.Token)), "SecurityLogin.Session",
    options =>
    {
        options.ContainerOptionsFunc = opt =>
        {
            opt.AuthenticationScheme = "asd";
            opt.AuthHeader = "asd";
            return opt;
        };
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public class MyCross : CrossAuthenticationHandler<UserSnapshot>
{
    public MyCross(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IRequestContainerConverter<UserStatusContainer<UserSnapshot>> requestContainerConverter, RequestContainerOptions<UserSnapshot> requestContainerOptions) : base(options, logger, encoder, clock, requestContainerConverter, requestContainerOptions)
    {
    }

    protected override Task<AuthenticationTicket> SucceedAsync(UserStatusContainer<UserSnapshot> container, RequestContainerOptions<UserSnapshot> options)
    {
        return Task.FromResult(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity[]
        {
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name,container.UserSnapshot?.Name),
                    new Claim(ClaimTypes.NameIdentifier,container.UserSnapshot?.Id),
                },options.AuthenticationScheme)
        }), options.AuthenticationScheme));
    }
}