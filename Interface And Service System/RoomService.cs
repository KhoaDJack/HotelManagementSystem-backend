using Dapper;
using HotelDBFinal.DomainSystem;
using HotelDBMiddle;
using HotelDBMiddle.Interfaces_And_Service;

namespace HotelDBFinal.InterfaceAndServiceSystem
{
    public class RoomService : IRoomService
    {
        private readonly DapperContext _context;
        private readonly IFileUploadService _fileUploadService;

        public RoomService(DapperContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            var query = "SELECT * FROM Rooms";
            using (var connection = _context.CreateConnection())
            {
                var rooms = await connection.QueryAsync<Room>(query);
                return rooms;
            }
        }

        public async Task<Room> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM Rooms WHERE RoomID = @RoomID";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<Room>(query, new { RoomID = id });
            }
        }

        public async Task<int> CreateAsync(Room room)
        {
            string? avatarPath = null;
            if (room.Avatar != null)
            {
                avatarPath = await _fileUploadService.UploadSingleFiles(new[] { "uploads", "rooms" }, room.Avatar);
            }

            var query = "INSERT INTO Rooms (RoomNumber, RoomType, Capacity, PricePerNight, Avatar) VALUES (@RoomNumber, @RoomType, @Capacity, @PricePerNight, @Avatar); SELECT CAST(SCOPE_IDENTITY() as int);";
            using (var connection = _context.CreateConnection())
            {
                var parameters = new
                {
                    room.RoomNumber,
                    room.RoomType,
                    room.Capacity,
                    room.PricePerNight,
                    Avatar = avatarPath
                };
                return await connection.ExecuteScalarAsync<int>(query, parameters);
            }
        }

        public async Task<bool> UpdateAsync(Room room)
        {
            string? avatarPath = null;
            if (room.Avatar != null)
            {
                avatarPath = await _fileUploadService.UploadSingleFiles(new[] { "uploads", "rooms" }, room.Avatar);
            }
            else
            {
                // If no new avatar uploaded, keep the old avatar path from DB
                var queryGetAvatar = "SELECT Avatar FROM Rooms WHERE RoomID = @RoomID";
                using var connection = _context.CreateConnection();
                avatarPath = await connection.QueryFirstOrDefaultAsync<string>(queryGetAvatar, new { room.RoomId });
            }

            var queryUpdate = @"
        UPDATE Rooms
        SET RoomNumber = @RoomNumber,
            RoomType = @RoomType,
            Capacity = @Capacity,
            PricePerNight = @PricePerNight,
            Avatar = @Avatar
        WHERE RoomID = @RoomID";

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(queryUpdate, new
                {
                    room.RoomId,
                    room.RoomNumber,
                    room.RoomType,
                    room.Capacity,
                    room.PricePerNight,
                    Avatar = avatarPath
                });

                return result > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var query = "DELETE FROM Rooms WHERE RoomID = @RoomID";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, new { RoomID = id });
                return result > 0;
            }
        }

        public async Task<(IEnumerable<Room> Items, int TotalCount)> GetPagedAndSearchedAsync(int page, int pageSize, string? query)
        {
            using var connection = _context.CreateConnection();

            var countSql = @"
        SELECT COUNT(*)
        FROM Rooms
        WHERE (@query IS NULL OR RoomNumber LIKE '%' + @query + '%' OR RoomType LIKE '%' + @query + '%')";

            var dataSql = @"
        SELECT *
        FROM Rooms
        WHERE (@query IS NULL OR RoomNumber LIKE '%' + @query + '%' OR RoomType LIKE '%' + @query + '%')
        ORDER BY RoomID
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { query });
            var items = await connection.QueryAsync<Room>(dataSql, new
            {
                query,
                Offset = (page - 1) * pageSize,
                PageSize = pageSize
            });

            return (items, totalCount);
        }

    }
}