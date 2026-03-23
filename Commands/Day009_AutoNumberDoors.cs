namespace RevitDayByDay.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Architecture;
    using Autodesk.Revit.UI;

    [Transaction(TransactionMode.Manual)]
    public class Day009_AutoNumberDoors : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<FamilyInstance> doors = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Doors)
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .ToList();

            if (doors.Count == 0)
            {
                TaskDialog.Show("Auto-Number Doors",
                    "No doors found in the model.");
                return Result.Succeeded;
            }

            Dictionary<string, List<FamilyInstance>> doorsByRoom = new();
            List<FamilyInstance> doorsWithoutRoom = new();

            foreach (FamilyInstance door in doors)
            {
                Room fromRoom = door.FromRoom;
                Room toRoom = door.ToRoom;
                Room room = fromRoom ?? toRoom;

                if (room == null)
                {
                    doorsWithoutRoom.Add(door);
                    continue;
                }

                string roomNumber = room.get_Parameter(
                    BuiltInParameter.ROOM_NUMBER)?.AsString() ?? "NoNum";

                if (!doorsByRoom.ContainsKey(roomNumber))
                    doorsByRoom[roomNumber] = new List<FamilyInstance>();

                doorsByRoom[roomNumber].Add(door);
            }

            int numberedCount = 0;

            using (Transaction tx = new(doc, "Auto-Number Doors"))
            {
                tx.Start();

                foreach (KeyValuePair<string, List<FamilyInstance>> kvp
                    in doorsByRoom.OrderBy(x => x.Key))
                {
                    int seq = 1;
                    foreach (FamilyInstance door in kvp.Value)
                    {
                        string mark = $"{kvp.Key}-{seq:D2}";
                        Parameter markParam = door.get_Parameter(
                            BuiltInParameter.ALL_MODEL_MARK);

                        if (markParam != null && !markParam.IsReadOnly)
                        {
                            markParam.Set(mark);
                            numberedCount++;
                        }

                        seq++;
                    }
                }

                tx.Commit();
            }

            StringBuilder sb = new();
            sb.AppendLine($"Numbered {numberedCount} doors.");

            if (doorsWithoutRoom.Count > 0)
            {
                sb.AppendLine($"Skipped {doorsWithoutRoom.Count} doors " +
                    "without an associated room.");
            }

            sb.AppendLine();
            sb.AppendLine("Sample assignments:");
            int shown = 0;
            foreach (KeyValuePair<string, List<FamilyInstance>> kvp
                in doorsByRoom.OrderBy(x => x.Key))
            {
                if (shown >= 5) break;
                sb.AppendLine($"  Room {kvp.Key}: {kvp.Value.Count} door(s)");
                shown++;
            }

            TaskDialog.Show("Auto-Number Doors", sb.ToString());

            return Result.Succeeded;
        }
    }
}
