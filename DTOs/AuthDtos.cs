namespace WorkOrderSystem.DTOs;

public record LoginDto(string Username, string Password);

public record LoginResponse(string Token, string Username, string Role, string FullName, int UserId);
