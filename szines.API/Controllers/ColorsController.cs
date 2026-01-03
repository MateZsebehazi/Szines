using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using szines.API.Data;
using szines.API.Models;

namespace szines.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ColorsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var colors = await _context.Colors
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return Ok(colors);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ColorEntity color)
        {
            _context.Colors.Add(color);
            await _context.SaveChangesAsync();
            return Ok(color);
        }
    }
}
