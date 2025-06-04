using System.Text.Json.Serialization;

namespace HotelDBFinal.DTONew
{
    public class Hotel: BaseEntity<int>
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Location { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? Notes { get; set; } 
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
        public int? Quantity { get; set; }
        public int? Stars { get; set; }
        public string? Thumbnail { get; set; }
        [JsonIgnore]
        public string? Images { get; set; } //["image1.jpg","images2.jpg"]
        public int Floor { get; set; }
        public DateTime? CheckInTime { get; set; } = DateTime.Now;
        public DateTime CheckOutTime { get; set; } = DateTime.Now;
    }
}