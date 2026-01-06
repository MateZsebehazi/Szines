using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using szines.API.Data;
using szines.API.Models;
using szines.API.Services;

namespace szines.API.Controllers
{
    [ApiController]
    [Route("api/colors")]
    public class ColorsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ColorEventService _eventService;

        public ColorsController(AppDbContext context, ColorEventService eventService)
        {
            _context = context;
            _eventService = eventService;
        }

        [HttpGet("stream")]
        public async Task Stream(CancellationToken cancellationToken)
        {
            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Connection", "keep-alive");

            var writer = new StreamWriter(Response.Body);
            var clientId = _eventService.AddClient(writer);

            try
            {
                // Send initial connection confirmation
                await writer.WriteAsync($"data: connected\n\n");
                await writer.FlushAsync();

                // Keep connection alive until cancelled
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(30000, cancellationToken);
                    await writer.WriteAsync($": keepalive\n\n");
                    await writer.FlushAsync();
                }
            }
            catch (OperationCanceledException)
            {
                // Client disconnected
            }
            finally
            {
                _eventService.RemoveClient(clientId);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var colors = await _context.Colors
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            return Ok(colors);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ColorEntity color)
        {
            _context.Colors.Add(color);
            await _context.SaveChangesAsync();
            await _eventService.NotifyClientsAsync();
            return Ok(color);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var color = await _context.Colors.FindAsync(id);

            if (color == null)
                return NotFound();

            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();
            await _eventService.NotifyClientsAsync();

            return NoContent();
        }
    }
}
