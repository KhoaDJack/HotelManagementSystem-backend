namespace HotelDBFinal.DomainSystem
{
    public class Room
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerNight { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}