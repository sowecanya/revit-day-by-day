namespace RevitDayByDay.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Architecture;
    using Autodesk.Revit.UI;

    [Transaction(TransactionMode.Manual)]
    public class Day003_ExportRoomsToCsv : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<Room> rooms = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .OfClass(typeof(SpatialElement))
                .Cast<Room>()
                .Where(r => r.Area > 0)
                .ToList();

            string desktop = Environment.GetFolderPath(
                Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktop, "rooms_export.csv");

            StringBuilder sb = new();
            sb.AppendLine("Number,Name,Area (m2),Level");

            foreach (Room room in rooms)
            {
                string number = room.get_Parameter(
                    BuiltInParameter.ROOM_NUMBER)?.AsString() ?? "";

                string name = room.get_Parameter(
                    BuiltInParameter.ROOM_NAME)?.AsString() ?? "";

                double areaRaw = room.get_Parameter(
                    BuiltInParameter.ROOM_AREA)?.AsDouble() ?? 0.0;

                // Convert from internal units (sq feet) to sq meters
                double areaSqM = UnitUtils.ConvertFromInternalUnits(
                    areaRaw, UnitTypeId.SquareMeters);

                string level = room.get_Parameter(
                    BuiltInParameter.ROOM_LEVEL_ID) != null
                    ? doc.GetElement(room.LevelId)?.Name ?? ""
                    : "";

                sb.AppendLine(
                    $"\"{number}\",\"{name}\",{areaSqM:F2},\"{level}\"");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);

            TaskDialog.Show("Export Rooms",
                $"Exported {rooms.Count} rooms to Dekstop");

            return Result.Succeeded;
        }
    }
}
