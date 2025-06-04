using Dapper;
using HotelDBFinal.DomainSystem;
using HotelDBMiddle;

namespace HotelDBFinal.InterfaceAndServiceSystem
{
    public class GuestServiceService : IGuestServiceService
    {
        private readonly DapperContext _context;

        public GuestServiceService(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GuestServiceNew>> GetAllAsync()
        {
            var query = "SELECT * FROM GuestServices";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<GuestServiceNew>(query);
        }

        public async Task<GuestServiceNew> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM GuestServices WHERE GuestServiceID = @GuestServiceID";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<GuestServiceNew>(query, new { GuestServiceID = id });
        }

        public async Task<int> CreateAsync(GuestServiceNew guestServiceNew)
        {
            var query = @"
            INSERT INTO GuestServices (BookingID, ServiceID, Quantity)
            VALUES (@BookingID, @ServiceID, @Quantity);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleAsync<int>(query, guestServiceNew);
        }

        public async Task<bool> UpdateAsync(GuestServiceNew guestServiceNew)
        {
            var query = @"
            UPDATE GuestServices
            SET BookingID = @BookingID,
                ServiceID = @ServiceID,
                Quantity = @Quantity
            WHERE GuestServiceID = @GuestServiceID;";
            using var connection = _context.CreateConnection();
            var rows = await connection.ExecuteAsync(query, guestServiceNew);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var query = "DELETE FROM GuestServices WHERE GuestServiceID = @GuestServiceID";
            using var connection = _context.CreateConnection();
            var rows = await connection.ExecuteAsync(query, new { GuestServiceID = id });
            return rows > 0;
        }

        public async Task<IEnumerable<GuestServiceNew>> SearchAsync(int? bookingId, int? serviceId)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
        SELECT * FROM GuestServices
        WHERE (@BookingId IS NULL OR BookingID = @BookingId)
          AND (@ServiceId IS NULL OR ServiceID = @ServiceId)
        ORDER BY GuestServiceID";

            var parameters = new
            {
                BookingId = bookingId,
                ServiceId = serviceId
            };
            return await connection.QueryAsync<GuestServiceNew>(sql, parameters);
        }

        public async Task<(IEnumerable<GuestServiceNew> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            var sql = @"
    SELECT * FROM GuestServices
    ORDER BY GuestServiceId
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    SELECT COUNT(*) FROM GuestServices;";

            using var connection = _context.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(sql, new
            {
                Offset = (page - 1) * pageSize,
                PageSize = pageSize
            });

            // Use GuestServiceNew here to match your model class
            var items = await multi.ReadAsync<GuestServiceNew>();
            var totalCount = await multi.ReadFirstAsync<int>();

            return (items, totalCount);
        }
    }
}