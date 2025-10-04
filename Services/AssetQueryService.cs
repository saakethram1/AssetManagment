using AssetManagment.Models.DTO;
using System.Data;
using Dapper;
using AssetManagment.Services.Interfaces;
namespace AssetManagment.Services
{
    public class AssetQueryService: IAssetQueryService
    {
        private readonly IDbConnection _conn;
        public AssetQueryService(IDbConnection conn) => _conn = conn;

        public async Task<PagedResult<AssetSearchResult>> SearchAssetsPagedAsync(
       List<string>? assetTypes,
       List<int>? statuses,
       string? serialSearch,
       int pageNumber,
       int pageSize)
        {
            // normalize empty lists to null so we don't emit IN ()
            assetTypes = (assetTypes?.Count > 0) ? assetTypes : null;
            statuses = (statuses?.Count > 0) ? statuses : null;

            var from = "FROM Assets a";

            // build WHERE dynamically
            var whereParts = new List<string>();
            if (assetTypes != null) whereParts.Add("a.AssetType IN @assetTypes");
            if (statuses != null) whereParts.Add("a.Status IN @statuses");

            // serialSearch used as a LIKE filter (case-insensitive)
            // Normalize the search term once (to lower-case) and include wildcards.
            string? serialSearchParam = null;
            if (!string.IsNullOrWhiteSpace(serialSearch))
            {
                var trimmed = serialSearch.Trim();
                if (trimmed.Length > 0)
                {
                    // normalize to lower once; we'll compare with LOWER(a.SerialNumber)
                    serialSearchParam = $"%{trimmed.ToLowerInvariant()}%";
                    // Use LOWER on the column only (not the parameter)
                    whereParts.Add("LOWER(a.SerialNumber) LIKE @serialSearch");
                }
            }

            var where = whereParts.Count > 0
                        ? "WHERE " + string.Join(" AND ", whereParts)
                        : string.Empty;

            var countSql = $@"
SELECT COUNT(1)
{from}
{where};
";

            var itemsSql = $@"
SELECT a.Id,
       a.AssetName,
       a.AssetType,
       a.Model,
       a.SerialNumber,
       a.PurchaseDate,
       a.WarrantyExpiryDate,
       a.IsSpare,
       a.Condition,
       a.Status
{from}
{where}
ORDER BY a.AssetName
OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;
";

            // Guard pageNumber to be at least 1
            var safePageNumber = pageNumber <= 0 ? 1 : pageNumber;
            var parms = new
            {
                assetTypes,
                statuses,
                serialSearch = serialSearchParam,
                skip = (safePageNumber - 1) * pageSize,
                take = pageSize
            };

            // Optional: helpful logging for debugging
            Console.WriteLine("COUNT SQL:\n" + countSql);
            Console.WriteLine("ITEMS SQL:\n" + itemsSql);
            Console.WriteLine("PARAMS: assetTypes count = " + (assetTypes?.Count ?? 0)
                              + ", statuses count = " + (statuses?.Count ?? 0)
                              + ", serialSearch = '" + serialSearchParam + "'"
                              + ", skip = " + parms.skip + ", take = " + parms.take);

            var total = await _conn.ExecuteScalarAsync<int>(countSql, parms);
            var items = (await _conn.QueryAsync<AssetSearchResult>(itemsSql, parms)).ToList();

            return new PagedResult<AssetSearchResult>
            {
                Items = items,
                TotalCount = total,
                PageNumber = safePageNumber,
                PageSize = pageSize
            };
        }



        public async Task<PagedResult<AssignmentHistory>> SearchAssignmentHistoryPagedAsync(
        List<string>? assetTypes,
        List<int>? statuses,
        string serialSearch,
        string employeeSearch,
        string? sortBy,
        bool sortDesc,
        int pageNumber,
        int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize <= 0) pageSize = 20;

            assetTypes = (assetTypes?.Count > 0) ? assetTypes : null;
            statuses = (statuses?.Count > 0) ? statuses : null;

            // Make sure LIKE searches have wildcards (dev: you can also pass already-wildcarded values)
            serialSearch = string.IsNullOrWhiteSpace(serialSearch) ? null : $"%{serialSearch.Trim()}%";
            employeeSearch = string.IsNullOrWhiteSpace(employeeSearch) ? null : $"%{employeeSearch.Trim()}%";

            var sortColumn = sortBy switch
            {
                "AssetName" => "a.AssetName",
                "AssetType" => "a.AssetType",
                "SerialNumber" => "a.SerialNumber",
                "EmployeeName" => "e.FullName",
                "AssignedDate" => "aa.AssignedDate",
                "ReturnedDate" => "aa.ReturnedDate",
                _ => "aa.AssignedDate"
            };
            var sortDir = sortDesc ? "DESC" : "ASC";

            var where = @"
        WHERE (@assetTypes IS NULL OR a.AssetType IN @assetTypes)
          AND (@statuses IS NULL OR a.Status IN @statuses)
          AND (@serialSearch IS NULL OR LOWER(a.SerialNumber) LIKE LOWER(@serialSearch))
          AND (@employeeSearch IS NULL OR LOWER(e.FullName) LIKE LOWER(@employeeSearch))
    ";

            var countSql = $@"
        SELECT COUNT(1)
        FROM AssetAssignments aa
        INNER JOIN Assets a ON a.Id = aa.AssetId
        INNER JOIN Employees e ON e.Id = aa.EmployeeId
        {where}
    ";

            var itemsSql = $@"
        SELECT aa.Id AS AssignmentId,
               a.Id AS AssetId,
               a.AssetName,
               a.AssetType,
               a.SerialNumber,
               a.Status AS StatusInt,
               e.Id AS EmployeeId,
               e.FullName AS EmployeeName,
               aa.AssignedDate,
               aa.ReturnedDate,
               aa.Notes
        FROM AssetAssignments aa
        INNER JOIN Assets a ON a.Id = aa.AssetId
        INNER JOIN Employees e ON e.Id = aa.EmployeeId
        {where}
        ORDER BY {sortColumn} {sortDir}
        OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;
    ";

            var parms = new
            {
                assetTypes,
                statuses,
                serialSearch,
                employeeSearch,
                skip = (pageNumber - 1) * pageSize,
                take = pageSize
            };

          
            var total = await _conn.ExecuteScalarAsync<int>(countSql, parms);
            var items = (await _conn.QueryAsync<AssignmentHistory>(itemsSql, parms)).ToList();

          

  
            return new PagedResult<AssignmentHistory>
            {
                Items = items,
                TotalCount = total,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        public async Task<AssetDashboard> GetDashboardAsync()
        {
            var sql = @"
                SELECT COUNT(*) FROM Assets;
                SELECT COUNT(*) FROM Assets WHERE Status = @assigned;
                SELECT COUNT(*) FROM Assets WHERE Status = @available;
                SELECT COUNT(*) FROM Assets WHERE Status = @repair;
                SELECT COUNT(*) FROM Assets WHERE Status = @retired;
                SELECT COUNT(*) FROM Assets WHERE IsSpare = 1;
                SELECT ISNULL(AssetType,'(Unknown)') AS AssetType, COUNT(*) AS Count FROM Assets GROUP BY ISNULL(AssetType,'(Unknown)');
                ";
            using var multi = await _conn.QueryMultipleAsync(sql, new
            {
                assigned = (int)AssetManagment.Models.Enums.AssetStatus.Assigned,
                available = (int)AssetManagment.Models.Enums.AssetStatus.Available,
                repair = (int)AssetManagment.Models.Enums.AssetStatus.UnderRepair,
                retired = (int)AssetManagment.Models.Enums.AssetStatus.Retired
            });

            var dto = new AssetDashboard
            {
                TotalAssets = multi.ReadSingle<int>(),
                AssignedAssets = multi.ReadSingle<int>(),
                AvailableAssets = multi.ReadSingle<int>(),
                UnderRepair = multi.ReadSingle<int>(),
                Retired = multi.ReadSingle<int>(),
                SpareAssets = multi.ReadSingle<int>(),
                AssetsByType = multi.Read<AssetTypeCount>().ToList()
            };

            return dto;
        }

        public async Task<AssetFilterValues> GetFilterValuesAsync()
        {
            // --- 1. Build statuses from enum (preferred if you have AssetStatus enum) ---
            Dictionary<int, string> statuses;
            try
            {
                statuses = Enum.GetValues(typeof(AssetManagment.Models.Enums.AssetStatus))
                               .Cast<AssetManagment.Models.Enums.AssetStatus>()
                               .ToDictionary(s => (int)s, s => s.ToString());
            }
            catch
            {
                statuses = new Dictionary<int, string>();
            }

            var assetTypes = new List<string>();
            var conditions = new List<string>();

            // Ensure connection is open. Do NOT dispose _conn here (it's injected).
            if (_conn is System.Data.Common.DbConnection dbConn)
            {
                if (dbConn.State != ConnectionState.Open)
                    await dbConn.OpenAsync();
            }
            else
            {
                if (_conn.State != ConnectionState.Open)
                    _conn.Open();
            }

            // --- 2. Distinct Asset Types ---
            const string typesSql = @"
                        SELECT DISTINCT AssetType 
                        FROM Assets 
                        WHERE AssetType IS NOT NULL AND LTRIM(RTRIM(AssetType)) <> '' 
                        ORDER BY AssetType;
                    ";
            var typeRows = await _conn.QueryAsync<string>(typesSql);
            assetTypes = typeRows
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();

            // --- 3. Distinct Conditions ---
            const string condSql = @"
                        SELECT DISTINCT [Condition] 
                        FROM Assets 
                        WHERE [Condition] IS NOT NULL AND LTRIM(RTRIM([Condition])) <> '' 
                        ORDER BY [Condition];
                    ";
            var condRows = await _conn.QueryAsync<string>(condSql);
            conditions = condRows
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();

            return new AssetFilterValues(assetTypes, statuses, conditions);
        }
    }
}
