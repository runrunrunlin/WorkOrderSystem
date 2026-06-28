using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkOrderSystem.Data;

namespace WorkOrderSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    public UsersController(AppDbContext db) => _db = db;

    [HttpGet("technicians")]
    public async Task<IActionResult> GetTechnicians()
    {
        var techs = await _db.Users
            .Where(u => u.Role == "Technician")
            .Select(u => new { u.Id, u.Username, u.FullName })
            .ToListAsync();
        return Ok(techs);
    }
}
