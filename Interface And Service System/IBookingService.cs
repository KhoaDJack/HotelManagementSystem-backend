using HotelDBFinal.DomainSystem;

namespace HotelDBFinal.InterfaceAndServiceSystem
{
    public interface IBookingService
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<Booking> GetByIdAsync(int id);
        Task<int> CreateAsync(Booking booking);
        Task<bool> UpdateAsync(Booking booking);
        Task<bool> DeleteAsync(int id);
        Task<(IEnumerable<Booking> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
        Task<IEnumerable<Booking>> SearchAsync(int? guestId, int? roomId);
    }
}