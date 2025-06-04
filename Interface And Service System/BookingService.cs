using System.Data;
using System.Text;
using Dapper;
using HotelDBFinal.DomainSystem;
using HotelDBMiddle;

namespace HotelDBFinal.InterfaceAndServiceSystem
{
    public class BookingService : IBookingService
    {
        private readonly DapperContext _context;

        public BookingService(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            var query = "SELECT * FROM Bookings";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Booking>(query);
        }

        public async Task<Booking> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM Bookings WHERE BookingID = @BookingID";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Booking>(query, new { BookingID = id });
        }

        public async Task<int> CreateAsync(Booking booking)
        {
            var query = @"
            INSERT INTO Bookings (GuestID, RoomID, CheckInDate, CheckOutDate, BookingStatus)
            VALUES (@GuestID, @RoomID, @CheckInDate, @CheckOutDate, @BookingStatus);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleAsync<int>(query, booking);
        }

        public async Task<bool> UpdateAsync(Booking booking)
        {
            var query = @"
            UPDATE Bookings
            SET GuestID = @GuestID,
                RoomID = @RoomID,
                CheckInDate = @CheckInDate,
                CheckOutDate = @CheckOutDate,
                BookingStatus = @BookingStatus
            WHERE BookingID = @BookingID;";
            using var connection = _context.CreateConnection();
            var rows = await connection.ExecuteAsync(query, booking);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var query = "DeleteBooking"; // stored procedure name
            using var connection = _context.CreateConnection();

            var affectedRows = await connection.ExecuteAsync(
                query,
                new { BookingID = id },
                commandType: CommandType.StoredProcedure
            );

            return affectedRows > 0;
        }

        public async Task<IEnumerable<Booking>> SearchAsync(int? guestId, int? roomId)
        {
            var sql = new StringBuilder("SELECT * FROM Bookings WHERE 1=1");
            var parameters = new DynamicParameters();

            if (guestId.HasValue)
            {
                sql.Append(" AND GuestID = @GuestID");
                parameters.Add("GuestID", guestId.Value);
            }
            if (roomId.HasValue)
            {
                sql.Append(" AND RoomID = @RoomID");
                parameters.Add("RoomID", roomId.Value);
            }

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Booking>(sql.ToString(), parameters);
        }

        public async Task<(IEnumerable<Booking> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            var countSql = "SELECT COUNT(*) FROM Bookings";

            var dataSql = @"SELECT * FROM Bookings
                    ORDER BY BookingID
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new DynamicParameters();
            parameters.Add("Offset", (page - 1) * pageSize);
            parameters.Add("PageSize", pageSize);

            using var connection = _context.CreateConnection();

            var totalCount = await connection.ExecuteScalarAsync<int>(countSql);
            var items = await connection.QueryAsync<Booking>(dataSql, parameters);

            return (items, totalCount);
        }
    }
}