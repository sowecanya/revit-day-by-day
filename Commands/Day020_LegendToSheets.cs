using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitDayByDay.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class Day020_LegendToSheets : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                // Find all legend views
                List<View> legends = new FilteredElementCollector(doc)
                    .OfClass(typeof(View))
                    .Cast<View>()
                    .Where(v => v.ViewType == ViewType.Legend && !v.IsTemplate)
                    .OrderBy(v => v.Name)
                    .ToList();

                if (legends.Count == 0)
                {
                    TaskDialog.Show("Legend to Sheets", "No legend views found in the project.");
                    return Result.Succeeded;
                }

                // Use the first legend
                View legend = legends[0];

                // Collect all sheets
                List<ViewSheet> sheets = new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewSheet))
                    .Cast<ViewSheet>()
                    .Where(s => !s.IsPlaceholder)
                    .OrderBy(s => s.SheetNumber)
                    .ToList();

                if (sheets.Count == 0)
                {
                    TaskDialog.Show("Legend to Sheets", "No sheets found in the project.");
                    return Result.Succeeded;
                }

                // Find sheets that already have this legend
                HashSet<long> sheetsWithLegend = new FilteredElementCollector(doc)
                    .OfClass(typeof(Viewport))
                    .Cast<Viewport>()
                    .Where(vp => vp.ViewId.Value == legend.Id.Value)
                    .Select(vp => vp.SheetId.Value)
                    .ToHashSet();

                int placed = 0;
                int skipped = 0;

                using (Transaction tx = new Transaction(doc, "Place Legend on Sheets"))
                {
                    tx.Start();

                    foreach (ViewSheet sheet in sheets)
                    {
                        // Skip sheets that already have this legend
                        if (sheetsWithLegend.Contains(sheet.Id.Value))
                        {
                            skipped++;
                            continue;
                        }

                        // Check if the viewport can be placed
                        if (!Viewport.CanAddViewToSheet(doc, sheet.Id, legend.Id))
                        {
                            skipped++;
                            continue;
                        }

                        // Get the sheet's outline for positioning
                        BoundingBoxUV sheetOutline = sheet.Outline;
                        double sheetWidth = sheetOutline.Max.U - sheetOutline.Min.U;
                        double sheetHeight = sheetOutline.Max.V - sheetOutline.Min.V;

                        // Position in the bottom-right corner with some margin
                        double margin = 0.05 * Math.Min(sheetWidth, sheetHeight);
                        XYZ position = new XYZ(
                            sheetOutline.Max.U - sheetWidth * 0.15,
                            sheetOutline.Min.V + sheetHeight * 0.15,
                            0);

                        Viewport viewport = Viewport.Create(doc, sheet.Id, legend.Id, position);
                        placed++;
                    }

                    tx.Commit();
                }

                TaskDialog.Show("Legend to Sheets",
                    $"Legend \"{legend.Name}\" placement results:\n\n" +
                    $"Placed on: {placed} sheet(s)\n" +
                    $"Skipped: {skipped} sheet(s) (already had legend or cannot place)\n" +
                    $"Total sheets: {sheets.Count}\n\n" +
                    "Note: Legends can be placed on multiple sheets\n" +
                    "(unlike regular views which allow only one viewport).");

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
