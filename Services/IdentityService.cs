using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using project.Authentication;
using project.Data;
using project.Models;
using project.ViewModel;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace project.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppSettings _appSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ApplicationDbContext _Context;
        public IdentityService(UserManager<IdentityUser> userManager, AppSettings appSettings, TokenValidationParameters tokenValidationParameters, ApplicationDbContext Context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _appSettings = appSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _Context = Context;
        }
        //Register to site
        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User with this email address already exists" }
                };
            }
            IdentityUser newUser = new IdentityUser
            {
                Email = email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = email
            };
            var CreatedUser = await _userManager.CreateAsync(newUser, password);
            if (!CreatedUser.Succeeded)
            {

                return new AuthenticationResult
                {
                    Errors = CreatedUser.Errors.Select(x => x.Description)
                };
            }
            return await GenerateAuthenticationResultForUserAsync(newUser);
        }
        //Regster Admain to site
        public async Task<AuthenticationResult> RegisterAdminAsync(string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User with this email address already exists" }
                };
            }
            IdentityUser newUser = new IdentityUser
            {
                Email = email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = email
            };
            var CreatedUser = await _userManager.CreateAsync(newUser, password);
            if (!CreatedUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Errors = CreatedUser.Errors.Select(x => x.Description)
                };
            }
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.Admin);
            }
            return await GenerateAuthenticationResultForUserAsync(newUser);
        }
        // login to site
        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User does not exist" }
                };
            }

            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!userHasValidPassword)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User/Password combination is wrong" }
                };
            }
            return await GenerateAuthenticationResultForUserAsync(user);
        }
        //RefreshToken
        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string RefreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);
            if (validatedToken == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Invalid Token" }
                };
            }
            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix);
            if (expiryDateUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This Token has not expired yet" }
                };
            }
            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            var storedRefrehToken = await _Context.refreshTokens.SingleOrDefaultAsync(x => x.Token == RefreshToken);
            if (storedRefrehToken == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This Refresh Token dose not exist" }
                };
            }
            if (DateTime.UtcNow > storedRefrehToken.ExpiryDate)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This Refresh Token has exist" }
                };
            }
            if (storedRefrehToken.Invalidated)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This Refresh Token has been Invalidated" }
                };
            }
            if (storedRefrehToken.Used)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This Refresh Token has been Used" }
                };
            }
            storedRefrehToken.Used = true;
            _Context.refreshTokens.Update(storedRefrehToken);
            await _Context.SaveChangesAsync();
            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await GenerateAuthenticationResultForUserAsync(user);
        }
        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHander = new JwtSecurityTokenHandler();
            try
            {
                var Principal = tokenHander.ValidateToken(token, _tokenValidationParameters, out var validatediontoken);
                if (!IsJwtWithValidSecurityAlgorithm(validatediontoken))
                {
                    return null;
                }
                return Principal;
            }
            catch
            {
                return null;
            }
        }
        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken ValidatedToken)
        {
            return (ValidatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }
        //Generte RefreshToken and AccessToken
        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var Key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var claims = new List<Claim>
            {
                    new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                    new Claim("id",user.Id),
            };
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role == null) continue;
                var roleClaims = await _roleManager.GetClaimsAsync(role);

                foreach (var roleClaim in roleClaims)
                {
                    if (claims.Contains(roleClaim))
                        continue;

                    claims.Add(roleClaim);
                }
            }

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_appSettings.Tokenlifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescription);
            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationData = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMinutes(6)
            };

            await _Context.refreshTokens.AddAsync(refreshToken);
            await _Context.SaveChangesAsync();
            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }


    }
}
