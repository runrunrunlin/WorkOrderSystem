using WorkOrderSystem.Models;

namespace WorkOrderSystem.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Users.Any()) return;

        db.Users.AddRange(
            new User { Username = "admin",  PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), Role = "Admin",      FullName = "System Administrator" },
            new User { Username = "tech1",  PasswordHash = BCrypt.Net.BCrypt.HashPassword("tech123"),  Role = "Technician", FullName = "John Smith" },
            new User { Username = "tech2",  PasswordHash = BCrypt.Net.BCrypt.HashPassword("tech123"),  Role = "Technician", FullName = "Mike Johnson" }
        );

        db.Equipment.AddRange(
            new Equipment { Name = "CNC Lathe A",        Model = "CK6150",      SerialNumber = "SN-001", Location = "Workshop 1", Status = "Normal" },
            new Equipment { Name = "Hydraulic Press",    Model = "HP-200T",     SerialNumber = "SN-002", Location = "Workshop 2", Status = "Normal" },
            new Equipment { Name = "Industrial Robot",   Model = "ABB-IRB1200", SerialNumber = "SN-003", Location = "Workshop 3", Status = "Normal" },
            new Equipment { Name = "Laser Cutter",       Model = "LC-3015",     SerialNumber = "SN-004", Location = "Workshop 1", Status = "UnderRepair" }
        );

        db.SaveChanges();
    }
}
