namespace RevitDayByDay.Commands
{
    using System.Collections.Generic;
    using System.Text;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Autodesk.Revit.UI.Selection;

    [Transaction(TransactionMode.ReadOnly)]
    public class Day028_TypeVsInstance : IExternalCommand
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
                "Select a door (or any family instance)");

            Element instance = doc.GetElement(picked);
            ElementId typeId = instance.GetTypeId();
            Element typeElem = doc.GetElement(typeId);

            if (typeElem == null)
            {
                TaskDialog.Show("Type vs Instance",
                    "Selected element has no type. Try a family instance.");
                return Result.Succeeded;
            }

            StringBuilder sb = new();

            sb.AppendLine("=== INSTANCE PARAMETERS ===");
            sb.AppendLine($"Element: {instance.Name} (Id: {instance.Id.Value})");
            sb.AppendLine();

            IList<Parameter> instanceParams = instance.GetOrderedParameters();
            int instanceCount = 0;

            foreach (Parameter p in instanceParams)
            {
                if (!p.HasValue) continue;

                string val = GetDisplayValue(p, doc);
                sb.AppendLine($"  {p.Definition.Name} = {val}");
                instanceCount++;

                if (instanceCount >= 15) break;
            }

            sb.AppendLine();
            sb.AppendLine("=== TYPE PARAMETERS ===");
            sb.AppendLine($"Type: {typeElem.Name} (Id: {typeId.Value})");
            sb.AppendLine();

            IList<Parameter> typeParams = typeElem.GetOrderedParameters();
            int typeCount = 0;

            foreach (Parameter p in typeParams)
            {
                if (!p.HasValue) continue;

                string val = GetDisplayValue(p, doc);
                sb.AppendLine($"  {p.Definition.Name} = {val}");
                typeCount++;

                if (typeCount >= 15) break;
            }

            sb.AppendLine();
            sb.AppendLine($"Instance params shown: {instanceCount}");
            sb.AppendLine($"Type params shown: {typeCount}");

            TaskDialog.Show("Type vs Instance Parameters", sb.ToString());

            return Result.Succeeded;
        }

        private static string GetDisplayValue(Parameter p, Document doc)
        {
            switch (p.StorageType)
            {
                case StorageType.String:
                    return p.AsString() ?? "";
                case StorageType.Integer:
                    return p.AsInteger().ToString();
                case StorageType.Double:
                    return p.AsValueString() ?? p.AsDouble().ToString("F4");
                case StorageType.ElementId:
                    Element linked = doc.GetElement(p.AsElementId());
                    return linked?.Name ?? p.AsValueString() ?? p.AsElementId().Value.ToString();
                default:
                    return "?";
            }
        }
    }
}
