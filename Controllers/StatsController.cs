using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkOrderSystem.Data;

namespace WorkOrderSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatsController : ControllerBase
{
    private readonly AppDbContext _db;
    public StatsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
        var orders = await _db.WorkOrders.ToListAsync();

        var byStatus = orders
            .GroupBy(w => w.Status)
            .Select(g => new { status = g.Key, count = g.Count() });

        var byPriority = orders
            .GroupBy(w => w.Priority)
            .Select(g => new { priority = g.Key, count = g.Count() });

        var recent = orders
            .Where(w => w.CreatedAt >= DateTime.UtcNow.AddDays(-7))
            .GroupBy(w => w.CreatedAt.Date)
            .Select(g => new { date = g.Key.ToString("MM/dd"), count = g.Count() })
            .OrderBy(x => x.date);

        return Ok(new { byStatus, byPriority, recent });
    }
}