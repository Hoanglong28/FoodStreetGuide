using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doanC_Admin.Models;

namespace doanC_Admin.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationApiController : ControllerBase
    {
        private readonly FoodStreetGuideDBContext _context;

        public LocationApiController(FoodStreetGuideDBContext context)
        {
            _context = context;
        }

        // GET: api/LocationApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationPoint>>> GetAllLocations()
        {
            var locations = await _context.LocationPoints
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
            return Ok(locations);
        }

        // GET: api/LocationApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LocationPoint>> GetLocationById(int id)
        {
            var location = await _context.LocationPoints.FindAsync(id);
            if (location == null)
                return NotFound();
            return Ok(location);
        }

        // GET: api/LocationApi/category/{category}
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<LocationPoint>>> GetLocationsByCategory(string category)
        {
            var locations = await _context.LocationPoints
                .Where(l => l.Category != null && l.Category.Contains(category))
                .ToListAsync();
            return Ok(locations);
        }

        // GET: api/LocationApi/search/{keyword}
        [HttpGet("search/{keyword}")]
        public async Task<ActionResult<IEnumerable<LocationPoint>>> SearchLocations(string keyword)
        {
            var locations = await _context.LocationPoints
                .Where(l => l.Name.Contains(keyword) ||
                            (l.Description != null && l.Description.Contains(keyword)) ||
                            (l.Address != null && l.Address.Contains(keyword)))
                .ToListAsync();
            return Ok(locations);
        }

        // ========== PHƯƠNG THỨC THÊM/SỬA/XÓA (CRUD) ==========

        // POST: api/LocationApi (thêm 1 địa điểm)
        [HttpPost]
        public async Task<ActionResult<LocationPoint>> AddLocation([FromBody] LocationPoint location)
        {
            try
            {
                if (location == null)
                    return BadRequest(new { error = "Dữ liệu không hợp lệ" });

                location.CreatedAt = DateTime.Now;
                location.UpdatedAt = DateTime.Now;

                _context.LocationPoints.Add(location);
                await _context.SaveChangesAsync();

                return Ok(location);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: api/LocationApi/batch (thêm nhiều địa điểm cùng lúc)
        [HttpPost("batch")]
        public async Task<IActionResult> BatchInsertLocations([FromBody] List<LocationPoint> locations)
        {
            try
            {
                if (locations == null || locations.Count == 0)
                    return BadRequest(new { error = "Không có dữ liệu để thêm" });

                foreach (var location in locations)
                {
                    location.CreatedAt = DateTime.Now;
                    location.UpdatedAt = DateTime.Now;
                    _context.LocationPoints.Add(location);
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    count = locations.Count,
                    message = $"Đã thêm thành công {locations.Count} địa điểm"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // PUT: api/LocationApi/5 (cập nhật địa điểm)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] LocationPoint location)
        {
            try
            {
                if (id != location.PointId)
                    return BadRequest(new { error = "ID không khớp" });

                var existingLocation = await _context.LocationPoints.FindAsync(id);
                if (existingLocation == null)
                    return NotFound(new { error = "Không tìm thấy địa điểm" });

                // Cập nhật thông tin
                existingLocation.Name = location.Name;
                existingLocation.Description = location.Description;
                existingLocation.Latitude = location.Latitude;
                existingLocation.Longitude = location.Longitude;
                existingLocation.Address = location.Address;
                existingLocation.Category = location.Category;
                existingLocation.Image = location.Image;
                existingLocation.Rating = location.Rating;
                existingLocation.ReviewCount = location.ReviewCount;
                existingLocation.OpeningHours = location.OpeningHours;
                existingLocation.PriceRange = location.PriceRange;
                existingLocation.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return Ok(existingLocation);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DELETE: api/LocationApi/5 (xóa địa điểm)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            try
            {
                var location = await _context.LocationPoints.FindAsync(id);
                if (location == null)
                    return NotFound(new { error = "Không tìm thấy địa điểm" });

                _context.LocationPoints.Remove(location);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đã xóa thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}