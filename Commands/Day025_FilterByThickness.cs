namespace RevitDayByDay.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Transaction(TransactionMode.ReadOnly)]
    public class Day025_FilterByThickness : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // 300 mm in feet (Revit internal units)
            double thresholdFeet = UnitUtils.ConvertToInternalUnits(
                300, UnitTypeId.Millimeters);

            // Build a parameter filter: WALL_ATTR_WIDTH_PARAM > 300mm
            ElementId paramId = new(BuiltInParameter.WALL_ATTR_WIDTH_PARAM);

            FilterRule rule = ParameterFilterRuleFactory.CreateGreaterRule(
                paramId, thresholdFeet, 0.001);

            ElementParameterFilter paramFilter = new(rule);

            IList<Element> thickWalls = new FilteredElementCollector(doc)
                .OfClass(typeof(Wall))
                .WhereElementIsNotElementType()
                .WherePasses(paramFilter)
                .ToElements();

            // For comparison: all walls
            int totalWalls = new FilteredElementCollector(doc)
                .OfClass(typeof(Wall))
                .WhereElementIsNotElementType()
                .GetElementCount();

            StringBuilder sb = new();
            sb.AppendLine($"Total walls: {totalWalls}");
            sb.AppendLine($"Walls thicker than 300 mm: {thickWalls.Count}");
            sb.AppendLine();

            foreach (Element wall in thickWalls.Take(20))
            {
                Parameter widthParam = wall.get_Parameter(
                    BuiltInParameter.WALL_ATTR_WIDTH_PARAM);

                string width = widthParam?.AsValueString() ?? "?";
                sb.AppendLine($"  Id {wall.Id.Value}: {wall.Name} — {width}");
            }

            if (thickWalls.Count > 20)
                sb.AppendLine($"  ... and {thickWalls.Count - 20} more");

            TaskDialog.Show("Filter By Thickness", sb.ToString());

            return Result.Succeeded;
        }
    }
}
