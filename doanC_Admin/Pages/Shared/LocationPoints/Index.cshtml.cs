using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using doanC_Admin.Data;
using doanC_Admin.Models;

namespace doanC_Admin.Pages.LocationPoints
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public List<LocationPoint> LocationPoints { get; set; }

        public async Task OnGetAsync()
        {
            LocationPoints = await _context.LocationPoints.ToListAsync();
        }
    }
}