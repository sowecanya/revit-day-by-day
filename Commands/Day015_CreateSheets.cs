using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitDayByDay.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class Day015_CreateSheets : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                // Define sheets to create (number, name)
                List<(string Number, string Name)> sheetData = new List<(string, string)>
                {
                    ("A-101", "Ground Floor Plan"),
                    ("A-102", "First Floor Plan"),
                    ("A-103", "Second Floor Plan"),
                    ("A-201", "North Elevation"),
                    ("A-202", "South Elevation"),
                    ("A-301", "Section A-A"),
                    ("A-302", "Section B-B"),
                    ("A-401", "Wall Details"),
                    ("A-501", "Door Schedule"),
                    ("A-601", "General Notes")
                };

                // Find a title block family type
                FamilySymbol titleBlock = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_TitleBlocks)
                    .OfClass(typeof(FamilySymbol))
                    .Cast<FamilySymbol>()
                    .FirstOrDefault();

                if (titleBlock == null)
                {
                    TaskDialog.Show("Create Sheets",
                        "No title block family found in the project.\n" +
                        "Load a title block family first.");
                    return Result.Failed;
                }

                // Check for existing sheet numbers to avoid duplicates
                HashSet<string> existingNumbers = new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewSheet))
                    .Cast<ViewSheet>()
                    .Select(s => s.SheetNumber)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                int created = 0;
                int skipped = 0;
                List<string> createdSheets = new List<string>();

                using (Transaction tx = new Transaction(doc, "Create Sheets"))
                {
                    tx.Start();

                    // Ensure title block is activated
                    if (!titleBlock.IsActive)
                    {
                        titleBlock.Activate();
                        doc.Regenerate();
                    }

                    foreach (var (number, name) in sheetData)
                    {
                        if (existingNumbers.Contains(number))
                        {
                            skipped++;
                            continue;
                        }

                        ViewSheet sheet = ViewSheet.Create(doc, titleBlock.Id);
                        sheet.SheetNumber = number;
                        sheet.Name = name;

                        createdSheets.Add($"{number} - {name}");
                        created++;
                    }

                    tx.Commit();
                }

                string resultMessage = $"Created {created} sheet(s) using title block \"{titleBlock.FamilyName}\".";

                if (skipped > 0)
                {
                    resultMessage += $"\nSkipped {skipped} sheet(s) (number already exists).";
                }

                if (createdSheets.Count > 0)
                {
                    resultMessage += "\n\nNew sheets:\n" + string.Join("\n", createdSheets);
                }

                TaskDialog.Show("Create Sheets", resultMessage);

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
