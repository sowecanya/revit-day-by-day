namespace RevitDayByDay.Commands
{
    using System;
    using System.IO;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Transaction(TransactionMode.Manual)]
    public class Day023_CreateProjectParam : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            Autodesk.Revit.ApplicationServices.Application app =
                commandData.Application.Application;

            string paramName = "Checked By";
            string groupName = "DayByDay";

            // Check if parameter already exists
            BindingMap bindingMap = doc.ParameterBindings;
            DefinitionBindingMapIterator it = bindingMap.ForwardIterator();
            while (it.MoveNext())
            {
                if (it.Key.Name == paramName)
                {
                    TaskDialog.Show("Create Project Param",
                        $"Parameter \"{paramName}\" already exists in this project.");
                    return Result.Succeeded;
                }
            }

            // Create or use a temporary shared parameter file
            string originalFile = app.SharedParametersFilename;
            string tempFile = Path.Combine(
                Path.GetTempPath(), "RevitDayByDay_SharedParams.txt");

            if (!File.Exists(tempFile))
                File.WriteAllText(tempFile, "");

            app.SharedParametersFilename = tempFile;

            DefinitionFile defFile = app.OpenSharedParameterFile();
            DefinitionGroup defGroup = defFile.Groups.get_Item(groupName)
                                      ?? defFile.Groups.Create(groupName);

            ExternalDefinitionCreationOptions options = new(paramName, SpecTypeId.String.Text);
            options.Visible = true;

            Definition definition = defGroup.Definitions.get_Item(paramName)
                                    ?? defGroup.Definitions.Create(options);

            // Build category set with Walls
            CategorySet catSet = new();
            Category wallCategory = doc.Settings.Categories
                .get_Item(BuiltInCategory.OST_Walls);
            catSet.Insert(wallCategory);

            // Create instance binding
            InstanceBinding binding = new(catSet);

            using (Transaction tx = new(doc, "Create Project Parameter"))
            {
                tx.Start();
                bindingMap.Insert(definition, binding,
                    GroupTypeId.IdentityData);
                tx.Commit();
            }

            // Restore original shared parameter file
            app.SharedParametersFilename = originalFile ?? "";

            TaskDialog.Show("Create Project Param",
                $"Parameter \"{paramName}\" created on Wall instances.\n" +
                "Check any wall's Identity Data group in Properties.");

            return Result.Succeeded;
        }
    }
}
