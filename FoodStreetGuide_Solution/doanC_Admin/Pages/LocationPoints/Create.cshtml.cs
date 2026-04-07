using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using doanC_Admin.Models;

namespace doanC_Admin.Pages.LocationPoints
{
    public class CreateModel : PageModel
    {
        private readonly FoodStreetGuideDBContext _context;

        public CreateModel(FoodStreetGuideDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LocationPoint LocationPoint { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            LocationPoint.CreatedAt = DateTime.UtcNow;
            LocationPoint.UpdatedAt = DateTime.UtcNow;

            _context.LocationPoints.Add(LocationPoint);
            await _context.SaveChangesAsync();

            return RedirectToPage("/LocationPoints/Index");
        }
    }
}