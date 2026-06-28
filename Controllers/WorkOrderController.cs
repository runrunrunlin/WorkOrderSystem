using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WorkOrderSystem.Data;
using WorkOrderSystem.DTOs;
using WorkOrderSystem.Models;

namespace WorkOrderSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkOrderController : ControllerBase
{
    private readonly AppDbContext _db;

    public WorkOrderController(AppDbContext db) => _db = db;

    private int CurrentUserId => int.Parse(User.FindFirst("id")!.Value);

    private static WorkOrderDto ToDto(WorkOrder w) => new(
        w.Id, w.Title, w.Description, w.Priority, w.Status,
        w.EquipmentId, w.Equipment.Name,
        w.ReportedById, w.ReportedBy.FullName,
        w.AssignedToId, w.AssignedTo?.FullName,
        w.CreatedAt, w.UpdatedAt, w.CompletedAt, w.CompletionNotes
    );

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status, [FromQuery] int? equipmentId)
    {
        var query = _db.WorkOrders
            .Include(w => w.Equipment)
            .Include(w => w.ReportedBy)
            .Include(w => w.AssignedTo)
            .AsQueryable();

        if (status != null) query = query.Where(w => w.Status == status);
        if (equipmentId != null) query = query.Where(w => w.EquipmentId == equipmentId);

        if (User.IsInRole("Technician"))
        {
            var uid = CurrentUserId;
            query = query.Where(w => w.ReportedById == uid || w.AssignedToId == uid);
        }

        var list = await query.OrderByDescending(w => w.CreatedAt).Select(w => ToDto(w)).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var w = await _db.WorkOrders
            .Include(w => w.Equipment)
            .Include(w => w.ReportedBy)
            .Include(w => w.AssignedTo)
            .FirstOrDefaultAsync(w => w.Id == id);
        if (w == null) return NotFound();
        return Ok(ToDto(w));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkOrderDto dto)
    {
        var equipment = await _db.Equipment.FindAsync(dto.EquipmentId);
        if (equipment == null) return BadRequest(new { message = "Equipment not found" });

        var w = new WorkOrder
        {
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            EquipmentId = dto.EquipmentId,
            ReportedById = CurrentUserId
        };
        _db.WorkOrders.Add(w);
        equipment.Status = "UnderRepair";
        await _db.SaveChangesAsync();

        var created = await _db.WorkOrders
            .Include(x => x.Equipment).Include(x => x.ReportedBy).Include(x => x.AssignedTo)
            .FirstAsync(x => x.Id == w.Id);
        return CreatedAtAction(nameof(Get), new { id = w.Id }, ToDto(created));
    }

    [HttpPut("{id}/assign")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Assign(int id, [FromBody] AssignWorkOrderDto dto)
    {
        var w = await _db.WorkOrders
            .Include(x => x.Equipment).Include(x => x.ReportedBy).Include(x => x.AssignedTo)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (w == null) return NotFound();
        if (w.Status != "Pending") return BadRequest(new { message = "Only pending work orders can be assigned" });

        var tech = await _db.Users.FindAsync(dto.TechnicianId);
        if (tech == null || tech.Role != "Technician") return BadRequest(new { message = "Technician not found" });

        w.AssignedToId = dto.TechnicianId;
        w.Status = "InProgress";
        w.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        await _db.Entry(w).Reference(x => x.AssignedTo).LoadAsync();
        return Ok(ToDto(w));
    }

    [HttpPut("{id}/complete")]
    public async Task<IActionResult> Complete(int id, [FromBody] CompleteWorkOrderDto dto)
    {
        var w = await _db.WorkOrders
            .Include(x => x.Equipment).Include(x => x.ReportedBy).Include(x => x.AssignedTo)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (w == null) return NotFound();
        if (w.Status != "InProgress") return BadRequest(new { message = "Only in-progress work orders can be completed" });
        if (!User.IsInRole("Admin") && w.AssignedToId != CurrentUserId) return Forbid();

        w.Status = "Completed";
        w.CompletionNotes = dto.CompletionNotes;
        w.CompletedAt = DateTime.UtcNow;
        w.UpdatedAt = DateTime.UtcNow;

        var equipment = await _db.Equipment.FindAsync(w.EquipmentId);
        if (equipment != null) equipment.Status = "Normal";

        await _db.SaveChangesAsync();
        return Ok(ToDto(w));
    }

    [HttpPut("{id}/cancel")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Cancel(int id)
    {
        var w = await _db.WorkOrders
            .Include(x => x.Equipment).Include(x => x.ReportedBy).Include(x => x.AssignedTo)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (w == null) return NotFound();
        if (w.Status is "Completed" or "Cancelled")
            return BadRequest(new { message = "Work order is already completed or cancelled" });

        w.Status = "Cancelled";
        w.UpdatedAt = DateTime.UtcNow;

        var equipment = await _db.Equipment.FindAsync(w.EquipmentId);
        if (equipment?.Status == "UnderRepair") equipment.Status = "Normal";

        await _db.SaveChangesAsync();
        return Ok(ToDto(w));
    }
}
