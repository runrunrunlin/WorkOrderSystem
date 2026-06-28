using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkOrderSystem.Data;
using WorkOrderSystem.DTOs;
using WorkOrderSystem.Models;

namespace WorkOrderSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EquipmentController : ControllerBase
{
    private readonly AppDbContext _db;

    public EquipmentController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.Equipment
            .Select(e => new EquipmentDto(e.Id, e.Name, e.Model, e.SerialNumber, e.Location, e.Status))
            .ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var e = await _db.Equipment.FindAsync(id);
        if (e == null) return NotFound();
        return Ok(new EquipmentDto(e.Id, e.Name, e.Model, e.SerialNumber, e.Location, e.Status));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateEquipmentDto dto)
    {
        var e = new Equipment { Name = dto.Name, Model = dto.Model, SerialNumber = dto.SerialNumber, Location = dto.Location };
        _db.Equipment.Add(e);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = e.Id },
            new EquipmentDto(e.Id, e.Name, e.Model, e.SerialNumber, e.Location, e.Status));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEquipmentDto dto)
    {
        var e = await _db.Equipment.FindAsync(id);
        if (e == null) return NotFound();
        if (dto.Name != null) e.Name = dto.Name;
        if (dto.Model != null) e.Model = dto.Model;
        if (dto.SerialNumber != null) e.SerialNumber = dto.SerialNumber;
        if (dto.Location != null) e.Location = dto.Location;
        if (dto.Status != null) e.Status = dto.Status;
        await _db.SaveChangesAsync();
        return Ok(new EquipmentDto(e.Id, e.Name, e.Model, e.SerialNumber, e.Location, e.Status));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await _db.Equipment.FindAsync(id);
        if (e == null) return NotFound();
        if (await _db.WorkOrders.AnyAsync(w => w.EquipmentId == id && w.Status != "Completed" && w.Status != "Cancelled"))
            return BadRequest(new { message = "Cannot delete equipment with open work orders" });
        _db.Equipment.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
