namespace RevitDayByDay.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Autodesk.Revit.UI.Selection;

    [Transaction(TransactionMode.Manual)]
    public class Day024_CopyParamValues : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            Reference picked = uidoc.Selection.PickObject(
                ObjectType.Element,
                "Select the SOURCE element (its Comments will be copied)");

            Element source = doc.GetElement(picked);

            Parameter sourceParam = source.get_Parameter(
                BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);

            if (sourceParam == null || !sourceParam.HasValue)
            {
                TaskDialog.Show("Copy Param Values",
                    "Source element has no Comments value. Fill it first.");
                return Result.Succeeded;
            }

            string commentValue = sourceParam.AsString();
            BuiltInCategory cat = source.Category.BuiltInCategory;

            IList<Element> sameCategory = new FilteredElementCollector(doc)
                .OfCategory(cat)
                .WhereElementIsNotElementType()
                .ToElements();

            int copied = 0;
            int skipped = 0;

            using (Transaction tx = new(doc, "Copy Comments Parameter"))
            {
                tx.Start();

                foreach (Element elem in sameCategory)
                {
                    if (elem.Id == source.Id)
                        continue;

                    Parameter p = elem.get_Parameter(
                        BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);

                    if (p == null || p.IsReadOnly)
                    {
                        skipped++;
                        continue;
                    }

                    p.Set(commentValue);
                    copied++;
                }

                tx.Commit();
            }

            TaskDialog.Show("Copy Param Values",
                $"Copied \"{commentValue}\" from {source.Name}\n" +
                $"to {copied} elements of category {source.Category.Name}.\n" +
                $"Skipped: {skipped}");

            return Result.Succeeded;
        }
    }
}
