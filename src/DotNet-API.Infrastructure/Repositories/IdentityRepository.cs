using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DotNet_API.Application.Repositories;
using DotNet_API.Domain.Entities;
using DotNet_API.Domain.Options;
using DotNet_API.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DotNet_API.Infrastructure.Repositories
{
    public class IdentityRepository : IIdentityRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ApplicationDbContext _dbContext;

        public IdentityRepository(UserManager<IdentityUser> userManager,
            JwtSettings jwtSettings,
            TokenValidationParameters tokenValidationParameters,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _dbContext = dbContext;
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new AuthenticationResult()
                {
                    Errors = new[] {"User does not exist"}
                };
            }

            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!userHasValidPassword)
            {
                return new AuthenticationResult()
                {
                    Errors = new[] {"User/password is wrong"}
                };
            }

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            var existedUser = await _userManager.FindByEmailAsync(email);

            if (existedUser != null)
            {
                return new AuthenticationResult()
                {
                    Errors = new[] {"User with this Email is already exist"}
                };
            }

            var newUser = new IdentityUser()
            {
                Email = email,
                UserName = email
            };
            var createUser = await _userManager.CreateAsync(newUser, password);

            if (!createUser.Succeeded)
            {
                return new AuthenticationResult()
                {
                    Errors = createUser.Errors.Select(x => x.Description)
                };
            }

            return await GenerateAuthenticationResultForUserAsync(newUser);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);

            if (validatedToken == null)
            {
                return new AuthenticationResult()
                {
                    Errors = new[] {"Invalid Token"}
                };
            }

            var expiryDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult()
                {
                    Errors = new[] {"This Token hasn't expired yet"}
                };
            }


            var storeRefreshToken = await _dbContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

            if (storeRefreshToken == null)
            {
                return new AuthenticationResult()
                {
                    Errors = new[] {"This refresh Token does not existed "}
                };
            }

            if (DateTime.UtcNow > storeRefreshToken.ExpiredDate)
            {
                return new AuthenticationResult()
                {
                    Errors = new[] {"This refresh Token has expired "}
                };
            }

            if (storeRefreshToken.Invalidated)
            {
                return new AuthenticationResult()
                {
                    Errors = new[] {"This refresh Token is invalidated "}
                };
            }

            if (storeRefreshToken.Used)
            {
                return new AuthenticationResult()
                {
                    Errors = new[] {"This refresh Token is concurrently in use "}
                };
            }
            
            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            if (storeRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult()
                {
                    Errors = new[] {"This refresh Token does not match this JWT"}
                };
            }

            storeRefreshToken.Used = true;
            _dbContext.RefreshTokens.Update(storeRefreshToken);
            await _dbContext.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await GenerateAuthenticationResultForUserAsync(user);
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("id", user.Id)
                }),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = new RefreshToken()
            {
                Token = Guid.NewGuid().ToString(),
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.Now,
                ExpiredDate = DateTime.UtcNow.AddMonths(6)
            };
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();
            return new AuthenticationResult()
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token    
            };
        }
    }
}