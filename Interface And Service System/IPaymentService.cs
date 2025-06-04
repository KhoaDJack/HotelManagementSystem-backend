using HotelDBFinal.DomainSystem;

namespace HotelDBFinal.InterfaceAndServiceSystem
{
    public interface IPaymentService
    {
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<Payment> GetByIdAsync(int id);
        Task<int> CreateAsync(Payment payment);
        Task<bool> UpdateAsync(Payment payment);
        Task<bool> DeleteAsync(int id);
        Task<(IEnumerable<Payment> Items, int TotalCount)> GetPagedAndFilteredAsync(
    int page, int pageSize, int? bookingId, string? paymentMethod);
    }
}