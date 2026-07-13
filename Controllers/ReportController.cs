using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WorkOrderSystem.Data;

namespace WorkOrderSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly AppDbContext _db;

    static ReportController()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public ReportController(AppDbContext db) => _db = db;

    [HttpGet("{workOrderId}")]
    public async Task<IActionResult> GetReport(int workOrderId)
    {
        var w = await _db.WorkOrders
            .Include(x => x.Equipment)
            .Include(x => x.ReportedBy)
            .Include(x => x.AssignedTo)
            .FirstOrDefaultAsync(x => x.Id == workOrderId);

        if (w == null) return NotFound();

        var checklist = await _db.ChecklistItems
            .Where(c => c.WorkOrderId == workOrderId)
            .OrderBy(c => c.Id)
            .ToListAsync();

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(inner =>
                        {
                            inner.Item().Text("Equipment Maintenance Work Order System")
                                .FontSize(16).Bold().FontColor("#1e3c72");
                            inner.Item().Text("Maintenance Report")
                                .FontSize(12).FontColor("#555555");
                        });
                        row.ConstantItem(120).AlignRight().Column(inner =>
                        {
                            inner.Item().Text($"Report Date:").FontSize(9).FontColor("#888888");
                            inner.Item().Text(DateTime.Now.ToString("yyyy-MM-dd HH:mm"))
                                .FontSize(9).Bold();
                        });
                    });
                    col.Item().PaddingTop(6).LineHorizontal(2).LineColor("#1e3c72");
                });

                page.Content().PaddingTop(16).Column(col =>
                {
                    // Work Order Info
                    col.Item().Background("#f0f4ff").Padding(10).Column(inner =>
                    {
                        inner.Item().Text("Work Order Information").Bold().FontSize(11).FontColor("#1e3c72");
                        inner.Item().PaddingTop(6).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(120);
                                c.RelativeColumn();
                                c.ConstantColumn(120);
                                c.RelativeColumn();
                            });

                            void AddRow(string label1, string value1, string label2, string value2)
                            {
                                table.Cell().Padding(3).Text(label1).Bold().FontColor("#444444");
                                table.Cell().Padding(3).Text(value1);
                                table.Cell().Padding(3).Text(label2).Bold().FontColor("#444444");
                                table.Cell().Padding(3).Text(value2);
                            }

                            AddRow("Work Order #:", $"WO-{w.Id:D4}", "Status:", w.Status);
                            AddRow("Title:", w.Title, "Priority:", w.Priority);
                            AddRow("Equipment:", w.Equipment.Name, "Equipment ID:", $"EQ-{w.EquipmentId:D3}");
                            AddRow("Created:", w.CreatedAt.ToString("yyyy-MM-dd HH:mm"), "Updated:", w.UpdatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "—");
                            AddRow("Completed:", w.CompletedAt?.ToString("yyyy-MM-dd HH:mm") ?? "—", "", "");
                        });
                    });

                    col.Item().PaddingTop(12).Column(inner =>
                    {
                        inner.Item().Text("Personnel").Bold().FontSize(11).FontColor("#1e3c72");
                        inner.Item().LineHorizontal(1).LineColor("#cccccc");
                        inner.Item().PaddingTop(6).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(120);
                                c.RelativeColumn();
                                c.ConstantColumn(120);
                                c.RelativeColumn();
                            });
                            table.Cell().Padding(3).Text("Reported By:").Bold().FontColor("#444444");
                            table.Cell().Padding(3).Text(w.ReportedBy.FullName);
                            table.Cell().Padding(3).Text("Assigned To:").Bold().FontColor("#444444");
                            table.Cell().Padding(3).Text(w.AssignedTo?.FullName ?? "Unassigned");
                        });
                    });

                    col.Item().PaddingTop(12).Column(inner =>
                    {
                        inner.Item().Text("Fault Description").Bold().FontSize(11).FontColor("#1e3c72");
                        inner.Item().LineHorizontal(1).LineColor("#cccccc");
                        inner.Item().PaddingTop(6).Background("#fafafa").Padding(8)
                            .Text(w.Description).FontColor("#333333");
                    });

                    col.Item().PaddingTop(12).Column(inner =>
                    {
                        inner.Item().Text("Completion Notes").Bold().FontSize(11).FontColor("#1e3c72");
                        inner.Item().LineHorizontal(1).LineColor("#cccccc");
                        inner.Item().PaddingTop(6).Background("#fafafa").Padding(8)
                            .Text(string.IsNullOrWhiteSpace(w.CompletionNotes) ? "N/A" : w.CompletionNotes)
                            .FontColor("#333333");
                    });

                    col.Item().PaddingTop(12).Column(inner =>
                    {
                        inner.Item().Text("Safety Inspection Checklist").Bold().FontSize(11).FontColor("#1e3c72");
                        inner.Item().LineHorizontal(1).LineColor("#cccccc");
                        inner.Item().PaddingTop(6).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(24);
                                c.RelativeColumn();
                                c.ConstantColumn(120);
                            });

                            table.Header(h =>
                            {
                                h.Cell().Padding(4).Text("").Bold();
                                h.Cell().Padding(4).Text("Item").Bold().FontColor("#444444");
                                h.Cell().Padding(4).Text("Verified At").Bold().FontColor("#444444");
                            });

                            if (checklist.Count == 0)
                            {
                                table.Cell().ColumnSpan(3).Padding(4)
                                    .Text("No checklist items found.").Italic().FontColor("#888888");
                            }
                            else
                            {
                                foreach (var item in checklist)
                                {
                                    var checkMark = item.IsChecked ? "✓" : "☐";
                                    var checkColor = item.IsChecked ? "#2e7d32" : "#cc0000";
                                    table.Cell().Padding(4).Text(checkMark).Bold().FontColor(checkColor);
                                    table.Cell().Padding(4).Text(item.ItemText);
                                    table.Cell().Padding(4).Text(item.CheckedAt?.ToString("yyyy-MM-dd HH:mm") ?? "—")
                                        .FontColor("#777777");
                                }
                            }
                        });
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Equipment Maintenance Work Order System  |  ").FontColor("#aaaaaa").FontSize(8);
                    text.Span($"WO-{w.Id:D4}  |  ").FontColor("#aaaaaa").FontSize(8);
                    text.Span("Page ").FontColor("#aaaaaa").FontSize(8);
                    text.CurrentPageNumber().FontColor("#aaaaaa").FontSize(8);
                    text.Span(" of ").FontColor("#aaaaaa").FontSize(8);
                    text.TotalPages().FontColor("#aaaaaa").FontSize(8);
                });
            });
        }).GeneratePdf();

        return File(pdf, "application/pdf", $"WorkOrder-{w.Id:D4}.pdf");
    }
}
