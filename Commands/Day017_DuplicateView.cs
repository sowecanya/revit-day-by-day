using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitDayByDay.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class Day017_DuplicateView : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                View activeView = uidoc.ActiveView;

                // Check if the view can be duplicated
                if (activeView.ViewType == ViewType.DrawingSheet
                    || activeView.ViewType == ViewType.ProjectBrowser
                    || activeView.ViewType == ViewType.SystemBrowser
                    || activeView.IsTemplate)
                {
                    TaskDialog.Show("Duplicate View",
                        "The active view cannot be duplicated.\n" +
                        "Switch to a floor plan, section, elevation, or 3D view.");
                    return Result.Cancelled;
                }

                // Check which duplicate options are available
                bool canDuplicate = activeView.CanViewBeDuplicated(ViewDuplicateOption.Duplicate);
                bool canDuplicateWithDetailing = activeView.CanViewBeDuplicated(ViewDuplicateOption.WithDetailing);
                bool canDuplicateAsDependent = activeView.CanViewBeDuplicated(ViewDuplicateOption.AsDependent);

                if (!canDuplicate && !canDuplicateWithDetailing)
                {
                    TaskDialog.Show("Duplicate View",
                        $"View \"{activeView.Name}\" cannot be duplicated.");
                    return Result.Cancelled;
                }

                ElementId duplicateId;
                ElementId withDetailingId;
                string duplicateName = "";
                string withDetailingName = "";

                using (Transaction tx = new Transaction(doc, "Duplicate View"))
                {
                    tx.Start();

                    // Option 1: Duplicate (geometry only, no annotations)
                    if (canDuplicate)
                    {
                        duplicateId = activeView.Duplicate(ViewDuplicateOption.Duplicate);
                        View duplicatedView = doc.GetElement(duplicateId) as View;
                        if (duplicatedView != null)
                        {
                            duplicateName = duplicatedView.Name;
                        }
                    }

                    // Option 2: Duplicate with Detailing (geometry + annotations)
                    if (canDuplicateWithDetailing)
                    {
                        withDetailingId = activeView.Duplicate(ViewDuplicateOption.WithDetailing);
                        View withDetailingView = doc.GetElement(withDetailingId) as View;
                        if (withDetailingView != null)
                        {
                            withDetailingName = withDetailingView.Name;
                        }
                    }

                    tx.Commit();
                }

                string resultMessage = $"Original view: \"{activeView.Name}\"\n\n";

                if (!string.IsNullOrEmpty(duplicateName))
                {
                    resultMessage += $"Duplicate (no detailing):\n  \"{duplicateName}\"\n" +
                                     "  Contains: crop region, view range, visibility settings.\n" +
                                     "  Missing: dimensions, text notes, detail lines.\n\n";
                }

                if (!string.IsNullOrEmpty(withDetailingName))
                {
                    resultMessage += $"Duplicate with Detailing:\n  \"{withDetailingName}\"\n" +
                                     "  Contains: everything from the original, including all annotations.\n\n";
                }

                if (canDuplicateAsDependent)
                {
                    resultMessage += "AsDependent is also available: creates a dependent view " +
                                     "that shares the same scope box and updates with the parent.";
                }

                TaskDialog.Show("Duplicate View", resultMessage);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
