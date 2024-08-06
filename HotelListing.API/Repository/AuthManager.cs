using AutoMapper;
using HotelListing.API.Contracts;
using HotelListing.API.Controllers;
using HotelListing.API.Data;
using HotelListing.API.Models.Country;
using HotelListing.API.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelListing.API.Repository
{
    public class AuthManager(IMapper mapper, UserManager<ApiUser> userManager, IConfiguration configuration, ILogger<AuthManager> logger) : IAuthManager
    {
        private const string _loginProvider = "HotelListingApi";
        private const string _refreshTokenName = "RefreshToken";
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<ApiUser> _userManager = userManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<AuthManager> _logger = logger;
        private ApiUser _user;

        public async Task<string> CreateRefreshToken()
        {
            await _userManager.RemoveAuthenticationTokenAsync(_user, _loginProvider, _refreshTokenName);
            var newRefreshToken = await _userManager.GenerateUserTokenAsync(_user, _loginProvider, _refreshTokenName);
            var result = await _userManager.SetAuthenticationTokenAsync(_user, _loginProvider, _refreshTokenName, newRefreshToken);
            return newRefreshToken;
        }

        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {

            _user = await _userManager.FindByEmailAsync(loginDto.Email);
            
            if (_user!= null && await _userManager.CheckPasswordAsync(_user, loginDto.Password))
            {
                var token = await GenerateToken();
                _logger.LogInformation($"Token generated for the user {loginDto.Email}");
                return new AuthResponseDto
                {
                    UserID = _user.Id,
                    Token = token,
                    RefreshToken = await CreateRefreshToken()
                };
            }

            return null;
        }

        public async Task<IEnumerable<IdentityError>> Register(ApiUserDto apiUserDto)
        {                    
            _user  = _mapper.Map<ApiUser>(apiUserDto);
            _user.UserName = apiUserDto.Email;
            var result = await _userManager.CreateAsync(_user, apiUserDto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(_user, "User");
            }

            return result.Errors;
        }

        public async Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);
            var userName = tokenContent.Claims.FirstOrDefault(q => q.Type == JwtRegisteredClaimNames.Email)?.Value;
            _user = await _userManager.FindByEmailAsync(userName);
            if (_user == null || _user.Id != request.UserID)
                return null;
            var isValid = await _userManager.VerifyUserTokenAsync(_user, _loginProvider, _refreshTokenName, request.Token);
            if (isValid)
            {
                var token = await GenerateToken();
                return new AuthResponseDto
                {
                    Token = token,
                    UserID = _user.Id,
                    RefreshToken = await CreateRefreshToken()
                };
            }
            await _userManager.UpdateSecurityStampAsync(_user);
            return null;
        }

        private async Task<string> GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var credentials =  new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var roles = await _userManager.GetRolesAsync(_user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role,x)).ToList();
            var userClaims = await _userManager.GetClaimsAsync(_user);
            var claims = new List<Claim>()
            {
                new(JwtRegisteredClaimNames.Sub, _user.Email ),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() ),
                new(JwtRegisteredClaimNames.Email, _user.Email ),
                new("uid", _user.Id)
            }.Union(userClaims).Union(roleClaims);
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
