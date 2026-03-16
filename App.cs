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
