namespace WorkOrderSystem.Models;

public class ChecklistItem
{
    public int Id { get; set; }
    public int WorkOrderId { get; set; }
    public WorkOrder WorkOrder { get; set; } = null!;
    public string ItemText { get; set; } = "";
    public bool IsChecked { get; set; }
    public DateTime? CheckedAt { get; set; }
    public int? CheckedById { get; set; }
    public User? CheckedBy { get; set; }
}
