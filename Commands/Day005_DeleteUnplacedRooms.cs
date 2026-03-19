namespace RevitDayByDay.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Architecture;
    using Autodesk.Revit.UI;

    [Transaction(TransactionMode.Manual)]
    public class Day005_DeleteUnplacedRooms : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<Room> allRooms = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .OfClass(typeof(SpatialElement))
                .Cast<Room>()
                .ToList();

            List<Room> unplaced = allRooms
                .Where(r => r.Location == null || r.Area == 0)
                .ToList();

            if (unplaced.Count == 0)
            {
                TaskDialog.Show("Delete Unplaced Rooms",
                    $"No unplaced rooms found. All {allRooms.Count} rooms are placed.");
                return Result.Succeeded;
            }

            int deletedCount = unplaced.Count;
            int remainingCount = allRooms.Count - deletedCount;

            using (Transaction tx = new(doc, "Delete Unplaced Rooms"))
            {
                tx.Start();

                foreach (Room room in unplaced)
                {
                    doc.Delete(room.Id);
                }

                tx.Commit();
            }

            TaskDialog.Show("Delete Unplaced Rooms",
                $"Deleted {deletedCount} unplaced rooms. {remainingCount} rooms remaining.");

            return Result.Succeeded;
        }
    }
}
