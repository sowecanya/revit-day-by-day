namespace RevitDayByDay.Commands
{
    using System.Text;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Transaction(TransactionMode.ReadOnly)]
    public class Day021_ReadBuiltInParam : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            Reference picked = uidoc.Selection.PickObject(
                Autodesk.Revit.UI.Selection.ObjectType.Element,
                "Select an element to read parameters");

            Element elem = doc.GetElement(picked);

            BuiltInParameter[] bips =
            {
                BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS,
                BuiltInParameter.ALL_MODEL_MARK,
                BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM,
                BuiltInParameter.ELEM_CATEGORY_PARAM,
                BuiltInParameter.ALL_MODEL_COST
            };

            StringBuilder sb = new();
            sb.AppendLine($"Element: {elem.Name} (Id: {elem.Id.Value})");
            sb.AppendLine();

            foreach (BuiltInParameter bip in bips)
            {
                Parameter p = elem.get_Parameter(bip);
                if (p == null)
                {
                    sb.AppendLine($"{bip}: not available on this element");
                    continue;
                }

                string asString = p.AsString() ?? "(null)";
                string asValueString = p.AsValueString() ?? "(null)";
                double asDouble = p.StorageType == StorageType.Double
                    ? p.AsDouble()
                    : 0.0;

                sb.AppendLine($"--- {bip} ---");
                sb.AppendLine($"  StorageType: {p.StorageType}");
                sb.AppendLine($"  AsString():      {asString}");
                sb.AppendLine($"  AsValueString():  {asValueString}");
                sb.AppendLine($"  AsDouble():       {asDouble:F4}");
                sb.AppendLine();
            }

            TaskDialog.Show("Read BuiltIn Parameters", sb.ToString());

            return Result.Succeeded;
        }
    }
}
