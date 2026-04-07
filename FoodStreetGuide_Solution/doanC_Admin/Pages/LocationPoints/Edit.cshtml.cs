using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using doanC_Admin.Models;

namespace doanC_Admin.Pages.LocationPoints
{
    public class EditModel : PageModel
    {
        private readonly FoodStreetGuideDBContext _context;

        public EditModel(FoodStreetGuideDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LocationPoint LocationPoint { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            LocationPoint = await _context.LocationPoints.FindAsync(id);  // FindAsync dùng khóa chính
            if (LocationPoint == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            LocationPoint.UpdatedAt = DateTime.UtcNow;
            _context.Update(LocationPoint);
            await _context.SaveChangesAsync();
            return RedirectToPage("/LocationPoints/Index");
        }
    }
}