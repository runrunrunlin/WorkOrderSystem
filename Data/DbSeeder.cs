using WorkOrderSystem.Models;

namespace WorkOrderSystem.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Users.Any()) return;

        var admin = new User { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), Role = "Admin",      FullName = "System Administrator" };
        var tech1 = new User { Username = "tech1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("tech123"),  Role = "Technician", FullName = "John Smith" };
        var tech2 = new User { Username = "tech2", PasswordHash = BCrypt.Net.BCrypt.HashPassword("tech123"),  Role = "Technician", FullName = "Mike Johnson" };
        db.Users.AddRange(admin, tech1, tech2);

        var lathe  = new Equipment { Name = "CNC Lathe A",      Model = "CK6150",      SerialNumber = "SN-001", Location = "Workshop 1", Status = "Normal" };
        var press  = new Equipment { Name = "Hydraulic Press",  Model = "HP-200T",     SerialNumber = "SN-002", Location = "Workshop 2", Status = "Normal" };
        var robot  = new Equipment { Name = "Industrial Robot", Model = "ABB-IRB1200", SerialNumber = "SN-003", Location = "Workshop 3", Status = "Normal" };
        var cutter = new Equipment { Name = "Laser Cutter",     Model = "LC-3015",     SerialNumber = "SN-004", Location = "Workshop 1", Status = "UnderRepair" };
        db.Equipment.AddRange(lathe, press, robot, cutter);

        db.SaveChanges();

        var now = DateTime.UtcNow;

        db.WorkOrders.AddRange(

            // ── Completed (15) ──────────────────────────────────────────────────

            new WorkOrder {
                Title = "Scheduled Lubrication Service",
                Description = "Perform monthly lubrication on spindle, tailstock, and carriage ways per maintenance schedule.",
                Priority = "Low", Status = "Completed",
                EquipmentId = lathe.Id, ReportedById = admin.Id, AssignedToId = tech1.Id,
                CreatedAt = now.AddDays(-29), UpdatedAt = now.AddDays(-27), CompletedAt = now.AddDays(-27),
                CompletionNotes = "All lubrication points serviced. Replaced way oil reservoir filter. No issues found."
            },
            new WorkOrder {
                Title = "Hydraulic Fluid Leak",
                Description = "Operator reported visible oil pooling beneath the press frame. Production halted pending inspection.",
                Priority = "High", Status = "Completed",
                EquipmentId = press.Id, ReportedById = tech2.Id, AssignedToId = tech2.Id,
                CreatedAt = now.AddDays(-28), UpdatedAt = now.AddDays(-27), CompletedAt = now.AddDays(-27),
                CompletionNotes = "Located cracked O-ring on return line fitting. Replaced O-ring and hose clamp. Pressure tested at 200T — no further leaks."
            },
            new WorkOrder {
                Title = "End-Effector Calibration Drift",
                Description = "Pick-and-place accuracy degraded over the past week. Parts placement off by approximately 2 mm.",
                Priority = "Medium", Status = "Completed",
                EquipmentId = robot.Id, ReportedById = admin.Id, AssignedToId = tech1.Id,
                CreatedAt = now.AddDays(-27), UpdatedAt = now.AddDays(-25), CompletedAt = now.AddDays(-25),
                CompletionNotes = "Recalibrated TCP and tool frame. Root cause: gripper mounting bolts had loosened. Re-torqued and re-ran acceptance cycle — accuracy within 0.3 mm."
            },
            new WorkOrder {
                Title = "Cutting Lens Contamination",
                Description = "Cut quality deteriorating — rough edges and incomplete cuts on 3 mm mild steel. Suspected dirty optics.",
                Priority = "Medium", Status = "Completed",
                EquipmentId = cutter.Id, ReportedById = tech1.Id, AssignedToId = tech2.Id,
                CreatedAt = now.AddDays(-26), UpdatedAt = now.AddDays(-25), CompletedAt = now.AddDays(-25),
                CompletionNotes = "Lens had heavy spatter deposit. Cleaned focus lens and protective window with IPA. Realigned beam path. Cut quality confirmed good on test pieces."
            },
            new WorkOrder {
                Title = "Spindle Bearing Noise",
                Description = "Intermittent grinding noise at spindle speeds above 1500 RPM. Vibration increasing over the last two shifts.",
                Priority = "High", Status = "Completed",
                EquipmentId = lathe.Id, ReportedById = tech1.Id, AssignedToId = tech1.Id,
                CreatedAt = now.AddDays(-25), UpdatedAt = now.AddDays(-23), CompletedAt = now.AddDays(-23),
                CompletionNotes = "Replaced front and rear spindle bearings (FAG 7210-B-TVP). Tested at full RPM range — noise eliminated. Spindle runout confirmed at 0.004 mm."
            },
            new WorkOrder {
                Title = "Pressure Relief Valve Replacement",
                Description = "Relief valve venting prematurely at 160 bar instead of rated 200 bar. Press cannot reach full tonnage.",
                Priority = "High", Status = "Completed",
                EquipmentId = press.Id, ReportedById = admin.Id, AssignedToId = tech2.Id,
                CreatedAt = now.AddDays(-24), UpdatedAt = now.AddDays(-23), CompletedAt = now.AddDays(-23),
                CompletionNotes = "Replaced Rexroth DBDS-10 relief valve with OEM part. Set and verified relief pressure at 200 bar on calibrated gauge. Full tonnage restored."
            },
            new WorkOrder {
                Title = "Routine Joint Lubrication",
                Description = "500-hour scheduled maintenance: grease all six axis joints and wrist assembly per ABB maintenance manual.",
                Priority = "Low", Status = "Completed",
                EquipmentId = robot.Id, ReportedById = admin.Id, AssignedToId = tech1.Id,
                CreatedAt = now.AddDays(-23), UpdatedAt = now.AddDays(-21), CompletedAt = now.AddDays(-21),
                CompletionNotes = "All joints greased with ABB-specified LGEP 2 grease. Wrist assembly inspected — no wear. Updated maintenance log with service hour count."
            },
            new WorkOrder {
                Title = "Cooling Water Pump Failure",
                Description = "Laser cooling alarm triggered. Chiller pump not circulating — machine locked out by thermal interlock.",
                Priority = "High", Status = "Completed",
                EquipmentId = cutter.Id, ReportedById = tech2.Id, AssignedToId = tech2.Id,
                CreatedAt = now.AddDays(-22), UpdatedAt = now.AddDays(-21), CompletedAt = now.AddDays(-21),
                CompletionNotes = "Pump impeller seized due to calcium buildup. Replaced pump unit and flushed cooling circuit with descaler. Coolant flow rate confirmed at 12 L/min."
            },
            new WorkOrder {
                Title = "Tool Holder Wear",
                Description = "Dimensional tolerance drifting on turned parts. Tool holder taper suspected to be worn, causing runout.",
                Priority = "Medium", Status = "Completed",
                EquipmentId = lathe.Id, ReportedById = tech1.Id, AssignedToId = tech2.Id,
                CreatedAt = now.AddDays(-21), UpdatedAt = now.AddDays(-19), CompletedAt = now.AddDays(-19),
                CompletionNotes = "Turret holder #3 had 0.02 mm taper wear. Replaced holder. Verified runout at 0.005 mm. Parts back in tolerance on next batch."
            },
            new WorkOrder {
                Title = "Main Cylinder Seal Replacement",
                Description = "Visible seepage around main cylinder rod. Hydraulic fluid consumption has doubled over the past week.",
                Priority = "Medium", Status = "Completed",
                EquipmentId = press.Id, ReportedById = admin.Id, AssignedToId = tech2.Id,
                CreatedAt = now.AddDays(-20), UpdatedAt = now.AddDays(-18), CompletedAt = now.AddDays(-18),
                CompletionNotes = "Replaced rod wiper and U-cup seals on main cylinder. Inspected rod surface — no scoring. Zero seepage after 4-hour pressure soak test."
            },
            new WorkOrder {
                Title = "Emergency Stop Circuit Fault",
                Description = "Robot e-stop triggers randomly without operator input. Entire workcell affected — production stopped.",
                Priority = "High", Status = "Completed",
                EquipmentId = robot.Id, ReportedById = tech2.Id, AssignedToId = tech1.Id,
                CreatedAt = now.AddDays(-18), UpdatedAt = now.AddDays(-17), CompletedAt = now.AddDays(-17),
                CompletionNotes = "Traced fault to loose terminal on safety relay K3. Re-torqued all terminals in the safety circuit cabinet. Ran 200-cycle test with zero nuisance trips."
            },
            new WorkOrder {
                Title = "Coolant System Flush",
                Description = "Coolant pH below specification (5.8, target 8.5–9.5) and strong odour. Bacterial contamination suspected.",
                Priority = "Low", Status = "Completed",
                EquipmentId = lathe.Id, ReportedById = admin.Id, AssignedToId = tech2.Id,
                CreatedAt = now.AddDays(-17), UpdatedAt = now.AddDays(-15), CompletedAt = now.AddDays(-15),
                CompletionNotes = "Drained and cleaned sump. Flushed with system cleaner. Refilled with fresh Blasocut 4000 at 8% concentration. pH confirmed at 9.0."
            },
            new WorkOrder {
                Title = "Assist Gas Regulator Malfunction",
                Description = "Nitrogen assist gas pressure fluctuating during cutting cycles. Causing inconsistent cut quality on stainless steel.",
                Priority = "Medium", Status = "Completed",
                EquipmentId = cutter.Id, ReportedById = tech1.Id, AssignedToId = tech1.Id,
                CreatedAt = now.AddDays(-15), UpdatedAt = now.AddDays(-13), CompletedAt = now.AddDays(-13),
                CompletionNotes = "Diaphragm in the assist gas regulator had micro-tears. Replaced regulator unit. Pressure verified stable at 14 bar across full cutting cycle."
            },
            new WorkOrder {
                Title = "Hydraulic Pump Whine",
                Description = "Loud whining noise from hydraulic power unit during startup. Noise subsides after 5 minutes but returns under load.",
                Priority = "Medium", Status = "Completed",
                EquipmentId = press.Id, ReportedById = tech1.Id, AssignedToId = tech2.Id,
                CreatedAt = now.AddDays(-13), UpdatedAt = now.AddDays(-11), CompletedAt = now.AddDays(-11),
                CompletionNotes = "Air ingestion through a loose pump inlet fitting. Re-sealed fitting with Loctite 545. Bled hydraulic circuit. Pump running quietly under full load."
            },
            new WorkOrder {
                Title = "Wrist Axis Encoder Failure",
                Description = "Axis 6 position error alarm on startup. Robot cannot home. Encoder fault code F3-0062 logged in controller.",
                Priority = "High", Status = "Completed",
                EquipmentId = robot.Id, ReportedById = admin.Id, AssignedToId = tech1.Id,
                CreatedAt = now.AddDays(-11), UpdatedAt = now.AddDays(-9), CompletedAt = now.AddDays(-9),
                CompletionNotes = "Replaced Axis 6 resolver encoder. Updated mastering data using dial gauge and reference fixture. Homing and full-range motion confirmed without errors."
            },

            // ── InProgress (8) ──────────────────────────────────────────────────

            new WorkOrder {
                Title = "Cutting Head Crash Damage",
                Description = "Cutting head collided with fixture clamp during automated cycle. Nozzle body cracked — machine taken offline.",
                Priority = "High", Status = "InProgress",
                EquipmentId = cutter.Id, ReportedById = admin.Id, AssignedToId = tech2.Id,
                CreatedAt = now.AddDays(-8), UpdatedAt = now.AddDays(-7)
            },
            new WorkOrder {
                Title = "X-Axis Servo Motor Fault",
                Description = "X-axis following error alarm during rapid traverse. Drive shows overcurrent fault. Motor suspected failing.",
                Priority = "High", Status = "InProgress",
                EquipmentId = lathe.Id, ReportedById = tech2.Id, AssignedToId = tech1.Id,
                CreatedAt = now.AddDays(-7), UpdatedAt = now.AddDays(-6)
            },
            new WorkOrder {
                Title = "HMI Touchscreen Partial Failure",
                Description = "Control panel touchscreen unresponsive in lower-left quadrant. Operator cannot input stroke values reliably.",
                Priority = "Medium", Status = "InProgress",
                EquipmentId = press.Id, ReportedById = tech1.Id, AssignedToId = tech2.Id,
                CreatedAt = now.AddDays(-6), UpdatedAt = now.AddDays(-5)
            },
            new WorkOrder {
                Title = "Teach Pendant Cable Damage",
                Description = "Teach pendant cable has visible sheath damage near the controller connector. Cable intermittently loses signal.",
                Priority = "Medium", Status = "InProgress",
                EquipmentId = robot.Id, ReportedById = admin.Id, AssignedToId = tech1.Id,
                CreatedAt = now.AddDays(-5), UpdatedAt = now.AddDays(-4)
            },
            new WorkOrder {
                Title = "Chip Conveyor Belt Broken",
                Description = "Chip conveyor belt snapped mid-shift. Chips accumulating in the machine — operator clearing manually until repaired.",
                Priority = "Low", Status = "InProgress",
                EquipmentId = lathe.Id, ReportedById = tech1.Id, AssignedToId = tech2.Id,
                CreatedAt = now.AddDays(-5), UpdatedAt = now.AddDays(-4)
            },
            new WorkOrder {
                Title = "Safety Door Interlock Fault",
                Description = "Bed safety door interlock giving intermittent faults. Machine requires multiple door cycles to reset. Safety risk.",
                Priority = "High", Status = "InProgress",
                EquipmentId = press.Id, ReportedById = admin.Id, AssignedToId = tech1.Id,
                CreatedAt = now.AddDays(-4), UpdatedAt = now.AddDays(-3)
            },
            new WorkOrder {
                Title = "Floor Anchor Bolt Torque Check",
                Description = "Annual torque verification required on all floor anchor bolts. Robot must be taken offline for access.",
                Priority = "Low", Status = "InProgress",
                EquipmentId = robot.Id, ReportedById = admin.Id, AssignedToId = tech2.Id,
                CreatedAt = now.AddDays(-3), UpdatedAt = now.AddDays(-2)
            },
            new WorkOrder {
                Title = "Focus Lens Cracked",
                Description = "Operator noticed beam scatter and reduced power output. Inspection through viewport confirms cracked focus lens.",
                Priority = "High", Status = "InProgress",
                EquipmentId = cutter.Id, ReportedById = tech2.Id, AssignedToId = tech1.Id,
                CreatedAt = now.AddDays(-3), UpdatedAt = now.AddDays(-2)
            },

            // ── Cancelled (4) ───────────────────────────────────────────────────

            new WorkOrder {
                Title = "Quarterly PM — Deferred by Production",
                Description = "Quarterly preventive maintenance originally scheduled this period. Deferred by production manager due to rush order.",
                Priority = "Low", Status = "Cancelled",
                EquipmentId = press.Id, ReportedById = admin.Id, AssignedToId = null,
                CreatedAt = now.AddDays(-20), UpdatedAt = now.AddDays(-19)
            },
            new WorkOrder {
                Title = "Exterior Guard Panel Touch-Up",
                Description = "Request to repaint scuffed exterior guard panels. Cosmetic only — no functional impact.",
                Priority = "Low", Status = "Cancelled",
                EquipmentId = robot.Id, ReportedById = tech1.Id, AssignedToId = null,
                CreatedAt = now.AddDays(-16), UpdatedAt = now.AddDays(-15)
            },
            new WorkOrder {
                Title = "Duplicate Spindle Noise Report",
                Description = "Second report submitted for spindle bearing noise. Already addressed under a separate active work order.",
                Priority = "Medium", Status = "Cancelled",
                EquipmentId = lathe.Id, ReportedById = tech2.Id, AssignedToId = null,
                CreatedAt = now.AddDays(-14), UpdatedAt = now.AddDays(-14)
            },
            new WorkOrder {
                Title = "False Alarm — Vibration Alert",
                Description = "High-vibration alert triggered automatically. On-site inspection found no fault — sensor tripped by nearby forklift at shift change.",
                Priority = "Low", Status = "Cancelled",
                EquipmentId = cutter.Id, ReportedById = tech1.Id, AssignedToId = null,
                CreatedAt = now.AddDays(-10), UpdatedAt = now.AddDays(-10)
            },

            // ── Pending (8) ─────────────────────────────────────────────────────

            new WorkOrder {
                Title = "Z-Axis Ballscrew Backlash",
                Description = "Operators reporting inconsistent depth of cut on facing operations. Suspected worn Z-axis ballscrew or nut.",
                Priority = "Medium", Status = "Pending",
                EquipmentId = lathe.Id, ReportedById = tech1.Id,
                CreatedAt = now.AddDays(-4)
            },
            new WorkOrder {
                Title = "Hydraulic Oil Overheating",
                Description = "Oil temperature reaching 75°C under sustained load — rated maximum is 60°C. Possible heat exchanger blockage.",
                Priority = "High", Status = "Pending",
                EquipmentId = press.Id, ReportedById = admin.Id,
                CreatedAt = now.AddDays(-3)
            },
            new WorkOrder {
                Title = "Gripper Pressure Inconsistency",
                Description = "Pneumatic gripper on end-effector intermittently releases parts during transfer. No clear pattern or trigger identified.",
                Priority = "Medium", Status = "Pending",
                EquipmentId = robot.Id, ReportedById = tech2.Id,
                CreatedAt = now.AddDays(-3)
            },
            new WorkOrder {
                Title = "Fume Extraction Fan Rattle",
                Description = "Rattling noise from fume extraction blower during cutting cycles. No performance impact yet but growing louder.",
                Priority = "Low", Status = "Pending",
                EquipmentId = cutter.Id, ReportedById = tech1.Id,
                CreatedAt = now.AddDays(-2)
            },
            new WorkOrder {
                Title = "Tailstock Quill Stiffness",
                Description = "Tailstock quill requires excessive force to advance. Likely needs cleaning and re-lubrication of the quill bore.",
                Priority = "Low", Status = "Pending",
                EquipmentId = lathe.Id, ReportedById = tech2.Id,
                CreatedAt = now.AddDays(-2)
            },
            new WorkOrder {
                Title = "Stroke Proximity Sensor Fault",
                Description = "Lower stroke proximity sensor giving inconsistent signals. Stroke position feedback unreliable — affecting batch counters.",
                Priority = "Medium", Status = "Pending",
                EquipmentId = press.Id, ReportedById = tech1.Id,
                CreatedAt = now.AddDays(-1)
            },
            new WorkOrder {
                Title = "Annual Safety Audit Preparation",
                Description = "Prepare robot workcell for upcoming third-party safety audit. Check guarding integrity, e-stop function, and speed limits.",
                Priority = "Medium", Status = "Pending",
                EquipmentId = robot.Id, ReportedById = admin.Id,
                CreatedAt = now.AddDays(-1)
            },
            new WorkOrder {
                Title = "Nozzle Concentricity Drift",
                Description = "Beam-to-nozzle concentricity has drifted — visible off-centre burn pattern on calibration target. Realignment required.",
                Priority = "High", Status = "Pending",
                EquipmentId = cutter.Id, ReportedById = tech2.Id,
                CreatedAt = now.AddDays(-1)
            }
        );

        db.SaveChanges();
    }
}
