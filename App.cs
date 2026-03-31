namespace RevitDayByDay
{
    using System.Reflection;
    using Autodesk.Revit.UI;

    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication app)
        {
       
            string tabName = "Day by Day";
            app.CreateRibbonTab(tabName);
            string path = Assembly.GetExecutingAssembly().Location;

            var p1 = app.CreateRibbonPanel(tabName, "Foundation");
            AddButton(p1, path, "Day001", "Day 1\nRename Views", "RevitDayByDay.Commands.Day001_RenameViews");
            AddButton(p1, path, "Day002", "Day 2\nCount by Cat", "RevitDayByDay.Commands.Day002_CountByCategory");
            AddButton(p1, path, "Day003", "Day 3\nRooms CSV", "RevitDayByDay.Commands.Day003_ExportRoomsToCsv");
            AddButton(p1, path, "Day004", "Day 4\nWarnings", "RevitDayByDay.Commands.Day004_ListWarnings");
            AddButton(p1, path, "Day005", "Day 5\nDel Rooms", "RevitDayByDay.Commands.Day005_DeleteUnplacedRooms");
            AddButton(p1, path, "Day006", "Day 6\nBulk Param", "RevitDayByDay.Commands.Day006_BulkSetParameter");
            AddButton(p1, path, "Day007", "Day 7\nLinked Files", "RevitDayByDay.Commands.Day007_ListLinkedFiles");
            AddButton(p1, path, "Day008", "Day 8\nNo Workset", "RevitDayByDay.Commands.Day008_NoWorksetElements");
            AddButton(p1, path, "Day009", "Day 9\nNumber Doors", "RevitDayByDay.Commands.Day009_AutoNumberDoors");
            AddButton(p1, path, "Day010", "Day 10\nTallest Wall", "RevitDayByDay.Commands.Day010_TallestWall");

            var p2 = app.CreateRibbonPanel(tabName, "Views & Sheets");
            AddButton(p2, path, "Day011", "Day 11\nView Template", "RevitDayByDay.Commands.Day011_ApplyViewTemplate");
            AddButton(p2, path, "Day012", "Day 12\nSchedule", "RevitDayByDay.Commands.Day012_CreateSchedule");
            AddButton(p2, path, "Day013", "Day 13\nSection Box", "RevitDayByDay.Commands.Day013_SectionBoxSelection");
            AddButton(p2, path, "Day014", "Day 14\nPDF Export", "RevitDayByDay.Commands.Day014_ExportSheetsPdf");
            AddButton(p2, path, "Day015", "Day 15\nCreate Sheets", "RevitDayByDay.Commands.Day015_CreateSheets");
            AddButton(p2, path, "Day016", "Day 16\nUnplaced Views", "RevitDayByDay.Commands.Day016_UnplacedViews");
            AddButton(p2, path, "Day017", "Day 17\nDuplicate View", "RevitDayByDay.Commands.Day017_DuplicateView");

            return Result.Succeeded;
        }


        public Result OnShutdown(UIControlledApplication app) => Result.Succeeded;
        private static void AddButton(RibbonPanel panel, string assemblyPath,
            string name, string text, string className)
        {
            panel.AddItem(new PushButtonData(name, text, assemblyPath, className));
        }

    }
}
