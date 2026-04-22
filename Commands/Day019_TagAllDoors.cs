using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitDayByDay.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class Day019_TagAllDoors : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                View activeView = uidoc.ActiveView;

                // Ensure we are in a plan or section view
                if (activeView.ViewType != ViewType.FloorPlan
                    && activeView.ViewType != ViewType.CeilingPlan
                    && activeView.ViewType != ViewType.Section
                    && activeView.ViewType != ViewType.Elevation)
                {
                    TaskDialog.Show("Tag Doors",
                        "Please switch to a floor plan, ceiling plan, section, or elevation view.");
                    return Result.Cancelled;
                }

                // Find a door tag type
                FamilySymbol doorTagType = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_DoorTags)
                    .OfClass(typeof(FamilySymbol))
                    .Cast<FamilySymbol>()
                    .FirstOrDefault();

                if (doorTagType == null)
                {
                    TaskDialog.Show("Tag Doors",
                        "No door tag family found in the project.\n" +
                        "Load a door tag family first.");
                    return Result.Failed;
                }

                // Get all doors visible in the active view
                List<FamilyInstance> doors = new FilteredElementCollector(doc, activeView.Id)
                    .OfCategory(BuiltInCategory.OST_Doors)
                    .WhereElementIsNotElementType()
                    .Cast<FamilyInstance>()
                    .ToList();

                if (doors.Count == 0)
                {
                    TaskDialog.Show("Tag Doors", "No doors found in the active view.");
                    return Result.Succeeded;
                }

                // Get existing door tags to avoid double-tagging
                HashSet<long> alreadyTaggedIds = new FilteredElementCollector(doc, activeView.Id)
                    .OfClass(typeof(IndependentTag))
                    .Cast<IndependentTag>()
                    .Where(t => t.Category != null
                                && t.Category.BuiltInCategory == BuiltInCategory.OST_DoorTags)
                    .SelectMany(t =>
                    {
                        List<long> ids = new List<long>();
                        try
                        {
                            IList<ElementId> taggedIds = t.GetTaggedElementIds()
                                .Select(r => r.LinkedElementId != ElementId.InvalidElementId
                                    ? r.LinkedElementId
                                    : r.HostElementId)
                                .ToList();
                            foreach (ElementId id in taggedIds)
                            {
                                ids.Add(id.Value);
                            }
                        }
                        catch
                        {
                            // Some tags may not have valid references
                        }
                        return ids;
                    })
                    .ToHashSet();

                int tagged = 0;
                int skipped = 0;

                using (Transaction tx = new Transaction(doc, "Tag All Doors"))
                {
                    tx.Start();

                    foreach (FamilyInstance door in doors)
                    {
                        // Skip already tagged doors
                        if (alreadyTaggedIds.Contains(door.Id.Value))
                        {
                            skipped++;
                            continue;
                        }

                        // Get the door's location point
                        LocationPoint locPt = door.Location as LocationPoint;
                        if (locPt == null)
                            continue;

                        XYZ tagPosition = locPt.Point;

                        // Create the tag
                        Reference doorRef = new Reference(door);
                        IndependentTag.Create(
                            doc,
                            doorTagType.Id,
                            activeView.Id,
                            doorRef,
                            false, // no leader
                            TagOrientation.Horizontal,
                            tagPosition);

                        tagged++;
                    }

                    tx.Commit();
                }

                TaskDialog.Show("Tag Doors",
                    $"Doors in view: {doors.Count}\n" +
                    $"Newly tagged: {tagged}\n" +
                    $"Already tagged (skipped): {skipped}");

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
