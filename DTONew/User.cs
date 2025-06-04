using System.Text.Json.Serialization;

namespace HotelDBFinal.DTONew
{
    public class User : BaseEntity<int>
    {
        public string? Email { get; set; }
        [JsonIgnore]
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateofBirth { get; set; }
        public string? Avatar { get; set; }
        public string? RefreshToken { get; set; }
        public bool? IsDisabled { get; set; }
        public bool? EmailVerified { get; set; }
        public bool? Gender { get; set; }
        public DateTime? LastLogin { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        //public int RoleId {get; set;}
        public virtual List<Role>? Roles { get; set; }
    }
}