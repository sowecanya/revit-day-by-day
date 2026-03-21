namespace RevitDayByDay.Commands
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Transaction(TransactionMode.ReadOnly)]
    public class Day007_ListLinkedFiles : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<RevitLinkInstance> links = new FilteredElementCollector(doc)
                .OfClass(typeof(RevitLinkInstance))
                .Cast<RevitLinkInstance>()
                .ToList();

            if (links.Count == 0)
            {
                TaskDialog.Show("Linked Files",
                    "No Revit links found in the model.");
                return Result.Succeeded;
            }

            StringBuilder sb = new();
            int loadedCount = 0;
            int unloadedCount = 0;

            foreach (RevitLinkInstance link in links)
            {
                RevitLinkType linkType = doc.GetElement(link.GetTypeId())
                    as RevitLinkType;

                if (linkType == null)
                    continue;

                ExternalFileReference extRef =
                    linkType.GetExternalFileReference();

                string filePath = extRef != null
                    ? ModelPathUtils.ConvertModelPathToUserVisiblePath(
                        extRef.GetAbsolutePath())
                    : "Unknown path";

                string fileName = Path.GetFileName(filePath);

                bool isLoaded = RevitLinkType.IsLoaded(doc, linkType.Id);
                string status = isLoaded ? "Loaded" : "Not Loaded";

                if (isLoaded)
                    loadedCount++;
                else
                    unloadedCount++;

                sb.AppendLine($"{fileName} — {status}");
            }

            sb.AppendLine();
            sb.AppendLine($"Total: {links.Count} links " +
                $"({loadedCount} loaded, {unloadedCount} not loaded)");

            TaskDialog.Show("Linked Files", sb.ToString());

            return Result.Succeeded;
        }
    }
}
