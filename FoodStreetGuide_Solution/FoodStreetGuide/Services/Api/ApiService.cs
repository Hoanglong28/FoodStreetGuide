using System.Text;
using System.Text.Json;
using doanC_.Models;
using doanC_.Config;

namespace doanC_.Services.Api
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private string _baseUrl;

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // ✅ SỬA: Gọi method GetBaseUrl() thay vì property BaseUrl
            _baseUrl = ApiConfig.ApiMode.GetBaseUrl();
        }

        // ========== CÁC PHƯƠNG THỨC CRUD ==========

        /// <summary>
        /// Lấy tất cả địa điểm
        /// </summary>
        public async Task<List<LocationPoint>> GetLocationPointsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<LocationPoint>>(json, _jsonOptions)
                           ?? new List<LocationPoint>();
                }

                return new List<LocationPoint>();
            }
            catch (HttpRequestException ex)
            {
                await ShowError($"Lỗi kết nối: {ex.Message}");
                return new List<LocationPoint>();
            }
            catch (JsonException ex)
            {
                await ShowError($"Lỗi xử lý dữ liệu: {ex.Message}");
                return new List<LocationPoint>();
            }
            catch (Exception ex)
            {
                await ShowError($"Lỗi không xác định: {ex.Message}");
                return new List<LocationPoint>();
            }
        }

        /// <summary>
        /// Lấy địa điểm theo ID
        /// </summary>
        public async Task<LocationPoint> GetLocationPointByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<LocationPoint>(json, _jsonOptions);
                }

                return null;
            }
            catch (Exception ex)
            {
                await ShowError($"Lỗi: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Thêm địa điểm mới
        /// </summary>
        public async Task<bool> AddLocationPointAsync(LocationPoint locationPoint)
        {
            try
            {
                var json = JsonSerializer.Serialize(locationPoint, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_baseUrl, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                await ShowError($"Lỗi thêm: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Cập nhật địa điểm
        /// </summary>
        public async Task<bool> UpdateLocationPointAsync(LocationPoint locationPoint)
        {
            try
            {
                var json = JsonSerializer.Serialize(locationPoint, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_baseUrl}/{locationPoint.PointId}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                await ShowError($"Lỗi cập nhật: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Xóa địa điểm
        /// </summary>
        public async Task<bool> DeleteLocationPointAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                await ShowError($"Lỗi xóa: {ex.Message}");
                return false;
            }
        }

        // ========== CÁC PHƯƠNG THỨC LỌC DỮ LIỆU ==========

        /// <summary>
        /// Lọc theo category
        /// </summary>
        public async Task<List<LocationPoint>> GetLocationPointsByCategoryAsync(string category)
        {
            var allPoints = await GetLocationPointsAsync();
            return allPoints.Where(p => p.Category?.Equals(category, StringComparison.OrdinalIgnoreCase) == true).ToList();
        }

        /// <summary>
        /// Tìm kiếm theo tên hoặc địa chỉ
        /// </summary>
        public async Task<List<LocationPoint>> SearchLocationPointsAsync(string keyword)
        {
            var allPoints = await GetLocationPointsAsync();
            return allPoints.Where(p =>
                p.Name?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true ||
                p.Description?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true ||
                p.Address?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true
            ).ToList();
        }

        /// <summary>
        /// Lấy địa điểm gần nhất (theo vị trí hiện tại)
        /// </summary>
        public async Task<List<LocationPoint>> GetNearbyLocationPointsAsync(double currentLat, double currentLng, double radiusInKm = 5)
        {
            var allPoints = await GetLocationPointsAsync();

            return allPoints.Where(p =>
            {
                var distance = CalculateDistance(currentLat, currentLng, p.Latitude, p.Longitude);
                return distance <= radiusInKm;
            }).OrderBy(p => CalculateDistance(currentLat, currentLng, p.Latitude, p.Longitude))
            .ToList();
        }

        // ========== HÀM TIỆN ÍCH ==========

        /// <summary>
        /// Tính khoảng cách giữa 2 điểm (Haversine formula)
        /// </summary>
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double degrees) => degrees * Math.PI / 180;

        /// <summary>
        /// Hiển thị lỗi (chạy trên UI thread)
        /// </summary>
        private async Task ShowError(string message)
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", message, "OK");
            }
        }

        /// <summary>
        /// Kiểm tra kết nối API
        /// </summary>
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}