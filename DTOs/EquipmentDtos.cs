namespace WorkOrderSystem.DTOs;

public record EquipmentDto(int Id, string Name, string Model, string SerialNumber, string Location, string Status);

public record CreateEquipmentDto(string Name, string Model, string SerialNumber, string Location);

public record UpdateEquipmentDto(string? Name, string? Model, string? SerialNumber, string? Location, string? Status);
