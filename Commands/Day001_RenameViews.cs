namespace RevitDayByDay.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Transaction(TransactionMode.Manual)]
    public class Day001_RenameViews : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            IList<View> views = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .Cast<View>()
                .Where(v => !v.IsTemplate)
                .ToList();

            Dictionary<string, int> counters = new();
            int count = 0;

            using (Transaction tx = new(doc, "Rename Views"))
            {
                tx.Start();

                foreach (View view in views)
                {
                    string typeName = view.ViewType.ToString();

                    if (!counters.ContainsKey(typeName))
                        counters[typeName] = 0;

                    counters[typeName]++;
                    string newName = $"{typeName}_{counters[typeName]:D3}";

                    try
                    {
                        view.Name = newName;
                        count++;
                    }
                    catch (Autodesk.Revit.Exceptions.ArgumentException)
                    {
                        // Name already exists — append element id to make unique
                        try
                        {
                            view.Name = $"{newName}_{view.Id.Value}";
                            count++;
                        }
                        catch
                        {
                            // Skip if still fails
                        }
                    }
                }

                tx.Commit();
            }

            TaskDialog.Show("Rename Views",
                $"Renamed {count} views out of {views.Count}.");

            return Result.Succeeded;
        }
    }
}
