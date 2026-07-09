using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkOrderSystem.Data;
using WorkOrderSystem.Models;

namespace WorkOrderSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PhotoController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    private static readonly HashSet<string> AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif"];
    private const long MaxBytes = 5 * 1024 * 1024;

    public PhotoController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    private int CurrentUserId => int.Parse(User.FindFirst("id")!.Value);

    [HttpGet("{workOrderId}")]
    public async Task<IActionResult> GetPhotos(int workOrderId)
    {
        var photos = await _db.Photos
            .Where(p => p.WorkOrderId == workOrderId)
            .OrderBy(p => p.UploadedAt)
            .Select(p => new { p.Id, p.WorkOrderId, p.FileName, p.UploadedAt, p.UploadedById })
            .ToListAsync();
        return Ok(photos);
    }

    [HttpPost("{workOrderId}")]
    public async Task<IActionResult> Upload(int workOrderId, [FromForm] IFormFile file)
    {
        if (await _db.WorkOrders.FindAsync(workOrderId) == null)
            return NotFound(new { message = "Work order not found" });

        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No file provided" });

        if (file.Length > MaxBytes)
            return BadRequest(new { message = "File exceeds the 5 MB limit" });

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            return BadRequest(new { message = "Only JPG, PNG, and GIF files are allowed" });

        var folder = Path.Combine(_env.WebRootPath, "uploads", workOrderId.ToString());
        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var filePath = Path.Combine(folder, fileName);

        await using (var stream = System.IO.File.Create(filePath))
            await file.CopyToAsync(stream);

        var photo = new Photo
        {
            WorkOrderId = workOrderId,
            FileName    = fileName,
            UploadedById = CurrentUserId
        };
        _db.Photos.Add(photo);
        await _db.SaveChangesAsync();

        return Ok(new { photo.Id, photo.WorkOrderId, photo.FileName, photo.UploadedAt, photo.UploadedById });
    }

    [HttpDelete("delete/{photoId}")]
    public async Task<IActionResult> Delete(int photoId)
    {
        var photo = await _db.Photos.FindAsync(photoId);
        if (photo == null) return NotFound();

        if (photo.UploadedById != CurrentUserId && !User.IsInRole("Admin"))
            return Forbid();

        var filePath = Path.Combine(_env.WebRootPath, "uploads", photo.WorkOrderId.ToString(), photo.FileName);
        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);

        _db.Photos.Remove(photo);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
