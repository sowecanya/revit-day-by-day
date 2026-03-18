namespace RevitDayByDay.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Transaction(TransactionMode.ReadOnly)]
    public class Day004_ListWarnings : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            IList<FailureMessage> warnings = doc.GetWarnings();

            if (warnings.Count == 0)
            {
                TaskDialog.Show("Warnings", "No warnings found in this model.");
                return Result.Succeeded;
            }

            var grouped = warnings
                .GroupBy(w => w.GetDescriptionText())
                .OrderByDescending(g => g.Count())
                .Take(10)
                .ToList();

            StringBuilder sb = new();
            sb.AppendLine($"Total warnings: {warnings.Count}");
            sb.AppendLine($"Top {grouped.Count} warning types:");
            sb.AppendLine();

            foreach (var group in grouped)
            {
                sb.AppendLine($"{group.Key}: {group.Count()}");
            }

            TaskDialog.Show("Model Warnings", sb.ToString());

            return Result.Succeeded;
        }
    }
}
