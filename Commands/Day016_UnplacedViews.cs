using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitDayByDay.Commands
{
    [Transaction(TransactionMode.ReadOnly)]
    public class Day016_UnplacedViews : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                // Get all ViewIds that are placed on sheets via Viewports
                HashSet<long> placedViewIds = new FilteredElementCollector(doc)
                    .OfClass(typeof(Viewport))
                    .Cast<Viewport>()
                    .Select(vp => vp.ViewId.Value)
                    .ToHashSet();

                // Get all views that could be placed on sheets
                List<View> allViews = new FilteredElementCollector(doc)
                    .OfClass(typeof(View))
                    .Cast<View>()
                    .Where(v => !v.IsTemplate
                                && v.ViewType != ViewType.Schedule
                                && v.ViewType != ViewType.DrawingSheet
                                && v.ViewType != ViewType.ProjectBrowser
                                && v.ViewType != ViewType.SystemBrowser
                                && v.ViewType != ViewType.Internal
                                && v.ViewType != ViewType.Undefined)
                    .OrderBy(v => v.ViewType.ToString())
                    .ThenBy(v => v.Name)
                    .ToList();

                // Find unplaced views
                List<View> unplacedViews = allViews
                    .Where(v => !placedViewIds.Contains(v.Id.Value))
                    .ToList();

                int totalViews = allViews.Count;
                int placedCount = allViews.Count - unplacedViews.Count;

                if (unplacedViews.Count == 0)
                {
                    TaskDialog.Show("Unplaced Views",
                        $"All {totalViews} view(s) are placed on sheets. Nothing to report.");
                    return Result.Succeeded;
                }

                // Group by view type for readability
                var grouped = unplacedViews
                    .GroupBy(v => v.ViewType)
                    .OrderBy(g => g.Key.ToString());

                string details = "";
                foreach (var group in grouped)
                {
                    details += $"\n--- {group.Key} ({group.Count()}) ---\n";
                    foreach (View v in group)
                    {
                        details += $"  {v.Name}\n";
                    }
                }

                // Truncate if too long for TaskDialog
                if (details.Length > 3000)
                {
                    details = details.Substring(0, 3000) + "\n... (truncated)";
                }

                TaskDialog.Show("Unplaced Views",
                    $"Found {unplacedViews.Count} unplaced view(s) out of {totalViews} total.\n" +
                    $"Placed on sheets: {placedCount}.\n" +
                    details);

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
