using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitDayByDay.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class Day013_SectionBoxSelection : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                // Get current selection
                ICollection<ElementId> selectedIds = uidoc.Selection.GetElementIds();

                if (selectedIds.Count == 0)
                {
                    TaskDialog.Show("Section Box", "Please select at least one element first.");
                    return Result.Cancelled;
                }

                // Compute union bounding box
                BoundingBoxXYZ unionBox = null;

                foreach (ElementId id in selectedIds)
                {
                    Element elem = doc.GetElement(id);
                    BoundingBoxXYZ bb = elem.get_BoundingBox(null);

                    if (bb == null)
                        continue;

                    if (unionBox == null)
                    {
                        unionBox = new BoundingBoxXYZ();
                        unionBox.Min = bb.Min;
                        unionBox.Max = bb.Max;
                    }
                    else
                    {
                        unionBox.Min = new XYZ(
                            Math.Min(unionBox.Min.X, bb.Min.X),
                            Math.Min(unionBox.Min.Y, bb.Min.Y),
                            Math.Min(unionBox.Min.Z, bb.Min.Z));

                        unionBox.Max = new XYZ(
                            Math.Max(unionBox.Max.X, bb.Max.X),
                            Math.Max(unionBox.Max.Y, bb.Max.Y),
                            Math.Max(unionBox.Max.Z, bb.Max.Z));
                    }
                }

                if (unionBox == null)
                {
                    TaskDialog.Show("Section Box", "Could not compute bounding box for selected elements.");
                    return Result.Failed;
                }

                // Add padding (2 feet ~ 0.6m on each side)
                double padding = 2.0;
                unionBox.Min = new XYZ(
                    unionBox.Min.X - padding,
                    unionBox.Min.Y - padding,
                    unionBox.Min.Z - padding);
                unionBox.Max = new XYZ(
                    unionBox.Max.X + padding,
                    unionBox.Max.Y + padding,
                    unionBox.Max.Z + padding);

                // Find or create a 3D view
                View3D view3d = new FilteredElementCollector(doc)
                    .OfClass(typeof(View3D))
                    .Cast<View3D>()
                    .FirstOrDefault(v => !v.IsTemplate && v.Name == "Section Box View (API)");

                using (Transaction tx = new Transaction(doc, "Set Section Box"))
                {
                    tx.Start();

                    if (view3d == null)
                    {
                        // Get 3D view family type
                        ViewFamilyType vft = new FilteredElementCollector(doc)
                            .OfClass(typeof(ViewFamilyType))
                            .Cast<ViewFamilyType>()
                            .FirstOrDefault(t => t.ViewFamily == ViewFamily.ThreeDimensional);

                        if (vft == null)
                        {
                            message = "No 3D view family type found.";
                            tx.RollBack();
                            return Result.Failed;
                        }

                        view3d = View3D.CreateIsometric(doc, vft.Id);
                        view3d.Name = "Section Box View (API)";
                    }

                    view3d.SetSectionBox(unionBox);

                    tx.Commit();
                }

                uidoc.ActiveView = view3d;

                TaskDialog.Show("Section Box",
                    $"Section box applied around {selectedIds.Count} element(s).\n" +
                    $"View: \"{view3d.Name}\".\n\n" +
                    $"Box size:\n" +
                    $"  Min: ({unionBox.Min.X:F1}, {unionBox.Min.Y:F1}, {unionBox.Min.Z:F1})\n" +
                    $"  Max: ({unionBox.Max.X:F1}, {unionBox.Max.Y:F1}, {unionBox.Max.Z:F1})");

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
