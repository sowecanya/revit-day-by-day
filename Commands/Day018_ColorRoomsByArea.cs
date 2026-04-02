using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace RevitDayByDay.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class Day018_ColorRoomsByArea : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                View activeView = uidoc.ActiveView;

                // Check if this view supports color fill schemes
                ElementId roomCategoryId = new ElementId(BuiltInCategory.OST_Rooms);

                if (!activeView.SupportedColorFillCategoryIds().Contains(roomCategoryId))
                {
                    TaskDialog.Show("Color Rooms", "This view does not support color fill schemes for rooms.");
                    return Result.Failed;
                }

                // Find an existing color fill scheme for rooms to duplicate
                ColorFillScheme sourceScheme = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_ColorFillSchema)
                    .Cast<ColorFillScheme>()
                    .FirstOrDefault(s => s.CategoryId == roomCategoryId);

                if (sourceScheme == null)
                {
                    TaskDialog.Show("Color Rooms", "No existing color fill scheme for rooms found. Create one manually first.");
                    return Result.Failed;
                }

                // Find solid fill pattern
                FillPatternElement solidFill = new FilteredElementCollector(doc)
                    .OfClass(typeof(FillPatternElement))
                    .Cast<FillPatternElement>()
                    .FirstOrDefault(fp => fp.GetFillPattern().IsSolidFill);

                if (solidFill == null)
                {
                    TaskDialog.Show("Color Rooms", "No solid fill pattern found in the project.");
                    return Result.Failed;
                }

                // Get the Area built-in parameter ID
                ElementId areaParamId = new ElementId(BuiltInParameter.ROOM_AREA);


                // Count rooms for the report
                List<Room> rooms = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Rooms)
                    .WhereElementIsNotElementType()
                    .Cast<Room>()
                    .Where(r => r.Area > 0)
                    .ToList();

                if (rooms.Count == 0)
                {
                    TaskDialog.Show("Color Rooms", "No placed rooms with area found.");
                    return Result.Succeeded;
                }

                // Area thresholds in square feet (Revit internal units)
                // 20 m² ≈ 215.28 ft², 50 m² ≈ 538.20 ft²
                double smallThreshold = UnitUtils.ConvertToInternalUnits(20, UnitTypeId.SquareMeters);
                double largeThreshold = UnitUtils.ConvertToInternalUnits(50, UnitTypeId.SquareMeters);

                int smallCount = rooms.Count(r => r.Area < smallThreshold);
                int mediumCount = rooms.Count(r => r.Area >= smallThreshold && r.Area < largeThreshold);
                int largeCount = rooms.Count(r => r.Area >= largeThreshold);

                string schemeName = "Area Heat Map";

                using (Transaction tx = new Transaction(doc, "Color Rooms by Area"))
                {
                    tx.Start();

                    // Check if our scheme already exists, delete it to recreate
                    ColorFillScheme existing = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_ColorFillSchema)
                        .Cast<ColorFillScheme>()
                        .FirstOrDefault(s => s.Name == schemeName);

                    if (existing != null)
                        doc.Delete(existing.Id);

                    // Duplicate the source scheme
                    ElementId newSchemeId = sourceScheme.Duplicate(schemeName);
                    ColorFillScheme scheme = doc.GetElement(newSchemeId) as ColorFillScheme;

                    // Configure: color by Area parameter, by range
                    scheme.ParameterDefinition = areaParamId;
                    scheme.IsByRange = true;

                    // Create range entries
                    var entries = new List<ColorFillSchemeEntry>();

                    var entrySmall = new ColorFillSchemeEntry(StorageType.Double);
                    entrySmall.SetDoubleValue(smallThreshold);
                    entrySmall.Color = new Color(255, 100, 100);  // Red — small rooms
                    entrySmall.FillPatternId = solidFill.Id;
                    entrySmall.Caption = "< 20 m²";
                    entries.Add(entrySmall);

                    var entryMedium = new ColorFillSchemeEntry(StorageType.Double);
                    entryMedium.SetDoubleValue(largeThreshold);
                    entryMedium.Color = new Color(100, 200, 100); // Green — medium rooms
                    entryMedium.FillPatternId = solidFill.Id;
                    entryMedium.Caption = "20–50 m²";
                    entries.Add(entryMedium);

                    var entryLarge = new ColorFillSchemeEntry(StorageType.Double);
                    entryLarge.SetDoubleValue(UnitUtils.ConvertToInternalUnits(100, UnitTypeId.SquareMeters));
                    entryLarge.Color = new Color(100, 100, 255);  // Blue — large rooms
                    entryLarge.FillPatternId = solidFill.Id;
                    entryLarge.Caption = "> 50 m²";
                    entries.Add(entryLarge);

                    scheme.SetEntries(entries);

                    // Apply scheme to the active view
                    activeView.SetColorFillSchemeId(roomCategoryId, scheme.Id);

                    tx.Commit();
                }

                TaskDialog.Show("Color Rooms by Area",
                    $"Applied \"Area Heat Map\" color scheme to \"{activeView.Name}\".\n\n" +
                    $"Small (< 20 m²): {smallCount} room(s) — red\n" +
                    $"Medium (20–50 m²): {mediumCount} room(s) — green\n" +
                    $"Large (> 50 m²): {largeCount} room(s) — blue\n\n" +
                    $"Total: {rooms.Count} room(s)");

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
