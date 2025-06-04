using System.Data;
using System.Security.Claims;
using Dapper;
using HotelDBFinal.DTONew;
using HotelDBMiddle;
using HotelDBMiddle.Infrastructure.Models;
using HotelDBMiddle.Interfaces_And_Service;

namespace HotelDBFinal.Interfaces_And_Service
{
    public class AuthJWTService : IAuthJWTService
    {
        private readonly IDbConnection _db;
        private readonly IJwtService _jwtService;
        public AuthJWTService(IDbConnection db, IJwtService jwtService )
        {
            _db = db;
            _jwtService = jwtService;
        }

        public TokenModel RefreshToken(string accessToken)
        {
            ClaimsPrincipal principal;
            try
            {
                principal = _jwtService.GetPrincipalFromExpiredToken(accessToken);//bỏ qua thời gian sống
                var newAccessToken = _jwtService.GenerateAccessToken(principal.Claims);//đưa claims mã hóa lại
                var newRefreshToken = _jwtService.GenerateRefreshToken();
                var tokenModel =  new TokenModel(newAccessToken, newRefreshToken);
                return tokenModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<(User, TokenModel)> SignIn(AuthDTO dto)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Email", dto.Email);
                parameters.Add("@Password", dto.Password);
                using var multi = await _db.QueryMultipleAsync("Users_CheckLogin", parameters, commandType: CommandType.StoredProcedure);

                var user = await multi.ReadSingleAsync<User>();
                var roles = (await multi.ReadAsync<Role>()).ToList();
                user.Roles = roles;

                //tạo token
                // Tạo danh sách claims cần thiết
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Email, user.Email),
                        // Thêm các claim khác nếu cần
                    };
                foreach (var item in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, item.Name));
                }
                var accessToken = _jwtService.GenerateAccessToken(claims);
                var refreshToken = _jwtService.GenerateRefreshToken();
                var tokenModel = new TokenModel(accessToken, refreshToken);
                return (user, tokenModel);
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> SignUp(AuthDTO dto)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Email", dto.Email);
                parameters.Add("@Password", dto.Password);
                var result = await _db.QueryFirstOrDefaultAsync<User>("Users_Create", parameters, commandType: CommandType.StoredProcedure
                );
                return result!;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}