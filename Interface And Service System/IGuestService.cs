using HotelDBFinal.DomainSystem;

namespace HotelDBFinal.InterfaceAndServiceSystem
{
    public interface IGuestService
    {
        Task<IEnumerable<Guest>> GetAllAsync();
        Task<Guest> GetByIdAsync(int id);
        Task<int> CreateAsync(Guest guest);
        Task<bool> UpdateAsync(Guest guest);
        Task<bool> DeleteAsync(int id);
        Task<(IEnumerable<Guest> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
        Task<IEnumerable<Guest>> SearchByLastNameAsync(string lastName);
    }
}