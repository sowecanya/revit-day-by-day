namespace RevitDayByDay.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Transaction(TransactionMode.ReadOnly)]
    public class Day008_NoWorksetElements : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            if (!doc.IsWorkshared)
            {
                TaskDialog.Show("No Workset Elements",
                    "This model does not use worksharing. " +
                    "Enable worksharing first to use worksets.");
                return Result.Succeeded;
            }

            WorksetTable worksetTable = doc.GetWorksetTable();
            WorksetId defaultWorksetId = worksetTable.GetActiveWorksetId();

            List<Element> modelElements = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .Where(e => e.Category != null
                    && e.Category.CategoryType == CategoryType.Model
                    && e.WorksetId != WorksetId.InvalidWorksetId)
                .ToList();

            Dictionary<string, int> defaultWorksetByCategory = new();
            int totalOnDefault = 0;

            foreach (Element el in modelElements)
            {
                Workset ws = worksetTable.GetWorkset(el.WorksetId);
                if (ws == null)
                    continue;

                if (el.WorksetId == defaultWorksetId)
                {
                    string catName = el.Category?.Name ?? "No Category";

                    if (defaultWorksetByCategory.ContainsKey(catName))
                        defaultWorksetByCategory[catName]++;
                    else
                        defaultWorksetByCategory[catName] = 1;

                    totalOnDefault++;
                }
            }

            if (totalOnDefault == 0)
            {
                TaskDialog.Show("No Workset Elements",
                    "All model elements are assigned to non-default worksets. " +
                    "Model is clean.");
                return Result.Succeeded;
            }

            StringBuilder sb = new();
            sb.AppendLine($"Elements still on the default workset " +
                $"(\"{worksetTable.GetWorkset(defaultWorksetId).Name}\"):");
            sb.AppendLine();

            foreach (KeyValuePair<string, int> kvp in
                defaultWorksetByCategory.OrderByDescending(x => x.Value))
            {
                sb.AppendLine($"  {kvp.Key}: {kvp.Value}");
            }

            sb.AppendLine();
            sb.AppendLine($"Total: {totalOnDefault} elements need workset assignment.");

            TaskDialog.Show("No Workset Elements", sb.ToString());

            return Result.Succeeded;
        }
    }
}
