using HotelDBFinal.ControllerSystem;
using HotelDBFinal.DomainSystem;

namespace HotelDBFinal.InterfaceAndServiceSystem
{
    public interface IStaffService
    {
        Task<IEnumerable<Staff>> GetAllAsync();
        Task<Staff> GetByIdAsync(int id);
        Task<int> CreateAsync(Staff staff);
        Task<bool> UpdateAsync(Staff staff);
        Task<bool> DeleteAsync(int id);
        Task<(IEnumerable<Staff> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
        Task<IEnumerable<Staff>> SearchByRoleAsync(string role);
    }
}