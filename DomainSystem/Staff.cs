namespace HotelDBFinal.DomainSystem
{
    public class Staff
    {
        public int StaffId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Role { get; set; }
        public DateTime HireDate { get; set; }
        public IFormFile? Avatar { get; set; }
        public string? AvatarPath { get; set; }
    }
}