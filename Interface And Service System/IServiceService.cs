using HotelDBFinal.DomainSystem;

namespace HotelDBFinal.InterfaceAndServiceSystem
{
    public interface IServiceService
    {
    Task<IEnumerable<Service>> GetAllAsync();
    Task<Service> GetByIdAsync(int id);
    Task<int> CreateAsync(Service service);
    Task<bool> UpdateAsync(Service service);
    Task<bool> DeleteAsync(int id);
    Task<(IEnumerable<Service> Items, int TotalCount)> GetPagedServicesAsync(int page, int pageSize);

    }
}