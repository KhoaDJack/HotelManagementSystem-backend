namespace HotelDBFinal.DomainSystem
{
    public class Booking
    {
    public int BookingId { get; set; }
    public int GuestId { get; set; }
    public int RoomId { get; set; }
    public DateTime CheckInDate { get; set; } = DateTime.MinValue;
    public DateTime CheckOutDate { get; set; }
    public string BookingStatus { get; set; }
    }
}