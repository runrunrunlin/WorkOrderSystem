namespace WorkOrderSystem.DTOs;

public record WorkOrderDto(
    int Id, string Title, string Description, string Priority, string Status,
    int EquipmentId, string EquipmentName,
    int ReportedById, string ReportedByName,
    int? AssignedToId, string? AssignedToName,
    DateTime CreatedAt, DateTime? UpdatedAt, DateTime? CompletedAt,
    string? CompletionNotes
);

public record CreateWorkOrderDto(string Title, string Description, string Priority, int EquipmentId);

public record AssignWorkOrderDto(int TechnicianId);

public record CompleteWorkOrderDto(string CompletionNotes);
