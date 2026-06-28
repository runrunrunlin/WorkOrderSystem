namespace WorkOrderSystem.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "Technician"; // Admin, Technician
    public string FullName { get; set; } = "";

    public ICollection<WorkOrder> ReportedOrders { get; set; } = [];
    public ICollection<WorkOrder> AssignedOrders { get; set; } = [];
}
