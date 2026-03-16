namespace RevitDayByDay.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Transaction(TransactionMode.ReadOnly)]
    public class Day002_CountByCategory : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<Element> allElements = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .ToElements()
                .Where(e => e.Category != null)
                .ToList();

            var grouped = allElements
                .GroupBy(e => e.Category?.Name ?? "No Category")
                .OrderByDescending(g => g.Count())
                .Take(15)
                .ToList();

            StringBuilder sb = new();
            sb.AppendLine($"Total elements: {allElements.Count}");
            sb.AppendLine($"Top {grouped.Count} categories:");
            sb.AppendLine();

            foreach (var group in grouped)
            {
                sb.AppendLine($"{group.Key}: {group.Count()}");
            }

            TaskDialog.Show("Count by Category", sb.ToString());

            return Result.Succeeded;
        }
    }
}
