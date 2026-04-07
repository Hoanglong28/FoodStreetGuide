using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using doanC_Admin.Models;

namespace doanC_Admin.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly FoodStreetGuideDBContext _context;

        public DashboardModel(FoodStreetGuideDBContext context)
        {
            _context = context;
        }

        public int TotalLocations { get; set; }
        public int TotalScans { get; set; }
        public int TotalAudios { get; set; }
        public int TotalUsers { get; set; }
        public List<LocationPoint> RecentLocations { get; set; }

        public async Task OnGetAsync()
        {
            // Cách 1: Chạy tuần tự (đơn giản, dễ hiểu)
            TotalLocations = await _context.LocationPoints.CountAsync();
            TotalAudios = await _context.LocationPoints.CountAsync(l => !string.IsNullOrEmpty(l.AudioFile));
            RecentLocations = await _context.LocationPoints
                .OrderByDescending(l => l.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Dữ liệu tạm (sau này có thể lấy từ bảng QRScanLogs)
            TotalScans = 1245;
            TotalUsers = 156;
        }
    }
}