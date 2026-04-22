namespace RevitDayByDay.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Transaction(TransactionMode.Manual)]
    public class Day022_WriteSharedParam : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            string paramName = "Comments";
            string newValue = "Updated by Revit API";

            IList<Element> walls = new FilteredElementCollector(doc)
                .OfClass(typeof(Wall))
                .WhereElementIsNotElementType()
                .ToElements();

            if (walls.Count == 0)
            {
                TaskDialog.Show("Write Shared Param", "No walls found in the model.");
                return Result.Succeeded;
            }

            int updated = 0;
            int notFound = 0;
            StringBuilder sb = new();

            using (Transaction tx = new(doc, "Write Shared Parameter"))
            {
                tx.Start();

                foreach (Element wall in walls)
                {
                    Parameter p = wall.LookupParameter(paramName);
                    if (p == null)
                    {
                        notFound++;
                        continue;
                    }

                    if (p.IsReadOnly)
                    {
                        sb.AppendLine($"Wall {wall.Id.Value}: parameter is read-only");
                        continue;
                    }

                    p.Set(newValue);
                    updated++;
                }

                tx.Commit();
            }

            sb.Insert(0, $"Updated {updated} walls.\n" +
                         $"Parameter not found on {notFound} walls.\n\n");

            if (notFound == walls.Count)
            {
                sb.AppendLine($"Hint: create a shared parameter named \"{paramName}\" " +
                              "on walls before running this command.");
            }

            TaskDialog.Show("Write Shared Param", sb.ToString());

            return Result.Succeeded;
        }
    }
}
