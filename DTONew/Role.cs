namespace HotelDBFinal.DTONew
{
    public class Role: BaseEntity<int>
    {
        public string? Name { get; set; }
        public string? Description { get; set;}
        public List<User>? Users { get; set; }
    }
}