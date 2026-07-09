using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkOrderSystem.Data;
using WorkOrderSystem.Models;

namespace WorkOrderSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChecklistController : ControllerBase
{
    private readonly AppDbContext _db;

    private static readonly string[] DefaultItems =
    [
        "Power isolation confirmed",
        "Lockout/Tagout applied",
        "Safety equipment worn (gloves, goggles)",
        "Work area secured and marked",
        "Required tools and parts ready",
        "Maintenance manual reviewed"
    ];

    public ChecklistController(AppDbContext db) => _db = db;

    private int CurrentUserId => int.Parse(User.FindFirst("id")!.Value);

    [HttpGet("{workOrderId}")]
    public async Task<IActionResult> GetItems(int workOrderId)
    {
        var items = await _db.ChecklistItems
            .Where(c => c.WorkOrderId == workOrderId)
            .OrderBy(c => c.Id)
            .Select(c => new
            {
                c.Id, c.WorkOrderId, c.ItemText,
                c.IsChecked, c.CheckedAt, c.CheckedById
            })
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost("{workOrderId}/initialize")]
    public async Task<IActionResult> Initialize(int workOrderId)
    {
        if (await _db.WorkOrders.FindAsync(workOrderId) == null)
            return NotFound(new { message = "Work order not found" });

        if (await _db.ChecklistItems.AnyAsync(c => c.WorkOrderId == workOrderId))
            return Ok(new { message = "Checklist already initialized" });

        _db.ChecklistItems.AddRange(DefaultItems.Select(text => new ChecklistItem
        {
            WorkOrderId = workOrderId,
            ItemText    = text
        }));
        await _db.SaveChangesAsync();
        return Ok(new { message = "Checklist initialized" });
    }

    [HttpPut("{itemId}/check")]
    public async Task<IActionResult> Check(int itemId)
    {
        var item = await _db.ChecklistItems.FindAsync(itemId);
        if (item == null) return NotFound();

        item.IsChecked  = true;
        item.CheckedAt  = DateTime.UtcNow;
        item.CheckedById = CurrentUserId;
        await _db.SaveChangesAsync();
        return Ok(new { item.Id, item.IsChecked, item.CheckedAt, item.CheckedById });
    }

    [HttpPut("{itemId}/uncheck")]
    public async Task<IActionResult> Uncheck(int itemId)
    {
        var item = await _db.ChecklistItems.FindAsync(itemId);
        if (item == null) return NotFound();

        item.IsChecked   = false;
        item.CheckedAt   = null;
        item.CheckedById = null;
        await _db.SaveChangesAsync();
        return Ok(new { item.Id, item.IsChecked, item.CheckedAt, item.CheckedById });
    }
}
