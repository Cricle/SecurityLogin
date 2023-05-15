<h2 align="center">
SecurityLogin
</h2>

<h6 align="center">
A easy, security login programme
</h6>

## Usage

For explample [samples](SecurityLogin.AspNetCore.Sample)

```csharp
//Define your IdentityService
public class MyIdentityService : IdentityService<string, UserSnapshot>
{
    public MyIdentityService(ICacheVisitor cacheVisitor) : base(cacheVisitor)
    {
    }

    protected override Task<UserSnapshot> AsTokenInfoAsync(string input, TimeSpan? cacheTime, string key, string token)
    {
        return Task.FromResult(new UserSnapshot { Token = token, Id = input, Name = key });//Your snapshot define
    }
    protected override string GetKey(string token)
    {
        return "Security.Login.Tokens." + token;
    }
}

//Add services
builder.Services.AddSecurityLogin<UserSnapshot>();

//Add login service
public class LoginService : RSALoginService
{
    public UserManager<IdentityUser> UserManager { get; }
    public IIdentityService<string, UserSnapshot> IdentityService { get; }
    public LoginService(ILockerFactory lockerFactory,
        ICacheVisitor cacheVisitor,
        UserManager<IdentityUser> userManager,
        IIdentityService<string, UserSnapshot> identityService)
        : base(lockerFactory, cacheVisitor)
    {
        IdentityService = identityService;
        UserManager = userManager;
    }

    public async Task<bool> RegistAsync(string connectId, string userName, string passwordHash)
    {
        try
        {
            var pwd = await DecryptAsync(connectId, passwordHash);
            var user = new IdentityUser
            {
                UserName = userName,
                NormalizedUserName = userName,
            };
            var res = await UserManager.CreateAsync(user, pwd);
            return res.Succeeded;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public async Task<IssureTokenResult> LoginAsync(string connectId, string userName, string passwordHash)
    {
        var user = await UserManager.FindByNameAsync(userName);
        if (user == null)
        {
            return null;
        }
        try
        {
            var pwd = await DecryptAsync(connectId, passwordHash);
            var res = await UserManager.CheckPasswordAsync(user, pwd);
            if (res)
            {
                return await IdentityService.IssureTokenAsync(userName);
            }
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
//Done!
```