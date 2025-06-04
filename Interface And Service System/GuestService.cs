using Dapper;
using HotelDBFinal.DomainSystem;
using HotelDBMiddle;

namespace HotelDBFinal.InterfaceAndServiceSystem
{
    public class GuestService : IGuestService
    {
        private readonly DapperContext _context;

        public GuestService(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Guest>> GetAllAsync()
        {
            var query = "SELECT * FROM Guests";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Guest>(query);
        }

        public async Task<Guest> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM Guests WHERE GuestID = @GuestID";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Guest>(query, new { GuestID = id });
        }

        public async Task<int> CreateAsync(Guest guest)
        {
            var query = @"INSERT INTO Guests(FirstName, LastName, Email, Phone) VALUES (@FirstName, @LastName, @Email, @Phone);
                      SELECT CAST(SCOPE_IDENTITY() as int)";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleAsync<int>(query, guest);
        }

        public async Task<bool> UpdateAsync(Guest guest)
        {
            var query = @"UPDATE Guests
        SET FirstName = @FirstName,
        LastName = @LastName,
        Email = @Email,
        Phone = @Phone
        WHERE GuestID = @GuestID";
            using var connection = _context.CreateConnection();
            var rows = await connection.ExecuteAsync(query, guest);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var query = "DELETE FROM Guests WHERE GuestID = @GuestID";
            using var connection = _context.CreateConnection();
            var rows = await connection.ExecuteAsync(query, new { GuestID = id });
            return rows > 0;
        }

        public async Task<(IEnumerable<Guest> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            var offset = (page - 1) * pageSize;

            var sqlData = @"SELECT * FROM Guests
                    ORDER BY GuestId
                    OFFSET @Offset ROWS
                    FETCH NEXT @PageSize ROWS ONLY";

            var sqlCount = @"SELECT COUNT(*) FROM Guests";

            using var connection = _context.CreateConnection();

            var totalCount = await connection.ExecuteScalarAsync<int>(sqlCount);
            var items = await connection.QueryAsync<Guest>(sqlData, new { Offset = offset, PageSize = pageSize });

            return (items, totalCount);
        }

        public async Task<IEnumerable<Guest>> SearchByLastNameAsync(string lastName)
        {
            var sql = "SELECT * FROM Guests WHERE LastName LIKE @LastName";

            using var connection = _context.CreateConnection();
            var guests = await connection.QueryAsync<Guest>(sql, new { LastName = $"%{lastName}%" });
            return guests;
        }

    }
}