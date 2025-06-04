using Dapper;
using HotelDBFinal.DomainSystem;
using HotelDBMiddle;

namespace HotelDBFinal.InterfaceAndServiceSystem
{
    public class PaymentService : IPaymentService
    {
        private readonly DapperContext _context;

        public PaymentService(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            var query = "SELECT * FROM Payments";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Payment>(query);
        }

        public async Task<Payment> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM Payments WHERE PaymentID = @PaymentID";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Payment>(query, new { PaymentID = id });
        }

        public async Task<int> CreateAsync(Payment payment)
        {
            var query = @"
            INSERT INTO Payments (BookingID, PaymentDate, Amount, PaymentMethod)
            VALUES (@BookingID, @PaymentDate, @Amount, @PaymentMethod);
            SELECT CAST(SCOPE_IDENTITY() as int)";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleAsync<int>(query, payment);
        }

        public async Task<bool> UpdateAsync(Payment payment)
        {
            var query = @"
            UPDATE Payments
            SET BookingID = @BookingID,
                PaymentDate = @PaymentDate,
                Amount = @Amount,
                PaymentMethod = @PaymentMethod
            WHERE PaymentID = @PaymentID";
            using var connection = _context.CreateConnection();
            var rows = await connection.ExecuteAsync(query, payment);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var query = "DELETE FROM Payments WHERE PaymentID = @PaymentID";
            using var connection = _context.CreateConnection();
            var rows = await connection.ExecuteAsync(query, new { PaymentID = id });
            return rows > 0;
        }

        public async Task<(IEnumerable<Payment> Items, int TotalCount)> GetPagedAndFilteredAsync(
    int page, int pageSize, int? bookingId, string? paymentMethod)
        {
            using var connection = _context.CreateConnection();

            var dataSql = @"
        SELECT * FROM Payments
        WHERE (@BookingID IS NULL OR BookingID = @BookingID)
          AND (@PaymentMethod IS NULL OR PaymentMethod LIKE '%' + @PaymentMethod + '%')
        ORDER BY PaymentID
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var countSql = @"
        SELECT COUNT(*) FROM Payments
        WHERE (@BookingID IS NULL OR BookingID = @BookingID)
          AND (@PaymentMethod IS NULL OR PaymentMethod LIKE '%' + @PaymentMethod + '%')";
            var parameters = new
            {
                BookingID = bookingId,
                PaymentMethod = paymentMethod,
                Offset = (page - 1) * pageSize,
                PageSize = pageSize
            };
            var items = await connection.QueryAsync<Payment>(dataSql, parameters);
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
            return (items, totalCount);
        }

    }
}