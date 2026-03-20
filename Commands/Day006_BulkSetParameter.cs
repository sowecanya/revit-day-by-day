namespace RevitDayByDay.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Transaction(TransactionMode.Manual)]
    public class Day006_BulkSetParameter : IExternalCommand
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
                TaskDialog.Show("Bulk Set Parameter",
                    "No walls found in the model.");
                return Result.Succeeded;
            }

            string stamp = $"Checked by API {DateTime.Now:yyyy-MM-dd HH:mm}";
            int updatedCount = 0;
            int skippedCount = 0;

            using (Transaction tx = new(doc, "Bulk Set Comments"))
            {
                tx.Start();

                foreach (Wall wall in walls)
                {
                    Parameter param = wall.get_Parameter(
                        BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);

                    if (param != null && !param.IsReadOnly)
                    {
                        param.Set(stamp);
                        updatedCount++;
                    }
                    else
                    {
                        skippedCount++;
                    }
                }

                tx.Commit();
            }

            TaskDialog.Show("Bulk Set Parameter",
                $"Updated {updatedCount} walls with \"{stamp}\".\n" +
                $"Skipped: {skippedCount}.");

            return Result.Succeeded;
        }
    }
}
