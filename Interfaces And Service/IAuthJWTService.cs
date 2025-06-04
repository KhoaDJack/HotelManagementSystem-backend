using HotelDBFinal.DTONew;

using HotelDBMiddle.Infrastructure.Models;

namespace HotelDBMiddle.Interfaces_And_Service
{
    public interface IAuthJWTService
    {
        Task<User> SignUp(AuthDTO dto);
        Task<(User, TokenModel)> SignIn(AuthDTO dto);
        TokenModel RefreshToken(string accessToken);
        
    }
}