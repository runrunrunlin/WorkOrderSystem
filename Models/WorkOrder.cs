namespace WorkOrderSystem.Models;

public class WorkOrder
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Priority { get; set; } = "Medium"; // Low, Medium, High
    public string Status { get; set; } = "Pending"; // Pending, Approved, InProgress, PendingReview, Completed, Rejected, Cancelled

    public int EquipmentId { get; set; }
    public Equipment Equipment { get; set; } = null!;

    public int ReportedById { get; set; }
    public User ReportedBy { get; set; } = null!;

    public int? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public string? CompletionNotes { get; set; }
    public string? RejectionReason { get; set; }
}
