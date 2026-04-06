using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using doanC_Admin.Data;
using doanC_Admin.Models;

namespace doanC_Admin.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly AppDbContext _context;

        public DashboardModel(AppDbContext context)
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
            TotalLocations = await _context.LocationPoints.CountAsync();
            TotalScans = 1245; // Tạm thời, sau này lấy từ bảng ScanLog
            TotalAudios = await _context.LocationPoints.CountAsync(l => !string.IsNullOrEmpty(l.AudioFile));
            TotalUsers = 156; // Tạm thời

            RecentLocations = await _context.LocationPoints
                .OrderByDescending(l => l.CreatedAt)
                .Take(5)
                .ToListAsync();
        }
    }
}