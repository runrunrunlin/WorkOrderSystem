namespace WorkOrderSystem.Models;

public class Equipment
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Model { get; set; } = "";
    public string SerialNumber { get; set; } = "";
    public string Location { get; set; } = "";
    public string Status { get; set; } = "Normal"; // Normal, UnderRepair, Scrapped

    public ICollection<WorkOrder> WorkOrders { get; set; } = [];
}
