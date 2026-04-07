using SQLite;
using doanC_.Models;
using doanC_.Services.Api;
using System.Diagnostics;

namespace doanC_.Services.Offline
{
    public class OfflinePoiService
    {
        private SQLiteAsyncConnection _database;
        private readonly ApiService _apiService;

        public OfflinePoiService(ApiService apiService)
        {
            _apiService = apiService;
            InitDatabase();
        }

        private async void InitDatabase()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "offline_data.db");
            _database = new SQLiteAsyncConnection(dbPath);
            await _database.CreateTableAsync<LocationPoint>();
            await _database.CreateTableAsync<ScanHistory>();
            await _database.CreateTableAsync<TranslatedText>();
        }

        // Lấy POI (offline-first)
        public async Task<List<LocationPoint>> GetLocationPointsAsync()
        {
            try
            {
                // Thử lấy từ API (online)
                var onlineData = await _apiService.GetLocationPointsAsync();
                if (onlineData != null && onlineData.Any())
                {
                    // Lưu vào SQLite để dùng offline
                    await SaveLocationPointsToLocal(onlineData);
                    return onlineData;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Offline] Cannot connect to API: {ex.Message}");
            }

            // Fallback: lấy từ SQLite (offline)
            Debug.WriteLine("[Offline] Loading data from local SQLite...");
            var localData = await _database.Table<LocationPoint>().ToListAsync();
            return localData;
        }

        private async Task SaveLocationPointsToLocal(List<LocationPoint> points)
        {
            await _database.DeleteAllAsync<LocationPoint>();
            await _database.InsertAllAsync(points);
            Debug.WriteLine($"[Offline] Saved {points.Count} POIs to local cache");
        }

        // Thêm scan history (offline)
        public async Task AddScanHistory(int poiId, string deviceId)
        {
            var history = new ScanHistory
            {
                PointId = poiId,
                DeviceId = deviceId,
                ScanTime = DateTime.Now
            };
            await _database.InsertAsync(history);
        }

        // Lấy scan history
        public async Task<List<ScanHistory>> GetScanHistoryAsync()
        {
            return await _database.Table<ScanHistory>()
                .OrderByDescending(h => h.ScanTime)
                .Take(20)
                .ToListAsync();
        }
    }

    [Table("ScanHistory")]
    public class ScanHistory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int PointId { get; set; }
        public string DeviceId { get; set; }
        public DateTime ScanTime { get; set; }
    }

    [Table("TranslatedText")]
    public class TranslatedText
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int PointId { get; set; }
        public string Language { get; set; }
        public string Text { get; set; }
        public DateTime CachedAt { get; set; }
    }
}