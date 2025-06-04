using HotelDBFinal.DomainSystem;

namespace HotelDBFinal.InterfaceAndServiceSystem
{
    public interface IGuestServiceService
    {
        Task<IEnumerable<GuestServiceNew>> GetAllAsync();
        Task<GuestServiceNew> GetByIdAsync(int id);
        Task<int> CreateAsync(GuestServiceNew guestServiceNew);
        Task<bool> UpdateAsync(GuestServiceNew guestServiceNew);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<GuestServiceNew>> SearchAsync(int? bookingId, int? serviceId);
        Task<(IEnumerable<GuestServiceNew> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
    }
}