using Microsoft.AspNetCore.Identity;
using SecurityLogin.Mode.RSA;
using SecurityLogin.Mode.RSA.Helpers;
using System;
using System.Threading.Tasks;

namespace SecurityLogin.AspNetCore.Services
{
    public class LoginService : RSALoginService
    {
        public static readonly string HeaderKey = "SecurityLogin.AspNetCore.Services.LoginService";
        public static readonly string SharedIdentityKey = "SecurityLogin.AspNetCore.Services.LoginService.SharedIdentity";
        public static readonly string SharedLockKey = "Lock.SecurityLogin.AspNetCore.Services.LoginService.Shared";

        public UserManager<IdentityUser> UserManager { get; }

        public LoginService(ILockerFactory lockerFactory,
            ICacheVisitor cacheVisitor,
            UserManager<IdentityUser> userManager)
            : base(lockerFactory, cacheVisitor)
        {
            UserManager = userManager;
        }
        
        public async Task<bool> RegistAsync(string connectId, string userName, string passwordHash)
        {
            try
            {
                var pwd = await DecryptAsync(connectId, passwordHash);
                var user = new IdentityUser
                {
                    UserName=userName,
                    NormalizedUserName=userName,
                };
                var res = await UserManager.CreateAsync(user, pwd);
                return res.Succeeded;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> LoginAsync(string connectId, string userName, string passwordHash)
        {
            var user = await UserManager.FindByNameAsync(userName);
            if (user == null)
            {
                return false;
            }
            try
            {
                var pwd = await DecryptAsync(connectId, passwordHash);
                var res = await UserManager.CheckPasswordAsync(user, pwd);
                return res;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
