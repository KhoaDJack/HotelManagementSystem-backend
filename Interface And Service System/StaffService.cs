using Dapper;
using HotelDBFinal.DomainSystem;
using HotelDBMiddle;
using HotelDBMiddle.Interfaces_And_Service;

namespace HotelDBFinal.InterfaceAndServiceSystem
{
    public class StaffService : IStaffService
    {
        private readonly DapperContext _context;
        private readonly IFileUploadService _fileService;

        public StaffService(DapperContext context, IFileUploadService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<IEnumerable<Staff>> GetAllAsync()
        {
            var query = "SELECT * FROM Staff";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Staff>(query);
            }
        }

        public async Task<Staff> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM Staff WHERE StaffID = @StaffID";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<Staff>(query, new { StaffID = id });
            }
        }

        public async Task<int> CreateAsync(Staff staff)
        {
            string avatarPath = null;

            if (staff.Avatar != null)
            {
                avatarPath = await _fileService.UploadSingleFiles(new[] { "Uploads", "StaffAvatars" }, staff.Avatar);
            }

            var query = @"
            INSERT INTO Staff (FirstName, LastName, Role, HireDate, Avatar)
            VALUES (@FirstName, @LastName, @Role, @HireDate, @Avatar);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
            var parameters = new
            {
                staff.FirstName,
                staff.LastName,
                staff.Role,
                staff.HireDate,
                Avatar = avatarPath
            };
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleAsync<int>(query, parameters);
        }

        public async Task<bool> UpdateAsync(Staff staff)
        {
            string avatarPath = null;

            if (staff.Avatar != null)
            {
                avatarPath = await _fileService.UploadSingleFiles(new[] { "Uploads", "StaffAvatars" }, staff.Avatar);
            }
            else
            {
                var queryGet = "SELECT Avatar FROM Staff WHERE StaffID = @StaffID";
                using var connection = _context.CreateConnection();
                avatarPath = await connection.QueryFirstOrDefaultAsync<string>(queryGet, new { staff.StaffId });
            }

            var query = @"
        UPDATE Staff
        SET FirstName = @FirstName,
            LastName = @LastName,
            Role = @Role,
            HireDate = @HireDate,
            Avatar = @Avatar
        WHERE StaffID = @StaffID;";

            var parameters = new
            {
                staff.FirstName,
                staff.LastName,
                staff.Role,
                staff.HireDate,
                Avatar = avatarPath,
                staff.StaffId
            };

            using var connection2 = _context.CreateConnection();
            var rows = await connection2.ExecuteAsync(query, parameters);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var query = "DELETE FROM Staff WHERE StaffID = @StaffID";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, new { StaffID = id });
                return result > 0;
            }
        }

        public async Task<(IEnumerable<Staff> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            var sql = @"
        SELECT * FROM Staff
        ORDER BY StaffID
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        SELECT COUNT(*) FROM Staff;";

            using var connection = _context.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(sql, new
            {
                Offset = (page - 1) * pageSize,
                PageSize = pageSize
            });

            var items = await multi.ReadAsync<Staff>();
            var totalCount = await multi.ReadFirstAsync<int>();

            return (items, totalCount);
        }

        public async Task<IEnumerable<Staff>> SearchByRoleAsync(string role)
        {
            var sql = "SELECT * FROM Staff WHERE Role LIKE @Role";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Staff>(sql, new { Role = $"%{role}%" });
        }

    }
}