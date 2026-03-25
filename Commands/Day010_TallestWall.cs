namespace RevitDayByDay.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Transaction(TransactionMode.ReadOnly)]
    public class Day010_TallestWall : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<Wall> walls = new FilteredElementCollector(doc)
                .OfClass(typeof(Wall))
                .Cast<Wall>()
                .ToList();

            if (walls.Count == 0)
            {
                TaskDialog.Show("Tallest Wall",
                    "No walls found in the model.");
                return Result.Succeeded;
            }

            Wall tallest = null;
            double maxHeightFeet = 0;

            foreach (Wall wall in walls)
            {
                BoundingBoxXYZ bb = wall.get_BoundingBox(null);
                if (bb == null)
                    continue;

                double heightFeet = bb.Max.Z - bb.Min.Z;

                if (heightFeet > maxHeightFeet)
                {
                    maxHeightFeet = heightFeet;
                    tallest = wall;
                }
            }

            if (tallest == null)
            {
                TaskDialog.Show("Tallest Wall",
                    "Could not determine wall heights. " +
                    "No walls have valid bounding boxes.");
                return Result.Succeeded;
            }

            double heightMm = UnitUtils.ConvertFromInternalUnits(
                maxHeightFeet, UnitTypeId.Millimeters);

            string typeName = tallest.WallType?.Name ?? "Unknown";
            double lengthFeet = tallest.get_Parameter(
                BuiltInParameter.CURVE_ELEM_LENGTH)?.AsDouble() ?? 0;
            double lengthMm = UnitUtils.ConvertFromInternalUnits(
                lengthFeet, UnitTypeId.Millimeters);

            TaskDialog.Show("Tallest Wall",
                $"Tallest wall found:\n\n" +
                $"Element ID: {tallest.Id.Value}\n" +
                $"Type: {typeName}\n" +
                $"Height: {heightMm:F0} mm\n" +
                $"Length: {lengthMm:F0} mm\n\n" +
                $"Total walls analyzed: {walls.Count}");

            return Result.Succeeded;
        }
    }
}
