using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WorkOrderSystem.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    DbSeeder.Seed(db);

    // EnsureCreated never alters an existing database, so new tables must be
    // created explicitly. This statement is a no-op if the table already exists.
    db.Database.ExecuteSqlRaw("""
        CREATE TABLE IF NOT EXISTS "Photos" (
            "Id"            INTEGER NOT NULL CONSTRAINT "PK_Photos" PRIMARY KEY AUTOINCREMENT,
            "WorkOrderId"   INTEGER NOT NULL,
            "FileName"      TEXT    NOT NULL,
            "UploadedAt"    TEXT    NOT NULL,
            "UploadedById"  INTEGER NOT NULL,
            CONSTRAINT "FK_Photos_WorkOrders_WorkOrderId"
                FOREIGN KEY ("WorkOrderId") REFERENCES "WorkOrders" ("Id") ON DELETE CASCADE,
            CONSTRAINT "FK_Photos_Users_UploadedById"
                FOREIGN KEY ("UploadedById") REFERENCES "Users" ("Id") ON DELETE RESTRICT
        )
        """);

    db.Database.ExecuteSqlRaw("""
        CREATE TABLE IF NOT EXISTS "ChecklistItems" (
            "Id"            INTEGER NOT NULL CONSTRAINT "PK_ChecklistItems" PRIMARY KEY AUTOINCREMENT,
            "WorkOrderId"   INTEGER NOT NULL,
            "ItemText"      TEXT    NOT NULL,
            "IsChecked"     INTEGER NOT NULL DEFAULT 0,
            "CheckedAt"     TEXT,
            "CheckedById"   INTEGER,
            CONSTRAINT "FK_ChecklistItems_WorkOrders_WorkOrderId"
                FOREIGN KEY ("WorkOrderId") REFERENCES "WorkOrders" ("Id") ON DELETE CASCADE,
            CONSTRAINT "FK_ChecklistItems_Users_CheckedById"
                FOREIGN KEY ("CheckedById") REFERENCES "Users" ("Id") ON DELETE RESTRICT
        )
        """);
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
