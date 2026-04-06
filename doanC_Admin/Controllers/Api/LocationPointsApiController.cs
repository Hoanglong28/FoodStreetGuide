using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doanC_Admin.Data;
using doanC_Admin.Models;

namespace doanC_Admin.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationPointsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LocationPointsApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/LocationPointsApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationPoint>>> GetLocationPoints()
        {
            return await _context.LocationPoints.ToListAsync();
        }

        // GET: api/LocationPointsApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LocationPoint>> GetLocationPoint(int id)
        {
            var locationPoint = await _context.LocationPoints.FindAsync(id);

            if (locationPoint == null)
            {
                return NotFound();
            }

            return locationPoint;
        }

        // PUT: api/LocationPointsApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLocationPoint(int id, LocationPoint locationPoint)
        {
            if (id != locationPoint.Id)
            {
                return BadRequest();
            }

            _context.Entry(locationPoint).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationPointExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/LocationPointsApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LocationPoint>> PostLocationPoint(LocationPoint locationPoint)
        {
            _context.LocationPoints.Add(locationPoint);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLocationPoint", new { id = locationPoint.Id }, locationPoint);
        }

        // DELETE: api/LocationPointsApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocationPoint(int id)
        {
            var locationPoint = await _context.LocationPoints.FindAsync(id);
            if (locationPoint == null)
            {
                return NotFound();
            }

            _context.LocationPoints.Remove(locationPoint);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LocationPointExists(int id)
        {
            return _context.LocationPoints.Any(e => e.Id == id);
        }
    }
}
