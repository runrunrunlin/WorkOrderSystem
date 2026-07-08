namespace WorkOrderSystem.Models;

public class Photo
{
    public int Id { get; set; }
    public int WorkOrderId { get; set; }
    public WorkOrder WorkOrder { get; set; } = null!;
    public string FileName { get; set; } = "";
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public int UploadedById { get; set; }
    public User UploadedBy { get; set; } = null!;
}
