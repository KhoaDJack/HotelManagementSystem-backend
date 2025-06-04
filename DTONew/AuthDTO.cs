using System.ComponentModel.DataAnnotations;

namespace HotelDBFinal.DTONew
{
    public class AuthDTO
    {
        [EmailAddress]
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}