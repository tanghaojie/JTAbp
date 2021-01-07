using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using JT.Abp.Authorization;
using JT.Abp.MultiTenancy;
using JT.Web.Core.Authentication.JwtBearer;
using JT.Authorization.Roles;
using JT.MultiTenancy;
using JT.Authorization.Users;
using JT.Web.Core.Models.TokenAuth;
using JT.Abp.Authorization.Users;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Abp.Runtime.Security;
using Abp.Authorization;
using Abp.MultiTenancy;

namespace JT.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TokenAuthController : JTControllerBase
    {
        private readonly JTLogInManager<Tenant, Role, User> _logInManager;
        private readonly IJTTenantCache _tenantCache;
        private readonly TokenAuthConfiguration _configuration;
        //private readonly IExternalAuthConfiguration _externalAuthConfiguration;
        //private readonly IExternalAuthManager _externalAuthManager;
        //private readonly UserRegistrationManager _userRegistrationManager;

        public TokenAuthController(
            JTLogInManager<Tenant, Role, User> logInManager,
            IJTTenantCache tenantCache,
            TokenAuthConfiguration configuration
            //IExternalAuthConfiguration externalAuthConfiguration,
            //IExternalAuthManager externalAuthManager,
            //UserRegistrationManager userRegistrationManager
            )
        {
            _logInManager = logInManager;
            _tenantCache = tenantCache;
            _configuration = configuration;
            //_externalAuthConfiguration = externalAuthConfiguration;
            //_externalAuthManager = externalAuthManager;
            //_userRegistrationManager = userRegistrationManager;
        }

        [HttpPost]
        public async Task<AuthenticateResultModel> Authenticate([FromBody] AuthenticateModel model)
        {
            var loginResult = await GetLoginResultAsync(
                model.UserNameOrEmailAddress,
                model.Password,
                GetTenancyNameOrNull()
            );

            var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));

            return new AuthenticateResultModel
            {
                AccessToken = accessToken,
                ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
                UserId = loginResult.User.Id
            };
        }

        //[HttpGet]
        //public List<ExternalLoginProviderInfoModel> GetExternalAuthenticationProviders()
        //{
        //    return ObjectMapper.Map<List<ExternalLoginProviderInfoModel>>(_externalAuthConfiguration.Providers);
        //}

        //[HttpPost]
        //public async Task<ExternalAuthenticateResultModel> ExternalAuthenticate([FromBody] ExternalAuthenticateModel model)
        //{
        //    var externalUser = await GetExternalUserInfo(model);

        //    var loginResult = await _logInManager.LoginAsync(new UserLoginInfo(model.AuthProvider, model.ProviderKey, model.AuthProvider), GetTenancyNameOrNull());

        //    switch (loginResult.Result)
        //    {
        //        case AbpLoginResultType.Success:
        //        {
        //            var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));
        //            return new ExternalAuthenticateResultModel
        //            {
        //                AccessToken = accessToken,
        //                EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
        //                ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
        //            };
        //        }
        //        case AbpLoginResultType.UnknownExternalLogin:
        //        {
        //            var newUser = await RegisterExternalUserAsync(externalUser);
        //            if (!newUser.IsActive)
        //            {
        //                return new ExternalAuthenticateResultModel
        //                {
        //                    WaitingForActivation = true
        //                };
        //            }

        //            // Try to login again with newly registered user!
        //            loginResult = await _logInManager.LoginAsync(new UserLoginInfo(model.AuthProvider, model.ProviderKey, model.AuthProvider), GetTenancyNameOrNull());
        //            if (loginResult.Result != AbpLoginResultType.Success)
        //            {
        //                throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
        //                    loginResult.Result,
        //                    model.ProviderKey,
        //                    GetTenancyNameOrNull()
        //                );
        //            }

        //            return new ExternalAuthenticateResultModel
        //            {
        //                AccessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity)),
        //                ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
        //            };
        //        }
        //        default:
        //        {
        //            throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
        //                loginResult.Result,
        //                model.ProviderKey,
        //                GetTenancyNameOrNull()
        //            );
        //        }
        //    }
        //}

        //private async Task<User> RegisterExternalUserAsync(ExternalAuthUserInfo externalUser)
        //{
        //    var user = await _userRegistrationManager.RegisterAsync(
        //        externalUser.Name,
        //        externalUser.Surname,
        //        externalUser.EmailAddress,
        //        externalUser.EmailAddress,
        //        Authorization.Users.User.CreateRandomPassword(),
        //        true
        //    );

        //    user.Logins = new List<UserLogin>
        //    {
        //        new UserLogin
        //        {
        //            LoginProvider = externalUser.Provider,
        //            ProviderKey = externalUser.ProviderKey,
        //            TenantId = user.TenantId
        //        }
        //    };

        //    await CurrentUnitOfWork.SaveChangesAsync();

        //    return user;
        //}

        //private async Task<ExternalAuthUserInfo> GetExternalUserInfo(ExternalAuthenticateModel model)
        //{
        //    var userInfo = await _externalAuthManager.GetUserInfo(model.AuthProvider, model.ProviderAccessCode);
        //    if (userInfo.ProviderKey != model.ProviderKey)
        //    {
        //        throw new UserFriendlyException(L("CouldNotValidateExternalUser"));
        //    }

        //    return userInfo;
        //}



        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }

        private async Task<JTLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case JTLoginResultType.Success:
                    return loginResult;
                default:
                    throw new Exception();
            }
        }

        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.UtcNow;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expiration ?? _configuration.Expiration),
                signingCredentials: _configuration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private static List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });

            return claims;
        }

        private string GetEncryptedAccessToken(string accessToken)
        {
            return SimpleStringCipher.Instance.Encrypt(accessToken, "gsKxGZ012HLL3MI5");
        }
    }
}
