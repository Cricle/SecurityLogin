﻿using Ao.Cache;
using Microsoft.AspNetCore.Identity;
using SecurityLogin.AccessSession;
using SecurityLogin.Mode.RSA;
using System;
using System.Threading.Tasks;

namespace SecurityLogin.AspNetCore.Services
{
    public class LoginService : RSALoginService
    {
        public UserManager<IdentityUser> UserManager { get; }

        public IIdentityService<UserSnapshot, UserSnapshot> IdentityService { get; }

        public LoginService(ILockerFactory lockerFactory,
            ICacheVisitor cacheVisitor,
            UserManager<IdentityUser> userManager,
            IIdentityService<UserSnapshot, UserSnapshot> identityService)
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
                    return await IdentityService.IssureTokenAsync(new UserSnapshot
                    {
                        Name = userName,
                        Id = user.Id
                    });
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
