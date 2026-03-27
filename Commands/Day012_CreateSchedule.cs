using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitDayByDay.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class Day012_CreateSchedule : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                ViewSchedule schedule;

                using (Transaction tx = new Transaction(doc, "Create Wall Schedule"))
                {
                    tx.Start();

                    // Create a schedule for Walls category
                    ElementId wallCategoryId = new ElementId(BuiltInCategory.OST_Walls);
                    schedule = ViewSchedule.CreateSchedule(doc, wallCategoryId);
                    schedule.Name = "Wall Schedule (API)";

                    // Get all schedulable fields
                    IList<SchedulableField> schedulableFields = schedule.Definition.GetSchedulableFields();

                    SchedulableField FindField(BuiltInParameter parameter)
                    {
                        return schedulableFields.FirstOrDefault(
                            f => f.ParameterId == new ElementId(parameter));
                    }

                    // Find and add Type field
                    SchedulableField typeField = FindField(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM);

                    if (typeField != null)
                    {
                        schedule.Definition.AddField(typeField);
                    }

                    // Find and add Length field
                    SchedulableField lengthField = FindField(BuiltInParameter.CURVE_ELEM_LENGTH);

                    if (lengthField != null)
                    {
                        schedule.Definition.AddField(lengthField);
                    }

                    // Find and add Area field
                    SchedulableField areaField = FindField(BuiltInParameter.HOST_AREA_COMPUTED);

                    if (areaField != null)
                    {
                        schedule.Definition.AddField(areaField);
                    }

                    // Try to add a filter for exterior walls (Function = Exterior)
                    SchedulableField functionField = FindField(BuiltInParameter.FUNCTION_PARAM);

                    if (functionField != null)
                    {
                        ScheduleField addedFunctionField = schedule.Definition.AddField(functionField);

                        // Function is stored as WallFunction enum/int, not string.
                        ScheduleFilter filter = new ScheduleFilter(
                            addedFunctionField.FieldId,
                            ScheduleFilterType.Equal,
                            (int)WallFunction.Exterior);

                        schedule.Definition.AddFilter(filter);

                        // Hide the Function column since it is used only for filtering
                        addedFunctionField.IsHidden = true;
                    }

                    tx.Commit();
                }

                // Open the created schedule
                uidoc.ActiveView = schedule;

                TaskDialog.Show("Create Schedule",
                    $"Wall schedule \"{schedule.Name}\" created successfully.\n\n" +
                    "Fields: Type, Length, Area.\n" +
                    "Filter: Exterior walls only (if Function field was available).");

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
